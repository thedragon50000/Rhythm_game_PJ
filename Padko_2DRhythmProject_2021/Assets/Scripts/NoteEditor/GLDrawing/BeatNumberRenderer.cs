using NoteEditor.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NoteEditor.GLDrawing
{
    public class BeatNumberRenderer : SingletonMonoBehaviour<BeatNumberRenderer>
    {
        [SerializeField]
        GameObject beatNumberPrefab = default;

        List<RectTransform> rectTransformPool = new List<RectTransform>();
        List<Text> textPool = new List<Text>();

        int size;
        int countPrevActive = 0;
        int countCurrentActive = 0;

        static public void Render(Vector3 pos, int number)
        {
            if (Instance.countCurrentActive < Instance.size)
            {
                if (Instance.countCurrentActive >= Instance.countPrevActive)
                {
                    Instance.textPool[Instance.countCurrentActive].gameObject.SetActive(true);
                }

                Instance.rectTransformPool[Instance.countCurrentActive].position = pos;
                Instance.textPool[Instance.countCurrentActive].text = number.ToString();
            }
            else
            {
                var obj = Instantiate(Instance.beatNumberPrefab, pos, Quaternion.identity) as GameObject;
                obj.transform.SetParent(Instance.transform);
                obj.transform.localScale = Vector3.one;
                Instance.rectTransformPool.Add(obj.GetComponent<RectTransform>());
                Instance.textPool.Add(obj.GetComponent<Text>());
                Instance.size++;
            }

            Instance.countCurrentActive++;
        }

        static public void Begin()
        {
            Instance.countPrevActive = Instance.countCurrentActive;
            Instance.countCurrentActive = 0;
        }

        static public void End()
        {
            if (Instance.countCurrentActive < Instance.countPrevActive)
            {
                for (int i = Instance.countCurrentActive; i < Instance.countPrevActive; i++)
                {
                    Instance.textPool[i].gameObject.SetActive(false);
                }
            }

            if (Instance.countCurrentActive * 2 < Instance.size)
            {
                foreach (var text in Instance.textPool.Skip(Instance.countCurrentActive + 1))
                {
                    Destroy(text.gameObject);
                }

                Instance.rectTransformPool.RemoveRange(Instance.countCurrentActive, Instance.size - Instance.countCurrentActive);
                Instance.textPool.RemoveRange(Instance.countCurrentActive, Instance.size - Instance.countCurrentActive);
                Instance.size = Instance.countCurrentActive;
            }
        }
    }
}
