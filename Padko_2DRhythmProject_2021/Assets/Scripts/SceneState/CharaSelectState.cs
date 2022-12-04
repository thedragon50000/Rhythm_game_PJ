using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaSelectState : ISceneState
{
    public CharaSelectState(SceneStateController controller) : base("CharaSelectScene", controller)
    {

    }
    public override void StateStart()
    {
        base.StateStart();
    }

    public override void StateEnd()
    {
        base.StateEnd();
        MainMenuFacade.Instance.Release();
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
        SwitchScene();
    }
}
