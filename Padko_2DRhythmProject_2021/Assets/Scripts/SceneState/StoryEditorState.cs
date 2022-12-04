using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEditorState : ISceneState
{
    public StoryEditorState(SceneStateController controller) : base("StoryEditorScene", controller)
    {

    }
    public override void StateAwake()
    {
        base.StateAwake();
    }

    public override void StateEnd()
    {
        base.StateEnd();
        StoryEditorFacade.Instance.Release();
    }

    public override void StateStart()
    {
        base.StateStart();
        StoryEditorFacade.Instance.Init();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        StoryEditorFacade.Instance.Update();
    }
}
