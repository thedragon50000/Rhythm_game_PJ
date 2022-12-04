using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSelectState : ISceneState
{
    public MusicSelectState(SceneStateController controller) : base("MusicSelectScene", controller)
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
        if (facade.SceneChangeFlag == SceneChangeFlag.MainMenuScene)
        {
            mController.SetState(new MainMenuState(mController));
        }
        else if(facade.SceneChangeFlag == SceneChangeFlag.RhythmScene)
        {
            
            mController.SetState(new RhythmState(mController));
        }
    }
}
