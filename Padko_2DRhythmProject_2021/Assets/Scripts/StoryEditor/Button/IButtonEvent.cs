using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class IButtonEvent : MonoBehaviour
, IPointerExitHandler
, IPointerDownHandler
, IPointerUpHandler
, IEventSystemHandler
{
    private bool DownState = false;

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Exit");
        DownState = false;
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Down");
        DownState = true;
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        DownState = false;
    }
    private float timer = 0;
    //void Update()
    //{
    //    //if (DownState)
    //    //    timer += Time.deltaTime;
    //    //else
    //    //    timer = 0;
    //}
}