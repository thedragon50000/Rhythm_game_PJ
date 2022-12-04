using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteEditor.Utility;
public class IBaseUISystem : SingletonMonoBehaviour<IBaseUISystem>
{
    

    [SerializeField]
    protected Button mOpenButton;
    [SerializeField]
    protected Button mCloseButton;
    [SerializeField]
    protected GameObject mUIObj;

    [SerializeField]
    protected List<GameObject> mShowObjs;

    protected List<GameObject> mSpawnObjs = new List<GameObject>();

    public virtual void Init() {
        if (mOpenButton != null)
            mOpenButton.onClick.AddListener(OpenUI);
        else
            Debug.LogWarning(name + "系統沒有設置用以打開UI介面的按鈕");
        if (mCloseButton != null)
            mCloseButton.onClick.AddListener(CloseUI);
        else
            Debug.LogWarning(name + "系統沒有設置用以關閉UI介面的按鈕");
    }
    public virtual void UIUpdate() { }
    public virtual void Release() { }

    public virtual void OpenUI()
    {
        if(MainMenuFacade.Instance.isMenuOpen)
        {
            return;
        }
        if(mUIObj != null)
        {
            if(!mUIObj.activeSelf)
            {
                MainMenuFacade.Instance.isMenuOpen = true;
                mUIObj.SetActive(true);
            }
        }
    }
    public virtual void CloseUI()
    {
        MainMenuFacade.Instance.isMenuOpen = false;
        if (mUIObj == null)
            return;
        if (mUIObj.activeSelf)
        {
            mUIObj.SetActive(false);
        }
    }

    public virtual void ShowUI(bool showFlag)
    {
        if (mShowObjs.Count == 0)
            return;
        if(showFlag && mSpawnObjs.Count>0)
        {
            for (int i = mSpawnObjs.Count-1; i >= 0 ; i--)
            {
                Destroy(mSpawnObjs[i]);
            }
        }

        for (int i = 0; i < mShowObjs.Count; i++)
        {
            mShowObjs[i].SetActive(showFlag);
        }
    }

}
