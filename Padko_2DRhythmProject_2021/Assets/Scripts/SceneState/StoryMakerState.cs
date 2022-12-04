using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StoryMakerState : ISceneState
{
    public StoryMakerState(SceneStateController controller) : base("StoryMakerScene", controller)
    {

    }

    public override void StateAwake()
    {
        base.StateAwake();
        
    }



    public override void StateStart()
    {
        base.StateStart();
        StoryMakerFacade.Instance.Init();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        var facade = StoryMakerFacade.Instance;
        facade.Update();
        switch(facade.SceneChangeFlag)
        {
            case SceneChangeFlag.StoryScene:
                mController.SetState(new StoryState(mController));
                break;
            case SceneChangeFlag.StoryEditorScene:
                mController.SetState(new StoryEditorState(mController));
                break;
        }
    }
    public override void StateEnd()
    {
        base.StateEnd();
        var facade = StoryMakerFacade.Instance;
        facade.SceneChangeFlag = SceneChangeFlag.none;
    }
}
