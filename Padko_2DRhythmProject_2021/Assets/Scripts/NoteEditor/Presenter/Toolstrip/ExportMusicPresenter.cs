using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using ICSharpCode.SharpZipLib.Zip;
using NoteEditor.Model;
using NoteEditor.Utility;
using SFB;
using UnityEditor;
using zipLib;



namespace System.IO
{
    public static class ExtendedMethod
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            if(newName.Length<=0)
            {
                fileInfo.MoveTo(fileInfo.Directory.FullName);
            }
            else
            {
                fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, newName));
            }
        }
    }
}


namespace NoteEditor.Presenter
{
    public class ExportMusicPresenter : SingletonMonoBehaviour<ExportMusicPresenter>
    {
        public Button exportWindowsButton; //�ɥX���s
        public GameObject exportSettingObj; 
        public GameObject noteEditorObj;
        public Button exportConfirmationButton; //�ɥX�T�w���s

        public InputField nameInput;
        public InputField levelInput;//����
        public InputField authorInput;
        public InputField sourceInput;//���v�����ӷ�

        public ReactiveProperty<string> _name = new ReactiveProperty<string>();
        public ReactiveProperty<string> _author = new ReactiveProperty<string>();
        public ReactiveProperty<string> _source = new ReactiveProperty<string>();
        public ReactiveProperty<int> _level = new ReactiveProperty<int>();

        public ReactiveProperty<bool> isOn = new ReactiveProperty<bool>();


        public ExportType exportType;

        string jsonFileName;
        string musicName;
        string musicFileName;
        string jsonFoldPath;
        string jsonPath;
        string musicInfoJsonPath;


        // Start is called before the first frame update
        void Start()
        {
            jsonFileName = Path.ChangeExtension(EditData.Name.Value, "json");
            musicName = Path.GetFileNameWithoutExtension(EditData.Name.Value);
            musicFileName = EditData.Name.Value;
            jsonFoldPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes/" + musicName);
            jsonPath = Path.Combine(jsonFoldPath, jsonFileName);
            musicInfoJsonPath = Path.Combine(jsonFoldPath, "MusicInfo.json");
            exportSettingObj.SetActive(false);
            EditData.Name.Subscribe(_ =>
            {
                jsonFileName = Path.ChangeExtension(EditData.Name.Value, "json");
                musicName = Path.GetFileNameWithoutExtension(EditData.Name.Value);
                musicFileName = EditData.Name.Value;
                jsonFoldPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes/" + musicName);
                jsonPath = Path.Combine(jsonFoldPath, jsonFileName);
                musicInfoJsonPath = Path.Combine(jsonFoldPath, "MusicInfo.json") ;
                

            });
            #region Input�ݩ��ܧ�
            nameInput.OnValueChangedAsObservable().Subscribe(_ =>
            {
                if (nameInput.text.Length + Path.GetExtension(EditData.Name.Value).Length > 50)
                {
                    nameInput.text = "�W�ٶW�L�W��(50�r��)";
                }

                _name.Value = nameInput.text + Path.GetExtension(EditData.Name.Value) ;
            });
            levelInput.OnValueChangedAsObservable().Subscribe(_ =>
            {
                //_level.Value = levelInput.text;
            });
            authorInput.OnValueChangedAsObservable().Subscribe(_ =>
            {
                //�n���D�X�i�W�O����
                if (authorInput.text.Length > 50)
                {
                    authorInput.text = "�W�ٶW�L�W��(50�r��)";
                }
                _author.Value = authorInput.text;
            });
            sourceInput.OnValueChangedAsObservable().Subscribe(_ =>
            {
                if (sourceInput.text.Length > 50)
                {
                    sourceInput.text = "�W�ٶW�L�W��(50�r��)";
                }
                _source.Value = sourceInput.text;
            });

            _name.Subscribe(_ =>
            {
                nameInput.text = Path.GetFileNameWithoutExtension(_name.Value);
                
            });
            _level.Subscribe(_ =>
            {
                levelInput.text = "" + _level.Value;
            });
            _author.Subscribe(_ =>
            {
                authorInput.text = _author.Value;
            });
            _source.Subscribe(_ =>
            {
                sourceInput.text = _source.Value;
            });

            #endregion


            isOn.Subscribe(isOnValue =>
            {
                exportSettingObj.SetActive(isOnValue);
                noteEditorObj.SetActive(!isOnValue);
                //if (_name.Value.Length <= 0)
                //{
                //    _name.Value = EditData.Name.Value;
                //}
                if(isOnValue)
                {
                    _name.Value = EditData.Name.Value;
                }
            });

            exportConfirmationButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                isOn.Value = false;
                SavePresenter.Instance.Save();
                var infoJson = Serialize();
                if (exportType == ExportType.save)
                {
                    return;
                }
                var musicPath = Path.Combine(Settings.WorkSpacePath.Value, "Musics" , _name.Value);
                jsonFoldPath = Path.Combine(Settings.WorkSpacePath.Value, "Notes", Path.GetFileNameWithoutExtension(_name.Value));
                string[] zipFilePaths = new string[] {
                //music
                    musicPath,
                //jsonFold
                    jsonFoldPath
                 };
                var saveExtensionList = new[] {
                    new ExtensionFilter(Path.GetFileNameWithoutExtension(_name.Value), "padko"),
                };
                var savePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", Path.GetFileNameWithoutExtension(_name.Value), saveExtensionList);
                var ZipCB = new ZipResult();
                var zipOutPath = Path.ChangeExtension(savePath, "padko");
                ZipHelper.Zip(zipFilePaths, zipOutPath, null, ZipCB);
                
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
                noteEditorObj.SetActive(true);
            });

