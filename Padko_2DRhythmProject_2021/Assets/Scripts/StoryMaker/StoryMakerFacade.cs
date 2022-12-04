using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StoryMakerFacade :IFacade<StoryMakerFacade>
{
    private PlayStoryUI mPlayStoryUI;
    private StoryEditorUI mStoryEditorUI;
    private ReturnMainMenuUI mReturnMainMenuUI;

    [SerializeField]
    public SerializableDictionary<string, Button> rightClickButtons;


    public override void Init()
    {
        base.Init();
        #region UISystem Init
        Debug.Log(nameof(PlayStoryUI));
        mPlayStoryUI = UITool.FindChild<PlayStoryUI>(mUISystemObj, nameof(PlayStoryUI));
        mStoryEditorUI = UITool.FindChild<StoryEditorUI>(mUISystemObj, nameof(StoryEditorUI));
        mReturnMainMenuUI = UITool.FindChild<ReturnMainMenuUI>(mUISystemObj, nameof(ReturnMainMenuUI));

        UISystemRun(SystemStateFlag.Start, mPlayStoryUI, mStoryEditorUI, mReturnMainMenuUI);
        #endregion
        #region GameSystem Init
        //mGameMode = UITool.FindChild<GameMode>(mSystemObj, "GameMode");
        //GameSystemRun(SystemStateFlag.Start, mGameMode);
        #endregion
    }

    public override void Release()
    {
        base.Release();
    }
    public override void Update()
    {
        base.Update();
    }
}


