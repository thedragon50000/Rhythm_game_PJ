// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using NoteEditor.Utility;
//
// namespace Game.Process
// {
//     /// <summary>
//     /// todo:音符的軌道，沒用到的腳本
//     /// </summary>
//     public class LaneController : SingletonMonoBehaviour<LaneController>
//     {
//         public GameObject preLane;
//
//         /// <summary>
//         /// Lane的父物件
//         /// </summary>
//         public Transform transformLaneParent;
//
//         private readonly List<GameObject> listLane = new();
//
//         private readonly List<Image> listImage = new();
//
//         private float width;
//
//         //public KeyCode KEY0 = KeyCode.A, KEY1 = KeyCode.S, KEY2 = KeyCode.D, KEY3 = KeyCode.F, KEY4 = KeyCode.G;
//         //private KeyCode[] keys;
//
//         private bool isPlaying = false;
//
//         void Awake()
//         {
//             if (transformLaneParent == null)
//             {
//                 Debug.LogError("Lane component lost!");
//             }
//
//             width = transformLaneParent.GetComponent<RectTransform>().sizeDelta.x;
//         }
//
//         public void CreateLanes(int laneCount)
//         {
//             //keys = new KeyCode[] { KEY0, KEY1, KEY2, KEY3, KEY4 };
//
//             if (laneCount < 1 || laneCount > 4 + (int) NotesController.Instance.keyType)
//             {
//                 Debug.LogError("Incorrect lane number!");
//                 return;
//             }
//
//             // if(listLane.Count == laneCount)
//             // {
//             //
//             // }
//             for (int i = 0; i < laneCount; i++)
//             {
//                 GameObject obj = Instantiate(preLane, transformLaneParent);
//
//                 obj.name = "lane " + i;
//
//                 //根據Enum來決定按鍵
//                 obj.GetComponentInChildren<Text>().text = PlayerSettings.Instance.GetKeyCode(i).ToString();
//
//                 listLane.Add(obj);
//
//                 Image img = obj.transform.Find("ShowClick").GetComponent<Image>();
//                 listImage.Add(img);
//             }
//             
//             isPlaying = true;
//         }
//
//         void Update()
//         {
//             if (isPlaying)
//             {
//                 for (int i = 0; i < listLane.Count; i++)
//                 {
//                     if (Input.GetKey(PlayerSettings.Instance.GetKeyCode(i)))
//                     {
//                         listImage[i].gameObject.SetActive(true);
//                     }
//                     else
//                     {
//                         listImage[i].gameObject.SetActive(false);
//                     }
//                 }
//             }
//         }
//
//         /// <summary>
//         /// todo: Get Position.X or what?
//         /// </summary>
//         /// <param name="index"></param>
//         /// <returns></returns>
//         public float GetLaneX(int index)
//         {
//             if (index < 0 || index >= listLane.Count)
//             {
//                 print($"Wrong index: {index}");
//                 return -1;
//             }
//             else
//             {
//                 //float x1 = laneList[index].GetComponent<RectTransform>().anchoredPosition.x;
//                 //float x2 = laneParent.GetComponent<RectTransform>().anchoredPosition.x;
//
//                 //Debug.Log(x1 + "-" + x2);
//
//                 float delta = width / listLane.Count;
//                 //float center = width/2;
//
//
//                 return delta * (0.5f + index) - width / 2;
//             }
//         }
//     }
// }