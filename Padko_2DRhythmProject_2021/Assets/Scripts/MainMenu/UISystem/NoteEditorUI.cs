using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class NoteEditorUI : IBaseUISystem
{
    public GameObject defaultUI;
    public GameObject selectUI;
    public Button noteEditorButton;
    public Button tutorialLinkButton;
    public Button shareLinkButton;
    public Button backButton;
    public override void OpenUI()
    {
        defaultUI.SetActive(false);
        selectUI.SetActive(true);

       
    }
    public void Start()
    {
        noteEditorButton.OnClickAsObservable().Subscribe(_ =>
        {
            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.NoteEditor;
        });

        tutorialLinkButton.OnClickAsObservable().Subscribe(_ =>
        {
            Application.OpenURL(StartGameUI.GetTutorialLink());
        });

        shareLinkButton.OnClickAsObservable().Subscribe(_ =>
        {
            Application.OpenURL(StartGameUI.GetCloudLink());
        });

        backButton.OnClickAsObservable().Subscribe(_ =>
        {
            defaultUI.SetActive(true);
            selectUI.SetActive(false);
        });
    }


}
