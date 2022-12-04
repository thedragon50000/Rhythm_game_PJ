using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEditorState : ISceneState
{
    public NoteEditorState(SceneStateController controller) : base("NoteEditorScene", controller)
    {

    }
    public override void StateStart()
    {
        base.StateStart();
    }

    public override void StateEnd()
    {
        base.StateEnd();

    }
    public override void StateUpdate()
    {
        var facade = MainMenuFacade.Instance;
        base.StateUpdate();
        //Debug.Log("noteEditorScene");

        if (facade.SceneChangeFlag == SceneChangeFlag.MainMenuScene)
        {
            mController.SetState(new MainMenuState(mController));
        }
        else if (facade.SceneChangeFlag == SceneChangeFlag.RhythmScene)
        {
            mController.SetState(new RhythmState(mController));
        }
        else if (facade.SceneChangeFlag == SceneChangeFlag.NoteEditor)
        {
            mController.SetState(new NoteEditorState(mController));
        }
    }
}
