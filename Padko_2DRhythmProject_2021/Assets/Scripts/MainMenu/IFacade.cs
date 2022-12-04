using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFacade<T> where T : class,new()
{
    
    protected GameObject mUISystemObj;
    protected GameObject mSystemObj;
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
    public SceneChangeFlag SceneChangeFlag
    {
        set
        {
            if(MainMenuFacade.Instance != null)
                MainMenuFacade.Instance.sceneChangeFlag = value;
        }
        get
        {
            if (MainMenuFacade.Instance != null)
                return MainMenuFacade.Instance.sceneChangeFlag;
            else
            {
                Debug.LogWarning("未獲取");
                return 0;
            }
                
        }
    }
    public virtual void Init()
    {
        #region UISystem Init
        mUISystemObj = GameObject.FindGameObjectWithTag("UISystem");
        #endregion
        #region GameSystem Init
        mSystemObj = GameObject.FindGameObjectWithTag("GameSystem");
        #endregion
    }
    public virtual void Update()
    {

    }
    public virtual void Release()
    {

    }


    
    public void UISystemRun(SystemStateFlag state, params IBaseUISystem[] uiSystems)
    {
        foreach(var item in uiSystems)
        {
            if (item == null)
                Debug.LogWarning(nameof(item) + " 該腳本未設置在UISystem物件或設錯了");
            if(state == SystemStateFlag.Start)
                item.Init();
            else if(state == SystemStateFlag.Update)
                item.UIUpdate();
            else if(state == SystemStateFlag.Release)
                item.Release();
        }
    }
    public void GameSystemRun(SystemStateFlag state, params IBaseGameSystem[] uiSystems)
    {
        foreach (var item in uiSystems)
        {
            if (state == SystemStateFlag.Start)
                item.Init();
            else if (state == SystemStateFlag.Update)
                item.GameUpdate();
            else if (state == SystemStateFlag.Release)
                item.Release();
        }
    }
}

public enum SystemStateFlag
{
    Awake,
    Start,
    Update,
    Release

}