using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Game;

public class GameMode : IBaseGameSystem
{
    //改歌模式
    // public Text title;
    // public Text titleShadow;
    // public GameObject padko;
    // public GameObject kusoPadko;
    //
    // public AudioClip padkoBGM;
    // public AudioClip kusoPadkoBGM;
    public AudioSource BGMSource;
   
    public override void Init()
    {
        base.Init();
        var facade = MainMenuFacade.Instance;
        if (Time.timeScale != 1)
            Time.timeScale = 1;
        #region story Init
        facade.storyAttr.storyEnding = -1;
        facade.storyAttr.favorability = 0;
        facade.storyAttr.storyDays = 0;
        #endregion
        
        

        Observable.EveryUpdate().Where(_=> BGMSource.volume != PlayerSettings.Instance.musicVolume).Subscribe(_ =>
        {
            BGMSource.volume = PlayerSettings.Instance.musicVolume;
        }).AddTo(this);


    }
}
