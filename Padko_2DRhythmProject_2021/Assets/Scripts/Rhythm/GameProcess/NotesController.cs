using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteEditor.Utility;
using NoteEditor.Notes;
using NoteEditor.DTO;
using Game.MusicSelect;
using DG.Tweening;
using System.IO;
using UniRx;
using System;

namespace Game.Process
{
    public class NotesController : SingletonMonoBehaviour<NotesController>
    {
        [SerializeField] private float maxY = 1120, minY = -40; //音符显示的最高和最低位置，低于最低位置的音符会被回收

        /// <summary>
        /// 音符、長按的音符、兩個長按音符之間的延長線
        /// </summary>
        public GameObject notePrefab, holdNotePrefab, holdBarPrefab;
        
        public List<MusicDTO.Note> notesInfo; //音符信息
        
        // List<NoteObject> _listNotesActivated = new();//激活的音符对象
        public List<NoteObject> listNotesWaiting = new(); //等待激活的音符对象

        // [SerializeField] public int notesPerFrame = 50; //每一帧最多生成的音符数量

        #region 數值

        [HideInInspector] public int laneCount; //轨道数量
        [HideInInspector] public int BPM; //速度
        [HideInInspector] public int offset; //开始的偏移时间
        [HideInInspector] public float frequency; //音乐频率
        [HideInInspector] public float length; //音乐时长
        [HideInInspector] public string musicName;
        [HideInInspector] public int musicLevel;
        public int playerOffset; //玩家调整的延迟 todo:這啥??
        public int notesCount; //總共要打幾個note

        #endregion

        [HideInInspector] public ReactiveProperty<float> startTime = new(3); //开始游戏前等待时间

        public GameObject startTimerObj;
        public Text startTimerText;

        //private float preOffset; //为了让第一个音符从顶部落下的偏移时间
        //[HideInInspector] public float playTime = 0f; //音乐播放了多久

        #region 狀態判斷

        [HideInInspector] public bool playing = false; //播放中
        private bool loading = false; //加载中
        [HideInInspector] public bool ready = false; //加载完毕
        [HideInInspector] public bool isEditMode = false; //編輯模式

        #endregion


        [HideInInspector] public float noteMoveTime = 1.5f; //从出现到落到位置需要多久
        
        //public float noteWaitTime = 5f;   //在开始移动前多久生成
        
        private float noteMoveY;
        public E_RhythmKeysType keyType;
        [HideInInspector] public float maxNoteDoPosY; //最後會抵達的位置
        [HideInInspector] public float noteDoPosY; //todo:當前的位置  要用以做比例計算 將進度條的value傳過來 將其轉變成位置的數值後 重做dotween的位移動畫
        private Tweener doMoveNote;
        public NoteTrack[] noteTracks;
        public GameObject[] noteTrackObj;

        void Awake()
        {
            #region 获取判定点、判定生效区的位置

            //for (int i=0; i<targetLines.Count; i++)
            //{
            //   targetYs[i] = targetLines[i].GetComponent<RectTransform>().anchoredPosition.y;
            //}    
            //clickFieldY = clickLine.rectTransform.anchoredPosition.y;
            //for (int i = 0; i < clickFieldYs.Length; i++)
            //{
            //    clickFieldYs[i] = targetLines[i].rectTransform.anchoredPosition.y + 100;
            //}

            #endregion

            #region 判断是否加载乐谱

            if (NotesContainer.Instance == null || NotesContainer.Instance.json == null)
            {
                Debug.LogError("Data not loaded!");
                return;
            }

            #endregion

            #region 读取乐谱信息

            var editData = JsonUtility.FromJson<MusicDTO.EditData>(NotesContainer.Instance.json);
            notesInfo = editData.notes;
            laneCount = editData.maxBlock;
            BPM = editData.BPM;
            offset = editData.offset;
            notesCount = NotesContainer.Instance.notesCount;
            musicName = NotesContainer.Instance.musicNameExtension;
            musicLevel = NotesContainer.Instance.level;
            isEditMode = NotesContainer.Instance.isEditMode;
            //Debug.Log("musicName:" + musicName);
            ComboPresenter.Instance.SetNoteScore(notesCount);

            #endregion

            if (laneCount == 4)
                keyType = E_RhythmKeysType.four;
            else if (laneCount == 5)
                keyType = E_RhythmKeysType.five;
            else
                keyType = E_RhythmKeysType.six;

            foreach (var item in noteTrackObj)
            {
                item.SetActive(false);
            }

            noteTrackObj[(int) keyType].SetActive(true);

            #region 获取音符下落速度信息

            int speed = PlayerSettings.Instance.speed;

            //noteMoveTime = 2.5f - 0.021f * speed;
            //noteMoveToEndTime = noteMoveTime * (maxY - minY) / (maxY - targetY);
            //noteMoveY = (maxY - targetY) / noteMoveTime;


            var targetY = noteTracks[(int) keyType].targetLines[0].GetComponent<RectTransform>().anchoredPosition.y;
            noteMoveTime = (2.5f - 0.021f * speed) / 2;

            noteMoveY = (maxY - targetY) / noteMoveTime;

            #endregion


            #region 判断是否加载音乐

            startTime.Value = 3;
            startTime.SubscribeToText(startTimerText);

            if (NotesContainer.Instance.music == null)
            {
                Debug.LogError("Music not loaded!");
                return;
            }

            #endregion

            #region 加载音乐信息

            //audioMusic.clip = NotesContainer.Instance.music;

            frequency = NotesContainer.Instance.music.frequency;
            length = NotesContainer.Instance.music.length;

            #endregion

            #region 加载玩家设定

            playerOffset = PlayerSettings.Instance.playerOffset;

            #endregion
        }