            exportWindowsButton.OnClickAsObservable() 
            .Subscribe(_ =>
            {

                setExportSetting(ExportType.export);
                
            });
        }

        public string Serialize()
        {
            var musicInfo = new MusicInfoData();
            musicInfo.level = _level.Value;
            musicInfo.author = _author.Value;
            musicInfo.source = _source.Value;
            Debug.Log("EditData.Name.Value:" + EditData.Name.Value);
            Debug.Log("_name.Value:" + _name.Value);
            string newMusicInfoPath = musicInfoJsonPath;
            if (Path.GetFileNameWithoutExtension(_name.Value).Length <= 0)
            {
                _name.Value = "NoName" + Path.GetExtension(EditData.Name.Value);
            }

            if (_name.Value != EditData.Name.Value)//�W�٤��P�i��ק�W�٪��ʧ@ �p�G�o���ɮ׸��|�����@�˦W�٪��ɮ� �h�ܧ��ɮצW��
            {

                string path = Application.streamingAssetsPath;
                var fileNameExtension = EditData.Name.Value;
                Debug.Log("fileNameExtension:" + fileNameExtension);
                var sourceMusic = Path.Combine(path, fileNameExtension);
                var distMusic = Path.Combine(path, "Musics", fileNameExtension);

                var jsonFold = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension));
                var sourceJsonPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(fileNameExtension, "json"));
                var distJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(fileNameExtension, "json"));

                var sourceInfoJsonPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension), "MusicInfo.json");
                var distInfoJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), "MusicInfo.json");

                var sourceDirectory = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension));
                var distDirectory = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension));


                _name.Value = UnityTool.FileDontReapeatName(Path.Combine(path, "Musics"), _name.Value);//�^�ǥi�Ϊ����ɦW�ٴN�n�F�a

                //���֦W��
                FileInfo musicFile = new FileInfo(distMusic);
                string newName = Path.Combine(Path.Combine(path, "Musics"), _name.Value);
                //string newName = UnityTool.FileDontReapeatName(musicPath, _name.Value);
                if (File.Exists(newName))
                    File.Delete(newName);

                musicFile.Rename(newName);
                Debug.Log("musicFileNewName:" + newName);
                var newMusicFilePath = newName;


                //json�ɮצW��
                FileInfo jsonFile = new FileInfo(distJsonPath);
                newName = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(_name.Value, "json"));
                Debug.Log("jsonFileNewName:" + newName);
                //newName = UnityTool.FileDontReapeatName(jsonFoldPath, _name.Value + ".json");
                newMusicInfoPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(_name.Value), "MusicInfo.json");
                if (File.Exists(newName))
                    File.Delete(newName);
                jsonFile.Rename(newName);

                //�ܧ��Ƨ��W��
                // FileInfo jsonFoldFile = new FileInfo(jsonFold);
                newName = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(_name.Value));
                if (Directory.Exists(newName))
                    Directory.Delete(newName,true);
                Directory.Move(jsonFold, newName);
                //newName = UnityTool.FileDontReapeatName(notesFold, _name.Value, true);
                // jsonFoldFile.Rename(newName);
                Debug.Log("jsonFoldFileNewName:" + newName);
                
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
                //MusicSelector.Instance.Load();
            }
            musicInfo.name = _name.Value;
            Settings.IsOpen.Value = false;//�o��isOpen������N������false�F
            MusicLoader.Instance.Load(_name.Value);

            var infoJson = UnityEngine.JsonUtility.ToJson(musicInfo);
            File.WriteAllText(newMusicInfoPath, infoJson, System.Text.Encoding.UTF8);

            return infoJson;
        }


        public void Deserialize(string json) 
        {

            var musicInfo = UnityEngine.JsonUtility.FromJson<MusicInfoData>(json);
            if(musicInfo.name.Length > 0)
            {
                _name.Value = musicInfo.name;
            }
            else
            {
                _name.Value = EditData.Name.Value;
            }
            _level.Value = musicInfo.level;
            _author.Value = musicInfo.author;
            _source.Value = musicInfo.source;
        }

        public void setExportSetting(ExportType exportType)
        {
            if (Audio.Source.clip == null)
                return;
            this.exportType = exportType;
            isOn.Value = true;
            if (File.Exists(musicInfoJsonPath))
            {
                var json = File.ReadAllText(musicInfoJsonPath, System.Text.Encoding.UTF8);
                Deserialize(json);
            }
            if(exportType == ExportType.export)
                noteEditorObj.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

public enum ExportType
{
    save,
    export
}
public enum VtuberType
{
    padko
}