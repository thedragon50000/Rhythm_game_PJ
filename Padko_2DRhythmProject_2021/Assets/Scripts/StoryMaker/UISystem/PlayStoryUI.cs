using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStoryUI : IBaseUISystem
{
    public Transform SpawnButtonTrans;
    public override void Init()
    {
        base.Init();
    }
    public override void UIUpdate()
    {
        base.UIUpdate();
    }
    public override void Release()
    {
        base.Release();
    }


    public override void OpenUI()
    {
        ShowUI(false);
        var localFactory = FactoryManager.Instance.localAssetFactory;
        IAssetFactory.OnClickDelegate onClick = delegate { ShowUI(true); };
        mSpawnObjs.Add(localFactory.CreateButton(SpawnButtonTrans, "本地遊玩"));
        mSpawnObjs.Add(localFactory.CreateButton(SpawnButtonTrans, "社群瀏覽"));
        mSpawnObjs.Add(localFactory.CreateButton(SpawnButtonTrans, "返回", onClick));
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }


}
