using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Game.MainMenu;

public class MainMenuFacade : IFacade<MainMenuFacade> 
{
    //public bool isStartGame;
    public bool isMenuOpen;

    public GameSettingsAttr gameSettingsAttr = new GameSettingsAttr();
    public MusicSettingsAttr musicAttr = new MusicSettingsAttr();
    public KeyboardSettingsAttr keyboardSettingsAttr = new KeyboardSettingsAttr();
    public PlayerSettings playerSettings;
    public StoryAttr storyAttr = new StoryAttr();
    public SceneChangeFlag sceneChangeFlag;


    //UISystem
    public StartGameUI mStartGameUI;
    private HowPlayUI mHowPlayUI;
    //private SettingsUI mSettingsUI;
    // private AboutUI mAboutUI;
    private ExitUI mExitUI;
    // private TestGameUI mTestGameUI;
    private IBaseUISystem settingsMenu;
    private IBaseUISystem noteEditorUI;

    //GameSystem

    private GameMode mGameMode;

    public override void Init()
    {
        base.Init();

        #region UISystem Init
        mStartGameUI = UITool.FindChild<StartGameUI>(mUISystemObj, nameof(StartGameUI));
        mHowPlayUI = UITool.FindChild<HowPlayUI>(mUISystemObj, nameof(HowPlayUI));
        //mSettingsUI = UITool.FindChild<SettingsUI>(mUISystemObj, nameof(SettingsUI));
        // mAboutUI = UITool.FindChild<AboutUI>(mUISystemObj, nameof(AboutUI));
        mExitUI = UITool.FindChild<ExitUI>(mUISystemObj, nameof(ExitUI));
        // mTestGameUI = UITool.FindChild<TestGameUI>(mUISystemObj, nameof(TestGameUI));
        settingsMenu = UITool.FindChild<SettingMenu>(mUISystemObj, nameof(SettingMenu));
        //settingsMenu = SettingMenu.Instance;
        noteEditorUI = NoteEditorUI.Instance;

        //UISystemRun(SystemStateFlag.Start,mStartGameUI, mHowPlayUI, mAboutUI, mExitUI, mTestGameUI, settingsMenu);


        UISystemRun(SystemStateFlag.Start, mStartGameUI, mHowPlayUI, /*mAboutUI,*/ mExitUI, /*mTestGameUI,*/ settingsMenu, noteEditorUI);
        //UISystemRun(SystemStateFlag.Start, settingsMenu , noteEditorUI);
        #endregion

        #region GameSystem Init
        mGameMode = UITool.FindChild<GameMode>(mSystemObj, nameof(GameMode));
        GameSystemRun(SystemStateFlag.Start, mGameMode);
        #endregion
        PlayerSettings.Instance.InitSavaData();

    }
    public override void Update()
    {
        base.Update();
        UISystemRun(SystemStateFlag.Update, mStartGameUI, mHowPlayUI, /*mAboutUI,*/ mExitUI, /*mTestGameUI,*/ settingsMenu);
        GameSystemRun(SystemStateFlag.Update, mGameMode);
    }
    public override void Release()
    {
        base.Release();
    }
    public static void SwitchScene(SceneStateController mController)
    {
        var facade = MainMenuFacade.Instance;
        
        if (facade.SceneChangeFlag == SceneChangeFlag.StoryMakerScene)
        {
            mController.SetState(new StoryMakerState(mController));
        }
        else if (facade.SceneChangeFlag == SceneChangeFlag.MusicSelect)
        {
            mController.SetState(new MusicSelectState(mController));
        }
        else if (facade.SceneChangeFlag == SceneChangeFlag.NoteEditor)
        {
            mController.SetState(new NoteEditorState(mController));
        }
    }
}
