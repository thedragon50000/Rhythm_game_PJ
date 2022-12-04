using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : IBaseUISystem
{
    public override void OpenUI()
    {
        ExitGame();
        
    }
    public override void CloseUI()
    {
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("exit");
    }
}
