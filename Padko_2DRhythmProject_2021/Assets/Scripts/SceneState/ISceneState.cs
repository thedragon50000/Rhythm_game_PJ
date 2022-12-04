using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISceneState 
{
    protected string mSceneName;
    protected SceneStateController mController;
    public ISceneState(string sceneName, SceneStateController controller)
    {
        mSceneName = sceneName;
        mController = controller;
    }
    public string SceneName
    {
        get{ return mSceneName; }
    }

    public virtual void StateAwake() { }
    public virtual void StateStart() {
        if (MainMenuFacade.Instance != null)
            MainMenuFacade.Instance.isMenuOpen = false;
    }
    public virtual void StateEnd(){
        if (MainMenuFacade.Instance != null)
            MainMenuFacade.Instance.SceneChangeFlag = SceneChangeFlag.none;
    }
    public virtual void StateUpdate() {
        //Debug.Log("SceneName:" + SceneName);
        
    }

    public void SwitchScene()
    {
        if(MainMenuFacade.Instance != null)
        {
            var facade = MainMenuFacade.Instance;

            switch(facade.SceneChangeFlag)
            {
                case SceneChangeFlag.none:

                    break;
                case SceneChangeFlag.MainMenuScene:
                    mController.SetState(new MainMenuState(mController));
                    break;
                case SceneChangeFlag.MusicSelect:
                    mController.SetState(new MusicSelectState(mController));
                    break;
                case SceneChangeFlag.RhythmScene:
                    mController.SetState(new RhythmState(mController));
                    break;
                case SceneChangeFlag.NoteEditor:
                    mController.SetState(new NoteEditorState(mController));
                    break;
                case SceneChangeFlag.CharaSelect:
                    mController.SetState(new CharaSelectState(mController));
                    break;
            }
        }
    }

}
public enum SceneChangeFlag
{
    none,
    MainMenuScene,
    StoryScene,
    MusicSelect,
    RhythmScene,
    NoteEditor,
    MusicTrackMakerScene,
    StoryMakerScene,
    StoryEditorScene,
    CharaSelect
}