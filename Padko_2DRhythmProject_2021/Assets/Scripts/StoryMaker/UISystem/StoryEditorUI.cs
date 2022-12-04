using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryEditorUI : IBaseUISystem
{
    [SerializeField]
    protected Button mCreateStoryButton;
    [SerializeField]
    protected Button mImportStoryButton;
    [SerializeField]
    protected Button mExportStoryButton;
    [SerializeField]
    protected Button mRemoveStoryButton;
    [SerializeField]
    public Transform mContent;




    public override void Init()
    {
        base.Init();
        mCreateStoryButton.onClick.AddListener(CreateStory);

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
        base.OpenUI();
        var facade = StoryMakerFacade.Instance;

        //facade.SceneChangeFlag = SceneChangeFlag.StoryEditorScene;
        ShowUI(false);

    }

    public override void CloseUI()
    {
        base.CloseUI();
        ShowUI(true);
    }


    public void CreateStory()
    {
        var localFactory = FactoryManager.Instance.localAssetFactory;
        IAssetFactory.OnClickDelegate onClick = delegate { EnterStoryEditor(); };

        localFactory.CreateStoryButton(mContent,"新增故事","LoadingPadko", onClick);




    }
    public void EnterStoryEditor()
    {
        StoryMakerFacade.Instance.SceneChangeFlag = SceneChangeFlag.StoryEditorScene;
    }


}
