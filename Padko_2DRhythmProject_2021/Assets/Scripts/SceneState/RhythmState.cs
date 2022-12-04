using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmState : ISceneState
{
    public RhythmState(SceneStateController controller) : base("RhythmScene", controller)
    {
    }

    public override void StateAwake()
    {
        base.StateAwake();
        
    }
    public override void StateStart()
    {
        base.StateStart();
        RhythmFacade.Instance.Init();
    }



    public override void StateEnd()
    {
        base.StateEnd();
        RhythmFacade.Instance.Release();
    }
    public override void StateUpdate()
    {
        var facade = MainMenuFacade.Instance;
        base.StateUpdate();

        SwitchScene();
        RhythmFacade.Instance.Update();
    }
}
