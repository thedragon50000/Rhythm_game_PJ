using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEditorFacade : IFacade<StoryEditorFacade>
{
    public DefinitUI definitUI;
    public DefinitCommandSystemHandler commandSystem;
    public RayControllerSystem rayControllerSystem;

    public override void Init()
    {
        base.Init();
        commandSystem = new DefinitCommandSystemHandler();
        commandSystem.Start();

        #region UISystem Init
        definitUI = UITool.FindChild<DefinitUI>(mUISystemObj, nameof(DefinitUI));

        UISystemRun(SystemStateFlag.Start, definitUI);
        #endregion
        #region GameSystem Init

        rayControllerSystem = UITool.FindChild<RayControllerSystem>(mSystemObj, nameof(RayControllerSystem));


        //commandSystem = UITool.FindChild<DefinitCommandSystemHandler>(mSystemObj, nameof(DefinitCommandSystemHandler));
        GameSystemRun(SystemStateFlag.Start, rayControllerSystem);


        #endregion


    }

    public override void Release()
    {
        base.Release();
    }

    public override void Update()
    {
        base.Update();
        GameSystemRun(SystemStateFlag.Update, rayControllerSystem);
        commandSystem.Update();
    }
}