        void Start()
        {
            var doAtkAni = Observable.Timer(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    StartGame();
                    //isAttack.Value = true;//0.005
                }).AddTo(this);
        }

        public void StartGame()
        {
            playing = false;
            loading = true;
            ready = false;

            ResetMusic();

            PlayController.Instance.Init(laneCount);

            StartCoroutine(GenerateNotes());
            //playing = true;
            //audioMusic.Play();
        }

        private void ResetMusic()
        {
            MusicController.Instance.SetMusic(NotesContainer.Instance.music);
        }

        private void ClearNotes()
        {
            if (listNotesWaiting.Count > 0)
            {
                foreach (NoteObject gn in listNotesWaiting)
                {
                    Destroy(gn.gameObject);
                }
            }

            listNotesWaiting.Clear();
        }

        /// <summary>
        /// 根據type值生成不同種Note
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private NoteObject CreateNote(int type)
        {
            var prefab = type == 1 ? notePrefab : holdNotePrefab;

            GameObject obj = Instantiate(prefab);
            obj.SetActive(true);
            // noteList.Add(obj);
            return obj.GetComponent<NoteObject>();
        }

        // public void NoteLookAt(Transform noteTrans) //2D遊戲要這個幹嘛?
        // {
        //     Vector2 direction = noteTrans.position - noteTrans.position;
        //     // Vector2 direction = Vector2.zero;
        //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //     noteTrans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // }

        private IEnumerator GenerateNotes()
        {
            // GameNotePool pool = GameNotePool.Instance;
            ClearNotes();
            
            PlayerController.Instance.SwitchNoteAssetSkill(notePrefab, holdNotePrefab);
            
            for (int i = 0; i < notesInfo.Count; i++)
            {
                //生成音符
                if (notesInfo[i].block >= laneCount)
                    continue;
                var dto = notesInfo[i];
                var type = dto.type;
                var note = CreateNote(type);
                
                note.Init(dto);
                listNotesWaiting.Add(note);
                
                
                note.transform.SetParent(noteTracks[(int) keyType].noteFields[note.Block()]);
                note.transform.localScale = new Vector2(0.8f, 0.8f);

                // NoteLookAt(note.transform);
                
                //音符的時間
                note.time = ConvertUtils.NoteToSamples(note, frequency, BPM) + playerOffset;

                //音符的位置         
                var targetY = noteTracks[(int) keyType].targetLines[0].GetComponent<RectTransform>().anchoredPosition.y;
                
                float y = targetY + noteMoveY * MusicController.Instance.SampleToTime(note.time + offset);
                note.SetPosition(new Vector2(0, y));

                //對於長按條，生成結尾音符和hold條
                if (type == 2)
                {
                    if (dto.notes.Count == 0)
                    {
                        Debug.Log("Hold key has no ending!");
                    }
                    else
                    {
                        var endDto = dto.notes[0];

                        var noteBar = CreateNote(2);
                        noteBar.Init(endDto);
                        noteBar.transform.SetParent(noteTracks[(int) keyType].noteFields[note.Block()]);
                        noteBar.transform.localScale = Vector3.one;

                        //音符時間
                        noteBar.time = ConvertUtils.NoteToSamples(noteBar, frequency, BPM) + playerOffset;

                        //音符位置          
                        float yy = targetY + noteMoveY * MusicController.Instance.SampleToTime(noteBar.time + offset);
                        noteBar.SetPosition(new Vector2(0, yy));

                        //生成長條
                        var bar = Instantiate(holdBarPrefab, noteTracks[(int) keyType].noteFields[noteBar.Block()])
                            .GetComponent<HoldingBar>();
                        bar.transform.SetAsFirstSibling();
                        bar.SetPosition(0, y);
                        bar.SetHeight(yy - y);

                        //將尾判音符和長條與頭部音符綁定
                        note.AddChainedNote(noteBar);
                        note.AddHoldingBar(bar);
                        noteBar.AddHoldingBar(bar);
                    }
                }
            }

            //讓音符提前下落，令其可在正確時間到達正確位置
            startTimerObj.SetActive(true);
            float time = startTime.Value;

            SetDoNoteMove(0, time,
                noteTracks[(int) keyType].noteFields[0].GetComponent<RectTransform>().anchoredPosition.y + 3000);
            
            StartCoroutine(SetStartTimer());
            
            yield return new WaitForSeconds(time);
            
            ready = true;
            startTimerObj.SetActive(false);
            yield return null;
        }

