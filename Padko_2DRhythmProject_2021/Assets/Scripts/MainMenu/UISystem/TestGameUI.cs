using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestGameUI : IBaseUISystem
{
    public Button makeMusicTrackButton;
    public Button makeStoryButton;

    public override void Init()
    {
        base.Init();
        makeMusicTrackButton.onClick.AddListener(delegate { TestGameSwitch(SceneChangeFlag.MusicTrackMakerScene); });
        makeStoryButton.onClick.AddListener(delegate { TestGameSwitch(SceneChangeFlag.StoryMakerScene); });
    }
    public void TestGameSwitch(SceneChangeFlag value)
    {
        MainMenuFacade.Instance.SceneChangeFlag = value;
    }
}
