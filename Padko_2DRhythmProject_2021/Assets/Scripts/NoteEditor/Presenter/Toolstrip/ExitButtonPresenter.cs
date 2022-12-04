using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class ExitButtonPresenter : MonoBehaviour
{
    public Button exitButton;
    // Start is called before the first frame update
    void Start()
    {
        exitButton.OnClickAsObservable().Subscribe(_ =>
        {

            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.MainMenuScene;


        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
