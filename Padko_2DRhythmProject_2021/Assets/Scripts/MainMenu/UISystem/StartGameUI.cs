using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using NoteEditor.Model;
using System.IO;
using UnityEngine.Networking;

public class StartGameUI : IBaseUISystem
{
    public Button selectMusicButton;
    public Button selectCharaButton;
    public Button shareLinkButton;
    public Button backButton;
    public GameObject defaultUI;
    public GameObject selectUI;

    public override void Init()
    {
        base.Init();
        backButton.OnClickAsObservable().Subscribe(_ =>
        {
            defaultUI.SetActive(true);
            selectUI.SetActive(false);
        });
        selectCharaButton.OnClickAsObservable().Subscribe(_ =>
        {
            //切換到選角場景
            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.CharaSelect;
        });
        selectMusicButton.OnClickAsObservable().Subscribe(_ =>
        {
            var facade = MainMenuFacade.Instance;
            if (!facade.isMenuOpen)
            {
                facade.SceneChangeFlag = SceneChangeFlag.MusicSelect;
            }
        });
        shareLinkButton.OnClickAsObservable().Subscribe(_ =>
        {
            Application.OpenURL(GetCloudLink());
        });
    }
    public static string GetCloudLink()
    {
        return "https://drive.google.com/drive/folders/1yIO6DMj4l-2pmJPUUN51BZSl59ifJ2Iw?usp=sharing";
    }
    public static string GetTutorialLink()
    {
        return "https://hackmd.io/@-sozKcyYR1usQsCoRhFr-A/SJKqZhMzo";
    }
    public override void Release()
    {
        base.Release();
    }
    public override void UIUpdate()
    {
        base.UIUpdate();
    }


    public override void OpenUI()
    {
        OpenSelectUI();
    }
    public void OpenSelectUI()
    {
        var facade = MainMenuFacade.Instance;
        if (facade.isMenuOpen)
            return;
        defaultUI.SetActive(false);
        selectUI.SetActive(true);

        //if (!facade.gameSettingsAttr.kusoMode)
        //{
        //    facade.SceneChangeFlag = SceneChangeFlag.MusicSelect;
        //}
        //else
        //{
        //    if (!mUIObj.activeSelf)
        //    {
        //        mUIObj.SetActive(true);
        //        MainMenuFacade.Instance.isMenuOpen = true;
        //    }
        //}
    }



    public void StartGameKusoMode(MusicTrack musicTrack)
    {
        //var facade = MainMenuFacade.Instance;
        ////facade.musicAttr.musicTrackAttr = musicTrack.musicTrackAttr;
        //facade.musicAttr.selectTrack = musicTrack.musicTrackAttr.musicTrack;
        //if (facade.gameSettingsAttr.liveMode)
        //    facade.musicAttr.selectAudio = musicTrack.musicTrackAttr.liveAudio;
        //else
        //    facade.musicAttr.selectAudio = musicTrack.musicTrackAttr.generalAudio;
        //facade.SceneChangeFlag = SceneChangeFlag.RhythmScene;
    }
    public void StartGameJson(string audioStr, string json)
    {
        var facade = MainMenuFacade.Instance;
        facade.musicAttr.selectJsonStr = json;
        facade.SceneChangeFlag = SceneChangeFlag.RhythmScene;
    }

}
[System.Serializable]
public struct MusicTrack
{
    public MusicTrackAttr musicTrackAttr;
    public Button stageButton;
}


