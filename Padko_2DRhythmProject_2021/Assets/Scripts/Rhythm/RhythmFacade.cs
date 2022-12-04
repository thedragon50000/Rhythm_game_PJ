using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Process;

public class RhythmFacade : IFacade<RhythmFacade>
{
    public override void Init()
    {
    }

    public void ExitScene(SceneChangeFlag sceneChange = default)
    {

        Debug.Log("ExitScene");
        MusicController.Instance.PauseMusic(false);
        if (!NotesController.Instance.isEditMode)
        {
            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.MusicSelect;
        }
        else
        {
            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.NoteEditor;
        }

    }


}
