using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Game.MusicSelect;
using NoteEditor;
using NoteEditor.Model;
using NoteEditor.Utility;
using NoteEditor.Presenter;
using System.IO;
using System;

public class GameEditUI : MonoBehaviour
{
    public Button startGameButton;
    public static bool isEditMode;
    bool isLoad;
    // Start is called before the first frame update
    void Start()
    {

        //從Edit狀態回來的話，要將檔案設回原本歌曲
        if(isEditMode)
        {
            //設定檔案
            if(NotesContainer.Instance.musicNameExtension != null)
            {
                MusicLoader.Instance.Load(NotesContainer.Instance.musicNameExtension);
            }
        }

        isLoad = false;

        startGameButton.OnClickAsObservable().Where(_ => !isLoad && EditData.Name.Value.Length > 0 && EditData.Name.Value != "Note Editor")
        .Subscribe(_ =>
        {
            isLoad = true;

            SavePresenter.Instance.Save();
            //先存檔後，再進入到遊戲中測試譜面
            string musicNameExtension = EditData.Name.Value;
            string musicName = Path.GetFileNameWithoutExtension(EditData.Name.Value);
            AudioClip music = null;

            var pathlist = UnityTool.GetMusicPath(musicNameExtension);
            //StartCoroutine(Game.MusicSelect.MusicSelector.LoadMusic(musicName, musicNameExtension, isEdit: true, audioClip: music));
            //StartCoroutine(Game.MusicSelect.MusicSelector.LoadMusic(musicName, musicNameExtension, isEdit: true,
            //    callbackOnFinish: (clip) =>
            //    {
            //        Debug.Log("clip.name:" + clip.name);
            //        music = clip;
            //    }
            //    ));
            string json = "";
            int level = 0;
            int notesCount = 0;
            bool autoplay;
            var loadMusicStream = Observable.FromCoroutine(_ => 
                Game.MusicSelect.MusicSelector.LoadMusic(musicName, musicNameExtension, isEdit: true,
                    callbackOnFinish: (clip) =>
                    {
                        Debug.Log("clip.name:" + clip.name);
                        music = clip;
                    }
                ));
            string readJson = File.ReadAllText(pathlist[(int)MusicPathType.noteJson], System.Text.Encoding.UTF8);

            //UniRx.IObservable<Unit> loadJsonStream = loadJsonStream = Observable.FromCoroutine(_ =>
            //    Game.MusicSelect.MusicSelector.LoadMusicInfoFromJson(readJson, isEdit: true,
            //         (callBackNotesCount, callBackjson) =>
            //        {
            //            notesCount = callBackNotesCount;
            //            json = callBackjson;
            //        }
            //    ));

            //var bStream = Observable.FromCoroutine(_ => B());

            Observable.WhenAll(loadMusicStream)
                .Subscribe(_ =>
                {
                    //LoadMusicInfoFromJson(json);
                    Game.MusicSelect.MusicSelector.LoadMusicInfoFromJson(readJson, isEdit: true,
                         callbackOnFinish: (callBackNotesCount, callBackjson) =>
                         {
                             notesCount = callBackNotesCount;
                             json = callBackjson;
                         });
                    var musicInfoData = UnityTool.DeserializeJson<MusicInfoData>(pathlist[(int)MusicPathType.infoJson]);
                    if(musicInfoData != null)
                        level = musicInfoData.level;

                    //level = ExportMusicPresenter.Instance._level.Value;

                    //Debug.Log("music.name:" + music.name);

                    isEditMode = true;
                    NotesContainer.Instance.SetContainer(music, json, musicNameExtension, level, notesCount, isEditMode);
                    MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.RhythmScene;
                    isLoad = false;
                    //Debug.LogError("C");
                }).AddTo(this);

            //Debug.Log("music.name:" + music.name);
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
