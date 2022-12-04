using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    private SceneStateController controller = null;
    public static bool isClone = false;

    //todo:專門為了Scene轉換服務的，用Zenject替代
    // Start is called before the first frame update
    void Awake()
    {
        if(!isClone)
        {
            DontDestroyOnLoad(gameObject);
            controller = new SceneStateController();
            controller.SetState(new MainMenuState(controller), false);
            isClone = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        controller.StateUpdate();
    }
}
