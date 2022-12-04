using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DefinitUI : IBaseUISystem
{
    public Transform charaImgsTrans;
    public Transform attributeTrans;
    public Transform charasListTrans;
    public Transform itemsListTrans;

    public GameObject attrWindowsObj;

    public Button addCharasListButton;
    public Button addAttributeButton;


    public SerializableDictionary<string, GameObject> charasListObjDic;
    public SerializableDictionary<string, GameObject> attributeObjDic;
    public string selectCharaName;
    public string selectAttrName;

    public List<string> commandString_debug;


    public override void Init()
    {
        base.Init();
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;

        addCharasListButton.onClick.AddListener(
           delegate { 
               facade.commandSystem.Add(CommandUIType.chara, asset.inputField.charaListInput); 
            });
        addAttributeButton.onClick.AddListener(
           delegate {
               facade.commandSystem.Add(CommandUIType.attr, asset.inputField.charaAttrInput);
           });
        InitScriptable();
            
        if(selectCharaName == null || selectCharaName.Length==0)
        {
            attrWindowsObj.SetActive(false);
        }
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

    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    public override void ShowUI(bool showFlag)
    {
        base.ShowUI(showFlag);
    }
    public void InitScriptable()
    {
        var definit = FactoryManager.Instance.localAssetFactory.mCreateStoryEditorAsset.definit;
        definit.charas.Clear();
        definit.items.Clear();
    }


    public GameObject CreateStoryAttr(CommandUIType commandType, Transform parent, string keyName, GameObject create = null, string createdName = null)
    {
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;
        IAssetFactory.OnClickDelegate onClick = null;
        GameObject obj = null;

        obj = factory.CreateInputField(inputField: create, parent: parent, keyName: keyName, commandSystem: facade.commandSystem,
              commandType: commandType, createdName: createdName);
        switch (commandType)
        {
            case CommandUIType.chara:
                charasListObjDic.Add(keyName, obj);
                //addCharasListButton.transform.SetSiblingIndex(charasListTrans.childCount);
                break;
            case CommandUIType.img:

                break;
            case CommandUIType.attr:
                attributeObjDic.Add(keyName, obj);
                //addAttributeButton.transform.SetSiblingIndex(attributeTrans.childCount);

                break;
            case CommandUIType.item:

                break;

        }
        return obj;
    }
    public void RemoveUIElements(SerializableDictionary<string,GameObject> dic ,string name)
    {
        var facade = StoryEditorFacade.Instance;
        dic[name].SetActive(false);
        Debug.Log("Destory:" + name);
    }
    public void SetSelect(CommandUIType commandType, SerializableDictionary<string, GameObject> objDic, 
        string value, bool flag, string previousObjName = null)
    {

        string name = null;
        switch (commandType)
        {
            case CommandUIType.chara:
                selectCharaName = value;
                name = selectCharaName;
                break;
            case CommandUIType.img:

                break;
            case CommandUIType.attr:
                selectAttrName = value;
                name = selectAttrName;
                break;
            case CommandUIType.item:

                break;

        }
        Button button = null;
        //foreach (var element in objDic)
        //{
        //    button = UITool.FindChild<Button>(element.Value, "Button");
        //    if (!button.gameObject.activeSelf)
        //    {
        //        button.gameObject.SetActive(true);
        //    }
        //}
        bool isPreviousObjExist = true;
        if (previousObjName != null)
        {
            if (previousObjName.Length == 0 || !objDic.ContainsKey(previousObjName))
            {
                isPreviousObjExist = false;
            }
        }
        else
        {
            isPreviousObjExist = false;
        }
        if (isPreviousObjExist)
        {
            button = UITool.FindChild<Button>(objDic[previousObjName], "Button");
            button.gameObject.SetActive(true);
        }


        if (name != null && objDic.ContainsKey(name))
        {
            UITool.FindChild<Button>(objDic[name], "Button").gameObject.SetActive(flag);
            if(commandType == CommandUIType.chara)
                attrWindowsObj.SetActive(true);
        }
        else
        {
            if (commandType == CommandUIType.chara)
                attrWindowsObj.SetActive(false);
        }

           
    }
    public void SetAllButton(CommandUIType commandType, SerializableDictionary<string, GameObject> objDic,
        string value, bool flag)
    {
        Button button = null;
        foreach (var element in objDic)
        {
            button = UITool.FindChild<Button>(element.Value, "Button");
            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(flag);
            }
        }
    }




    public void RenameUIElements(SerializableDictionary<string, GameObject> dic, string name)
    {
    }


}