        IEnumerator SetStartTimer()
        {
            for (int i = 0; i < startTime.Value; i++)
            {
                yield return new WaitForSeconds(1);
                startTime.Value -= 1;
            }

            yield return null;
        }


        public void SetNoteFieldsPos(float posY)
        {
            for (int i = 0; i < laneCount; i++)
            {
                RectTransform noteField = noteTracks[(int) keyType].noteFields[i].GetComponent<RectTransform>();
                var pos = new Vector2(noteField.anchoredPosition.x, posY);
                noteTracks[(int) keyType].noteFields[i].GetComponent<RectTransform>().anchoredPosition = pos;
                //讓譜面播放進度可以透過slider來調整
                //doMoveNote = noteTracks[(int)keyType].noteFields[i].GetComponent<RectTransform>().DOAnchorPosY(endValue, length);
                //doMoveNote.SetEase(Ease.Linear);
                //doMoveNote.SetUpdate(false);
            }
        }

        public void SetDoNoteMove(float endValue, float length, float posY = default)
        {
            if (doMoveNote != null)
            {
                doMoveNote.Kill();
            }

            if (posY != default)
            {
                SetNoteFieldsPos(posY);
                //Debug.Log("posY:" + posY);
            }

            //讓譜面播放進度可以透過slider來調整
            for (int i = 0; i < laneCount; i++)
            {
                doMoveNote = noteTracks[(int) keyType].noteFields[i].GetComponent<RectTransform>()
                    .DOAnchorPosY(endValue, length);
                doMoveNote.SetEase(Ease.Linear);
                doMoveNote.SetUpdate(false);
            }
        }


        void Update()
        {
            #region 加载完成后，音符开始下落

            if (loading && ready)
            {
                playing = true;
                loading = false;

                //audioMusic.Play();
                MusicController.Instance.PlayMusic();
                maxNoteDoPosY = -noteMoveY * length;
                SetDoNoteMove(maxNoteDoPosY, length);
            }

            #endregion

            if (playing)
            {
                noteDoPosY = noteTracks[(int) keyType].noteFields[0].GetComponent<RectTransform>().anchoredPosition.y;
            }

            if (!playing) return;

            #region 将所有音符加入判定队列中

            while (listNotesWaiting.Count > 0)
            {
                var gn = listNotesWaiting[0];
                listNotesWaiting.Remove(gn);
                PlayController.Instance.NoteEnqueue(gn);

                if (gn.Type() == 2)
                {
                    var cn = gn.GetChainedNote();
                    PlayController.Instance.NoteEnqueue(cn);
                }
            }

            #endregion

            #region 音乐播放完成后，结束游戏

            if (!MusicController.Instance.IsPlaying() && !MusicController.Instance.isPause)
            {
                if (playing)
                {
                    ComboPresenter.Instance.ShowResult();
                    playing = false;
                }
            }

            #endregion
        }
    }
}

[System.Serializable]
public class NoteTrack
{
    public List<Transform> targetLines;
    public List<Transform> noteFields;
    public List<Text> noteFieldsText;
}