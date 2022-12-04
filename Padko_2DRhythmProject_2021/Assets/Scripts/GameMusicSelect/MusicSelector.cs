using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using NoteEditor.Utility;
using NoteEditor.Model;
using NoteEditor.DTO;
using NoteEditor.Presenter;
using UniRx;
using System;
using UnityEditor;
using Game;
using System.Linq;

//using Ggross.Utils;

namespace Game.MusicSelect
{
    public class MusicSelector : SingletonMonoBehaviour<MusicSelector>
    {
        //todo:選曲，看完這個再去看遊戲

        [SerializeField] private Text pathSpaceHolder;

        /// <summary>
        /// 音樂檔目錄
        /// </summary>
        public string musicPath;

        /// <summary>
        /// 譜Json檔目錄
        /// </summary>
        public string jsonPath;


        #region 歌曲列表

        /// <summary>
        /// 父物件
        /// </summary>
        [SerializeField] private Transform contentField;
        /// <summary>
        /// 歌曲prefab
        /// 讀取List的內容，改變UI
        /// </summary>
        [SerializeField] private GameObject itemPrefab;
        /// <summary>
        /// NoteProp的集合，按 .Count 來決定生成幾個itemPrefab
        /// </summary>
        private readonly List<NoteProp> _items = new();

        #endregion


        [SerializeField] private Text pathInfo, musicInfo, fileInfo;

        [SerializeField] private Text bpm, time, notes;

        [SerializeField] private AudioSource audioSource;

        private bool notesLoaded = false, musicLoaded = false;
        [SerializeField] private Button startButton, autoButton;



        public Text scoreText, accuracyText, maxComboText, FCAPText, gradeText;
        public string score, accuracy, maxCombo, FCAP, grade;

        AudioClip music;
        string json;
        string musicNameExtension;
        int level;
        int notesCount;
        bool autoplay;
        bool isEditMode;


        void Awake()
        {
            startButton.onClick.AddListener(StartGame);
            startButton.interactable = false;

            autoButton.onClick.AddListener(StartGame);


            autoButton.interactable = false;
        }

        void Start()
        {
            PlayerSettings.Instance.musicPath = Application.streamingAssetsPath + "\\Musics";
            musicPath = PlayerSettings.Instance.musicPath;
            jsonPath = Application.streamingAssetsPath + "\\Notes";

            var p = musicPath;
            pathSpaceHolder.text = p;

            Load();
        }

        void Update()
        {
            var ready = musicLoaded && notesLoaded;
            startButton.interactable = ready;
            autoButton.interactable = ready;

            if (Input.GetKeyDown(KeyCode.Escape))
                Quit();
        }

        /// <summary>
        /// 从文本栏的地址中读取所有json文件并生成按钮
        /// </summary>
        public void LoadJson()
        {
            ImportMusicPresenter.ImportMusic(true);
            Observable.Timer(TimeSpan.FromSeconds(0))
                .Subscribe(_ => { Load(); }).AddTo(this);
        }

        public void Load()
        {
            var load = LoadJsonInDirectory(musicPath);
            if (load == 1)
                PlayerSettings.Instance.SetMusicPath(musicPath);
        }

