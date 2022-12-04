using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : ISceneState
{
    public MainMenuState(SceneStateController controller) : base("MainMenuScene", controller)
    {
        
    }
    public override void StateStart()
    {
        base.StateStart();
        
        var facade = MainMenuFacade.Instance;
        facade.Init();
    }

    public override void StateEnd()
    {
        base.StateEnd();
        MainMenuFacade.Instance.Release();
        
    }
    public override void StateUpdate()
    {
        var facade = MainMenuFacade.Instance;
        base.StateUpdate();
        SwitchScene();
        facade.Update();
    }
}
