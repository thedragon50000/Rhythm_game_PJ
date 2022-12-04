/// <summary>
/// The 'Receiver' class - this handles what a move command actually does
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommandReceiver
{
    #region Add
    public void AddOperation(AddCommand command, bool isInverse = false)
    {
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;


        switch (command.commandType)
        {
            case CommandUIType.chara:
                var charasListTrans = facade.definitUI.charasListTrans;
                var charasListObjDic = facade.definitUI.charasListObjDic;
                var charaListInput = asset.inputField.charaListInput;

                Add(name: "新建角色", commandObjName: ref command.objName, createParent: charasListTrans,
                    commandParent: ref command.parent, createObj: charaListInput, dic: definit.charas,
                    objDic: charasListObjDic, commandType: command.commandType, isInverse: isInverse);
                break;
            case CommandUIType.img:
                //Add(command, isInverse);
                break;
            case CommandUIType.attr:
                  var selectCharaName = facade.definitUI.selectCharaName;
                if (selectCharaName.Length > 0)
                {
                    var attributeTrans = facade.definitUI.attributeTrans;
                    var attributeUIDic = facade.definitUI.attributeObjDic;
                    var charaAttrInput = asset.inputField.charaAttrInput;
                  
                    var stringAttr = definit.charas[selectCharaName].stringAttributes;

                    //Debug.Log("selectCharaName:"+ selectCharaName);
                    Add(name: "新建屬性", commandObjName: ref command.objName, createParent: attributeTrans,
                    commandParent: ref command.parent, createObj: charaAttrInput, dic: stringAttr,
                    objDic: attributeUIDic, commandType: command.commandType, isInverse: isInverse);
                }
                //Add(command, isInverse);
                break;
            case CommandUIType.item:
                //Add(command, isInverse);
                break;
        }
    }
    public GameObject Add<T>(string name , ref string commandObjName, Transform createParent, ref Transform commandParent, 
        GameObject createObj, SerializableDictionary<string, T> dic, SerializableDictionary<string, GameObject> objDic,
        CommandUIType commandType = CommandUIType.none, bool isInverse = false) 
    {
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;
        var charasListTrans = facade.definitUI.charasListTrans;

        string createdName = null;
        GameObject createdObject = null;
        if (!isInverse)
        {
            commandParent = createParent;
            if (commandObjName == null )
            {
                commandObjName = name;
            }
            if (dic.ContainsKey(commandObjName))
            {
                commandObjName = UnityTool.DontReapeatKey(dic, commandObjName);
            }
            
            switch (commandType)
            {
                case CommandUIType.chara:
                    dic[commandObjName] = (T)(object)new StoryChara();
                    break;
                case CommandUIType.img:

                    break;
                case CommandUIType.attr:
                    dic[commandObjName] = (T)(object)(name + "值");
                    createdName = commandObjName + ":" + "值";
                    break;
                case CommandUIType.item:

                    break;

            }

            if (objDic.ContainsKey(commandObjName))
            {
                objDic[commandObjName].SetActive(true);
                createdObject = objDic[commandObjName];
            }
            else
            {
                createdObject = facade.definitUI.CreateStoryAttr(commandType, commandParent, commandObjName, createObj, createdName: createdName);
            }
        }
        else
        {
            dic.Remove(commandObjName);
            facade.definitUI.RemoveUIElements(objDic, commandObjName);
        }
        return createdObject;
    }



    #endregion

    #region Select
    public void SelectOperation(SelectCommand command, bool isInverse = false)
    {
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;


        SerializableDictionary<string, GameObject> UIObjDic = facade.definitUI.charasListObjDic;
        string key = null;

        switch (command.commandType)
        {
            case CommandUIType.chara:
                //Debug.Log("key:" + key + ",selectCharaName:" + facade.definitUI.selectCharaName);
                UIObjDic = facade.definitUI.charasListObjDic;
                key = UnityTool.KeyByValue(UIObjDic, command._gameObject);
                LoadAttrUI(command, key, isInverse);
                command.selectName = facade.definitUI.selectCharaName;
                if (!isInverse)
                {
                    //選取左側角色清單中的一個角色
                    
                    facade.definitUI.SetSelect(command.commandType, facade.definitUI.charasListObjDic, key, false, previousObjName: command.selectName);
                    command.resetName = key;
                }
                else
                {
                    facade.definitUI.SetSelect(command.commandType, facade.definitUI.charasListObjDic, command.resetName, false, previousObjName: command.selectName);
                }
                break;
            case CommandUIType.img:

                break;
            case CommandUIType.attr:
                UIObjDic = facade.definitUI.attributeObjDic;
                key = UnityTool.KeyByValue(UIObjDic, command._gameObject);
                command.selectName = facade.definitUI.selectAttrName;
                if (!isInverse)
                {
                    //選取右側的一個屬性
                    Debug.Log("Attr");
                    command.resetName = key;
                    facade.definitUI.SetSelect(command.commandType, facade.definitUI.attributeObjDic, key, false, previousObjName: command.selectName);

                }
                else
                {
                    facade.definitUI.SetSelect(command.commandType, facade.definitUI.attributeObjDic, command.resetName, false, previousObjName: command.selectName);
                }

                break;
            case CommandUIType.item:

                break;

        }


    }

    public void LoadAttrUI(SelectCommand command, string key, bool isInverse = false)
    {
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;
        var attrTrans = facade.definitUI.attributeTrans;
        var attrObjDic = facade.definitUI.attributeObjDic;
        var attrDic = definit.charas[key].stringAttributes;
        if(facade.definitUI.selectCharaName.Length>0)
        {
            if (key != facade.definitUI.selectCharaName) //如果選擇了不同的角色的話 則載入屬性
            {
                //獲得所有顯示的子物件
                List<GameObject> childs = UnityTool.GetChildGameObjects(attrTrans);
                //Debug.Log("childsCount:" + childs.Count);
                int count = 0;

                facade.definitUI.SetSelect(CommandUIType.attr, facade.definitUI.attributeObjDic, facade.definitUI.selectAttrName, true, previousObjName: null);
                facade.definitUI.selectAttrName = null;

                //int count
                foreach (var attr in attrDic) 
                {
                    GameObject reviseObj = null;
                
                    //先看物件拿不拿得到 拿得到就顯示 拿不到就修改或新增
                    if (attrObjDic.ContainsKey(attr.Key))
                    {
                        reviseObj = attrObjDic[attr.Key];
                        reviseObj.SetActive(true);
                        childs.Remove(reviseObj);
                        //這樣做的話有可能順序會亂掉 所以拿到後要移出childs裡面
                    }
                    else
                    {
                        //不存在就新增新的InputField在裡面 並進行綁定
                        if(childs.Count>0) // 如果場上顯示的物件數量>0的話 就把該物件進行修改
                        {
                            //childs[count].SetActive(true);
                            reviseObj = childs[count];
                            count++;

                        }
                        else
                        {
                            //物件數量不足，新增一個新的物件用以顯示
                            var attributeTrans = facade.definitUI.attributeTrans;
                            var attributeUIDic = facade.definitUI.attributeObjDic;
                            var charaAttrInput = asset.inputField.charaAttrInput;
                            var selectCharaName = facade.definitUI.selectCharaName;
                            var stringAttr = definit.charas[selectCharaName].stringAttributes;

                            reviseObj = facade.definitUI.CreateStoryAttr(CommandUIType.attr, attributeTrans, attr.Key, charaAttrInput, createdName: attr.Key);
                        }
                    }
                    //不管存不存在都進行修改 將物件修改成scriptableObject的內容
                    InputField reviseInput = reviseObj.GetComponent<InputField>();
                    string[] sArray = reviseInput.text.Split(':');//取分號前 前面命名的名稱作為Key
                    string childKey = sArray[0]; 
                    if (childKey != attr.Key) //物件Key不一致的話 進行修改
                    {
                        reviseInput.text = attr.Key + ":" + attr.Value;
                        attrObjDic[attr.Key] = reviseObj; 
                        //attrObjDic[childKey] = childs[count];
                    }
                }
                if (count < childs.Count)
                {
                    for (int i = count; i < childs.Count; i++)
                    {
                        if (childs[i].activeSelf)
                        {
                            childs[i].SetActive(false);
                        }
                    }
                }

                //facade.commandSystem.commands.Clear();
                //facade.commandSystem.currentCommandNum = 0;

            }
        }
    }

    #endregion


    #region Rename
    public void RenameOperation(RenameCommand command, bool isInverse = false)
    {
        switch (command.commandType)
        {
            case CommandUIType.chara:
                var factory = FactoryManager.Instance.localAssetFactory;
                var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
                var definit = factory.mCreateStoryEditorAsset.definit;
                var facade = StoryEditorFacade.Instance;
                var UIElement = facade.definitUI.charasListObjDic;
                if (command._gameObject == null )
                {
                    if(UIElement.ContainsKey(command.changeName))
                    {
                        command._gameObject = UIElement[command.changeName];
                    }
                }

                string key = UnityTool.KeyByValue(UIElement, command._gameObject);
                command.changeName = command._gameObject.GetComponent<InputField>().text;

                if (!isInverse)
                {

                    if (key == command.changeName)//上次跟這次結果一樣
                    {
                        command.changeName = command.resetName;
                        if(command.resetName == null)
                        {
                            return;
                        }
                            
                    }
                    command.changeName = UnityTool.DontReapeatKey(definit.charas, command.changeName);
                    //if (command.changeName == null)//如果玩家重複輸入一樣的名稱就撤銷該命令
                    //{
                    //    facade.commandSystem.commands.Remove(command);
                    //    facade.commandSystem.currentCommandNum--;
                    //    return;
                    //}
                    definit.charas[command.changeName] = definit.charas[key];
                    //definit.charas.Add(command.changeName, definit.charas[key]);
                    definit.charas.Remove(key);
                    UIElement[command.changeName] = UIElement[key];
                    //UIElement.Add(command.changeName, UIElement[key]);
                    UIElement.Remove(key);
                    facade.definitUI.SetSelect(command.commandType, facade.definitUI.attributeObjDic,command.changeName, false);
                    UIElement[command.changeName].GetComponent<InputField>().text = command.changeName;

                }
                else
                {
                    if (command.resetName == null)
                    {
                        return;
                    }
                    command.resetName = UnityTool.DontReapeatKey(definit.charas, command.resetName);
                    UIElement[key].GetComponent<InputField>().text = command.resetName;
                    definit.charas[command.resetName] = definit.charas[key];
                    //definit.charas.Add(command.resetName, definit.charas[key]);
                    definit.charas.Remove(key);
                    UIElement[command.resetName] = UIElement[key];
                    //UIElement.Add(command.resetName, UIElement[key]);
                    UIElement.Remove(key);
                    facade.definitUI.SetSelect(command.commandType, facade.definitUI.attributeObjDic,command.changeName, true);
                    facade.definitUI.SetSelect(command.commandType, facade.definitUI.attributeObjDic,command.resetName, false);
                    //command.changeName = command.resetName;
                }
                command.resetName = key;
                Debug.Log("resetName:"+command.resetName);
                Debug.Log("changeName:" + command.changeName);
                break;
            case CommandUIType.img:
                //Add(command, isInverse);
                break;
            case CommandUIType.attr:
                //Add(command, isInverse);
                break;
            case CommandUIType.item:
                //Add(command, isInverse);
                break;
        }
    }

    #endregion

    #region Move
    public void MoveOperation(MoveCommand command, bool isInverse = false)
    {
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;
        //var attrTrans = facade.definitUI.attributeTrans;
        //var attrObjDic = facade.definitUI.attributeObjDic;
        //if(facade.definitUI.selectCharaName.Length > 0)
        //{
        //    var attrDic = definit.charas[facade.definitUI.selectCharaName].stringAttributes;
        //}
        
        switch (command.commandType)
        {
            case CommandUIType.chara:
                Move(command, facade.definitUI.charasListTrans, isInverse);
                break;
            case CommandUIType.img:
                //Move(command, isInverse);
                break;
            case CommandUIType.attr:
                Move(command, facade.definitUI.attributeTrans, isInverse);
                break;
            case CommandUIType.item:
                //Move(command, isInverse);
                break;
        }
    }
    public void Move(MoveCommand command, Transform trans, bool isInverse = false)
    {
        var facade = StoryEditorFacade.Instance;

        if(!isInverse)
        {
            var exchangeObj = trans.GetChild(command.moveIndex);
            exchangeObj.transform.SetSiblingIndex(command.startIndex);
            command._gameObject.transform.SetSiblingIndex(command.moveIndex);
        }
        else
        {
            var exchangeObj = trans.GetChild(command.startIndex);
            exchangeObj.transform.SetSiblingIndex(command.moveIndex);
            command._gameObject.transform.SetSiblingIndex(command.startIndex);
        }
    }

    #endregion

    #region Copy
    public void CopyOperation(CopyCommand command, bool isInverse = false)
    {
        GameObject createdObj = null;
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;

        switch (command.commandType)
        {
            case CommandUIType.chara:
                var charasListTrans = facade.definitUI.charasListTrans;
                var charasListObjDic = facade.definitUI.charasListObjDic;
                var charaListInput = asset.inputField.charaListInput;

                int targetIndex = command._gameObject.transform.GetSiblingIndex();
                if (command.copyName == null)
                {
                    command.copyName = UnityTool.KeyByValue(charasListObjDic, command._gameObject);
                }
                createdObj = Add(name: command.copyName, commandObjName: ref command.copyName, createParent: charasListTrans,
                    commandParent: ref command.parent, createObj: charaListInput, dic: definit.charas,
                    objDic: charasListObjDic, commandType: command.commandType, isInverse: isInverse);
                if(!isInverse)
                    createdObj.transform.SetSiblingIndex(targetIndex + 1);

                //Copy(command, isInverse);
                break;
            case CommandUIType.img:
                //Copy(command, isInverse);
                break;
            case CommandUIType.attr:
                //Copy(command, isInverse);
                break;
            case CommandUIType.item:
                //Copy(command, isInverse);
                break;
        }
        


    }
    public void Copy<T>(CopyCommand command, GameObject createdObj, SerializableDictionary<string, T> dic, SerializableDictionary<string, GameObject> objDic, bool isInverse = false)
    {
    

    }


    #endregion

    #region Remove
    public void RemoveOperation(RemoveCommand command, bool isInverse = false)
    {
        var factory = FactoryManager.Instance.localAssetFactory;
        var asset = FactoryManager.Instance.localAssetFactory.mCreateAsset;
        var definit = factory.mCreateStoryEditorAsset.definit;
        var facade = StoryEditorFacade.Instance;
        GameObject createdObj = null;

        switch (command.commandType)
        {
            case CommandUIType.chara:
                var charasListTrans = facade.definitUI.charasListTrans;
                var charasListObjDic = facade.definitUI.charasListObjDic;
                var charaListInput = asset.inputField.charaListInput;
                if (command.removeName == null)
                {
                    command.removeName = UnityTool.KeyByValue(charasListObjDic, command._gameObject);
                }
                createdObj = Add(name: command.removeName, commandObjName: ref command.removeName, createParent: charasListTrans,
                    commandParent: ref command.parent, createObj: charaListInput, dic: definit.charas,
                    objDic: charasListObjDic, commandType: command.commandType, isInverse: !isInverse);

                //Remove(command, isInverse);

                break;
            case CommandUIType.img:
                //Remove(command, isInverse);
                break;
            case CommandUIType.attr:
                //Remove(command, isInverse);
                break;
            case CommandUIType.item:
                //Remove(command, isInverse);
                break;
        }
    }
    public void Remove(RemoveCommand command, bool isInverse = false)
    {
        //var facade = StoryEditorFacade.Instance;
        //if (!isInverse)
        //{
        //    var exchangeObj = facade.definitUI.charasListTrans.GetChild(command.moveIndex);
        //    exchangeObj.transform.SetSiblingIndex(command.startIndex);
        //    command._gameObject.transform.SetSiblingIndex(command.moveIndex);
        //}
        //else
        //{
        //    var exchangeObj = facade.definitUI.charasListTrans.GetChild(command.startIndex);
        //    exchangeObj.transform.SetSiblingIndex(command.moveIndex);
        //    command._gameObject.transform.SetSiblingIndex(command.startIndex);
        //}
    }



    #endregion

    //缺乏功能:
    //匯入圖片 1.從素材庫  2.從檔案 
    //移動圖片 放大縮小圖片

}


