using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RayControllerSystem : IBaseGameSystem
{
    public GameObject rayButtonDownObj;
    public int startIndex;
    public int index;
    public int moveIndex;
    public bool charaButtonDown;
    public bool attrButtonDown;

    public override void Init()
    {
        base.Init();
    }
    public override void GameUpdate()
    {
        base.GameUpdate();
        UIDrag("CharaInputField",ref charaButtonDown, CommandUIType.chara);
        UIDrag("AttrInputField",ref attrButtonDown, CommandUIType.attr);
    }
    public override void Release()
    {
        base.Release();
    }
    public void UIDrag(string targetTag, ref bool buttonDown , CommandUIType commandType)
    {

        var facade = StoryEditorFacade.Instance;
        if (Input.GetMouseButtonDown(0))
        {
            var rayObj = GetCurrentSelect(targetTag);
            if (rayObj == null)
                return;
            if (rayObj.CompareTag(targetTag))//&& obj.transform.parent == facade.definitUI.charasListTrans)
            {
                startIndex = rayObj.transform.GetSiblingIndex();
                index = startIndex;
                rayButtonDownObj = rayObj;
                buttonDown = true;
            }

        }
        if (buttonDown)
        {
            //當對某按鈕按著左鍵拖曳時，射線射到另外一個按鈕則進行交換位置的動作
            var rayObj = GetCurrentSelect(targetTag);
            if (rayObj == null)
                return;
            if (rayObj.CompareTag(targetTag)) //&& obj.transform.parent == facade.definitUI.charasListTrans)
            {
                moveIndex = rayObj.transform.GetSiblingIndex();
                if (index != moveIndex)
                {
                    ExchangeObjPos(rayObj, rayButtonDownObj);
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && buttonDown )
        {
            buttonDown = false;
            if(startIndex != moveIndex)
                facade.commandSystem.Move(commandType, rayButtonDownObj, startIndex, moveIndex);
        }
    }


    //交換兩個物體的階層位置
    public void ExchangeObjPos(GameObject obj, GameObject exchangeObj)
    {
        int rayIndex = obj.transform.GetSiblingIndex();
        int exchangeObjIndex = exchangeObj.transform.GetSiblingIndex();
        rayButtonDownObj.transform.SetSiblingIndex(rayIndex);
        obj.transform.SetSiblingIndex(exchangeObjIndex);
        index = moveIndex;
    }

    //用索引來換位
    public void ExchangeObjIndex(GameObject obj, int index, int moveIndex)
    {
        //int objIndex = obj.transform.GetSiblingIndex();
        //obj.transform.SetSiblingIndex(moveIndex);
    }


    /// <summary>
    /// 獲得當前點擊到的UI物體
    /// </summary>
    public GameObject GetCurrentSelect(string tag)
    {
        GameObject obj = null;

        GraphicRaycaster[] graphicRaycasters = FindObjectsOfType<GraphicRaycaster>();

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();

        if (graphicRaycasters == null || graphicRaycasters.Length == 0)
            return null;

        foreach (var item in graphicRaycasters)
        {
            item.Raycast(eventData, list);
            for (int i = 0; i < list.Count; i++)
            {
                obj = list[i].gameObject;
                if (obj.gameObject.CompareTag(tag))
                {
                    return obj;
                }
            }
        }
        Debug.LogWarning(tag + " not found");
        return obj;
    }

}
