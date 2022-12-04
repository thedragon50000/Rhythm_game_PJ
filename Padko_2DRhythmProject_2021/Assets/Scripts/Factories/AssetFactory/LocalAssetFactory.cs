
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Newtonsoft.Json;
public class LocalAssetFactory : IAssetFactory
{
    
    public GameObject CreateButton(Transform parent, string name, OnClickDelegate click = null , GameObject create = null)
    {
        var btn = mCreateAsset.button;
        GameObject obj = null;
        Button button = null;
        if (create == null)
        {
            obj = btn.menuButton;
        }
        else
        {
            obj = create;
        }
        
        obj = SpawnGameObject(parent: parent, obj: obj);
        if(create == btn.addButton)
        {
            button = obj.GetComponent<Button>();
        }
        else
        {
            button = UITool.FindChild<Button>(obj, "Button", isContainSelf: true);
        }
        
        if (click != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { click(); });
        }
        var buttonText = UITool.FindChild<Text>(obj, "ButtonText");
        buttonText.text = name;
        return obj;
    }
    /*
    public GameObject CreateButton(Transform parent, string name)
    {
        var btn = mCreateAsset.button;
        GameObject obj = btn.menuButton;
        obj = SpawnGameObject(parent, obj);

        var button = UITool.FindChild<Button>(obj, "Button");
        button.onClick.RemoveAllListeners();
        var buttonText = UITool.FindChild<Text>(obj, "ButtonText");
        buttonText.text = name;

        return obj;
    }*/

    public GameObject CreateInputField(GameObject inputField, Transform parent, string keyName, 
        OnClickDelegate onButtonClick = null, OnClickDelegate onEndEdit = null, DefinitCommandSystemHandler commandSystem = null,
        CommandUIType commandType = CommandUIType.none, string createdName = null)
    {
        GameObject obj = inputField;
        obj = SpawnGameObject(parent: parent, obj: obj);
        InputField input = null;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        input = obj.GetComponent<InputField>();
        Button button = null;
        button = UITool.FindChild<Button>(obj, "Button");
        //if (onButtonClick != null)
        //{
        //    button.onClick.RemoveAllListeners();
        //    button.onClick.AddListener(delegate { onButtonClick(); });
        //}
        //if (onEndEdit != null)
        //{
        //    input.onEndEdit.RemoveAllListeners();
        //    input.onEndEdit.AddListener(delegate { onEndEdit(); });
        //}
        switch (commandType)
        {
            case CommandUIType.chara:
                if (commandSystem != null)
                {
                    onButtonClick = delegate { commandSystem.Select(commandType, obj, keyName); };
                    onEndEdit = delegate { commandSystem.Rename(commandType, obj, keyName); };
                    button.onClick.AddListener(delegate { onButtonClick(); });
                    input.onEndEdit.AddListener(delegate { onEndEdit(); });
                    //input.onSubmit.AddListener(delegate { onEndEdit(); });
                }
                break;
            case CommandUIType.img:

                break;
            case CommandUIType.attr:
                if (commandSystem != null)
                {
                    onButtonClick = delegate { commandSystem.Select(commandType, obj, keyName); };
                    onEndEdit = delegate { commandSystem.Rename(commandType, obj, keyName); };
                    button.onClick.AddListener(delegate { onButtonClick(); });
                    input.onEndEdit.AddListener(delegate { onEndEdit(); });
                }
                break;
            case CommandUIType.item:

                break;

        }
        //input.on.RemoveAllListeners();
        var inputText = obj.GetComponent<InputField>();
        if(createdName == null)
            inputText.text = keyName;
        else
            inputText.text = createdName;

        return obj;
    }
    public GameObject CreateStoryButton(Transform parent, string name, string imageName, OnClickDelegate click)
    {
        var btn = mCreateAsset.button;
        GameObject obj = btn.storyButton;
        obj = SpawnGameObject(parent:parent,obj: obj);
        Button button = null;
        Image image = null;
        Text buttonText = null;
        button = obj.GetComponent<Button>();
        image = obj.GetComponent<Image>();
        buttonText = UITool.FindChild<Text>(obj, "Text");
        buttonText.text = name;
        image.sprite = mCreateAsset.image.storyButtonImage["LoadingPadko"];
        button.onClick.RemoveAllListeners();
        if(click != null)
            button.onClick.AddListener(delegate { click(); });
        return obj;
    }
    
    //public T SaveJson<T>(T t, string name)
    //{
    //    var jsonPath = mSavePath + "/" + name;
    //    //如果本地没有对应的json 文件，重新创建
    //    if (!File.Exists(jsonPath))
    //    {
    //        File.Create(jsonPath);
    //    }
    //    string json = JsonConvert.SerializeObject(t);
    //    //string json = JsonUtility.ToJson(t, true);
    //    File.WriteAllText(jsonPath, json);
    //    Debug.Log("保存成功");

    //    return default(T);
    //}

    ////从本地读取json数据
    //public T ReadJson<T>(string name)
    //{
    //    var jsonPath = mSavePath + "/" + name;
    //    if (!File.Exists(jsonPath))
    //    {
    //        Debug.LogError("讀取的文件不存在！");
    //        return default(T);
    //    }

    //    string json = File.ReadAllText(jsonPath);
    //    //var dayrangeMessagetemp = JsonUtility.FromJson<T>(json);

    //    //T data = JsonUtility.FromJson<T>(json);
    //    T data = JsonConvert.DeserializeObject<T>(json);
    //    Debug.Log("讀取成功");
    //    return data;
    //}
}
