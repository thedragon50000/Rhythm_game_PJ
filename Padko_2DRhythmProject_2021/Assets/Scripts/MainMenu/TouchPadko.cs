using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPadko : MonoBehaviour
{
    public int click;
    // Start is called before the first frame update

    public void OnMouseDown()
    {
        var facade = MainMenuFacade.Instance;
        if (facade.isMenuOpen)
            return;
        click++;
        if (click >= 2)
        {
            if (!facade.gameSettingsAttr.kusoMode)
                facade.gameSettingsAttr.kusoMode = true;
            else
                facade.gameSettingsAttr.kusoMode = false;
            click = 0;
        }
    }

}