        public void Quit()
        {
            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.MainMenuScene;
            //SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// 显示指定路径中的json文件，并创建对应的按钮
        /// </summary>
        /// <param name="path">文件路径</param>
        private int LoadJsonInDirectory(string path)
        {
            ContentClear();
            notesLoaded = false;
            musicLoaded = false;
            //根據音頻來創立列表
            if (Directory.Exists(path)) //notes
            {
                DirectoryInfo directory = new DirectoryInfo(path);

                //獲得所有音頻的數量
                FileInfo[] files = directory.GetFiles();

                for (int i = 0; i < files.Length; i++)
                {
                    //讀取音頻的檔案
                    string fileName = files[i].Name;

                    //Debug.Log("fileName:"+ fileName);
                    if (!fileName.EndsWith(".mp3") && !fileName.EndsWith(".wav")) continue;

                    CreateListItem(fileName);
                }

                //根據難度排序
                _items.Sort((x, y) =>
                {
                    int compare = x.level.Value.CompareTo(y.level.Value);
                    return compare;
                }); // 遞增
                for (int i = 0; i < _items.Count; i++)
                {
                    //Debug.Log("level:" + items[i].level.Value);
                    _items[i].transform.SetSiblingIndex(i);
                }

                return 1;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// 根据按钮的数据，读取.wav音乐和.json文件存储的谱面信息
        /// </summary>
        /// <param name="item"></param>
        public void LoadMusicInfo(NoteProp item)
        {
            // ShowLoadInfo(pathInfo, "");
            // ShowLoadInfo(musicInfo, "");
            // ShowLoadInfo(fileInfo, "");
            // ShowLoadInfo(maxComboText, "");
            // ShowLoadInfo(FCAPText, "");
            // ShowLoadInfo(scoreText, "");
            // ShowLoadInfo(accuracyText, "");

            // if (item == null || !_items.Contains(item))
            // {
            //     ShowLoadInfo(fileInfo, "Incorrect file!");
            //     return;
            // }

            notesLoaded = false;
            musicLoaded = false;


            //读取谱面信息

            var name = Path.ChangeExtension(item.fileName, ".json");
            var dir = this.jsonPath;

            //string dir = Path.Combine(Path.GetDirectoryName(path), "Notes");
            var folderName = Path.GetFileNameWithoutExtension(item.fileName);
            var folderPath = Path.Combine(dir, folderName);
            Debug.Log("folderPath:" + folderPath);

            //獲得文件名檢查是否有這個資料夾存在
            // if (!Directory.Exists(folderPath))
            // {
            //     ShowLoadInfo(fileInfo, "Incorrect file!");
            //     return;
            // }

            string jsonPath = Path.Combine(folderPath, name);
            string json = File.ReadAllText(jsonPath, System.Text.Encoding.UTF8);

            LoadMusicInfoFromJson(json);

            //加载音乐
            StartCoroutine(LoadMusic(item.fileName, item.fileNameExtension));

            #region 讀取最佳成績

            var saveData = PlayerSettings.Instance.saveData;
            bool isFlag = false;
            Debug.Log("item.fileNameExtension:" + item.fileNameExtension);

            if (saveData != null)
            {
                foreach (var data in saveData.listHighScoreDatas)
                {
                    if (data.name == item.fileNameExtension)
                    {
                        score = "Score:" + data.score; //item.fileName
                        maxCombo = "Combo:" + data.maxCombo;
                        FCAP = "Clear";
                        if (data.isFC)
                        {
                            FCAP = "FullCombo";
                        }

                        if (data.isAP)
                        {
                            FCAP = "AllPerfect";
                        }

                        grade = "Grade:" + data.grade.ToString();
                        accuracy = "Accuracy:" + data.accuracy + "%";
                        isFlag = true;
                        break;
                    }
                }
            }

            if (!isFlag)
            {
                score = "Score:?"; //item.fileName
                maxCombo = "Combo:?";
                grade = "Grade:?";
                FCAP = "";
                accuracy = "Accuracy:?";
            }

            #endregion

            //讀取難度


            Debug.Log("accuracy:" + accuracy);

            // ShowLoadInfo(scoreText, score);
            // ShowLoadInfo(maxComboText, maxCombo);
            // ShowLoadInfo(FCAPText, FCAP);
            // ShowLoadInfo(gradeText, grade);
            // ShowLoadInfo(accuracyText, accuracy);
            musicNameExtension = item.fileNameExtension;
            level = item.level.Value;
        }

        //讀取.wav音樂檔案

        public static IEnumerator LoadMusic(string n, string fileNameExtension, bool isEdit = false,
            Action<AudioClip> callbackOnFinish = default)
        {
            var name = fileNameExtension;
            string musicPath = UnityTool.GetMusicPath(name)[0];
            UnityWebRequest _unityWebRequest = default;
            if (Path.GetExtension(fileNameExtension) == ".mp3")
            {
                _unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(musicPath, AudioType.MPEG);
            }
            else if (Path.GetExtension(fileNameExtension) == ".wav")
            {
                _unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(musicPath, AudioType.WAV);
            }

            AudioClip clip = null;

            yield return _unityWebRequest.SendWebRequest();


            if (_unityWebRequest.result == UnityWebRequest.Result.ProtocolError ||
                _unityWebRequest.result ==
                UnityWebRequest.Result
                    .ConnectionError) //(_unityWebRequest.isHttpError || _unityWebRequest.isNetworkError)
            {
                Debug.Log(_unityWebRequest.error.ToString());
            }
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(_unityWebRequest);
            }


            if (clip == null)
            {
            }
            else
            {
                clip.name = n;

                if (!isEdit)
                {
                    MusicSelector.Instance.audioSource.clip = clip;
                    MusicSelector.Instance.audioSource.volume = PlayerSettings.Instance.musicVolume;
                    MusicSelector.Instance.audioSource.Play();
                    MusicSelector.Instance.time.text = "Time\t\t" + ComputeUtility.FormatTwoTime((int)clip.length);
                    MusicSelector.Instance.music = clip;
                    MusicSelector.Instance.musicLoaded = true;
                }
                else
                {
                    if (callbackOnFinish != default)
                        callbackOnFinish(clip);
                }
            }

            yield return null;
        }


        /// <summary>
        /// 根据一个json文件读取音乐及其信息
        /// </summary>
        /// <param name="json"></param>
        public static void LoadMusicInfoFromJson(string json, bool isEdit = false,
            Action<int, string> callbackOnFinish = default)
        {
            var editData = JsonUtility.FromJson<MusicDTO.EditData>(json);

            if (editData == null)
            {
                //ShowLoadInfo(fileInfo, "Incorrect Json file!");
                return;
            }

            //ShowLoadInfo(fileInfo, "Json loaded!");

            int notesCount = 0;
            foreach (var note in editData.notes)
            {
                //Debug.Log("noteBlock:" + note.block);
                if (note.block >= editData.maxBlock)
                    continue;
                if (note.type == 2)
                    notesCount += 2;
                else
                    notesCount++;
            }

            if (!isEdit)
            {
                Instance.notesCount = notesCount;
                Instance.notesLoaded = true;
                Instance.bpm.text = "BPM\t\t" + editData.BPM;
                Instance.notes.text = "Notes\t" + notesCount;
                Instance.json = json;
            }
            else
            {
                if (callbackOnFinish != default)
                    callbackOnFinish(notesCount, json);
            }
        }

        private void ContentClear()
        {
            //Debug.Log("items.Count:" + items.Count);
            for (int i = 0; i < _items.Count; i++)
            {
                NoteProp item = _items[i];
                Destroy(item.gameObject);
                //Debug.Log("Destory:" + item.fileName);
            }
            _items.Clear();
        }

        private void CreateListItem(string f)
        {
            string fileName = f.Remove(f.LastIndexOf("."));
            var musicInfoData = UnityTool.DeserializeJson<MusicInfoData>(UnityTool.GetMusicPath(f)[2]);
            if (musicInfoData == null)
            {
                return;
            }

            // var obj = Instantiate(itemPrefab, contentField).GetComponent<NoteProp>();
            // NoteProp item = obj.GetComponent<NoteProp>();
            var item = Instantiate(itemPrefab, contentField).GetComponent<NoteProp>();

            item.SetName(fileName, f);
            item.level.Value = musicInfoData.level;
            string jsonPath = UnityTool.GetMusicPath(f)[1];
            string json = File.ReadAllText(jsonPath, System.Text.Encoding.UTF8);
            var editData = JsonUtility.FromJson<MusicDTO.EditData>(json);
            item.block.Value = editData.maxBlock;
            _items.Add(item);
            //Debug.Log("Create:" + item.fileName);
        }

        // private void ShowLoadInfo(Text text, string info)
        // {
        //     text.text = info;
        // }

        public void StartGame()
        {
            if (notesLoaded && musicLoaded)
            {
                NotesContainer.Instance.SetContainer(music, json, musicNameExtension, level, notesCount);

                MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.RhythmScene;
            }
            else
            {
                Debug.LogError("File not fully loaded!");
            }
        }
    }
}