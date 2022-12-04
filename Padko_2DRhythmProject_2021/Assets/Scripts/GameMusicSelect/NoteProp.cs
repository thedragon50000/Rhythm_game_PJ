using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.IO;
using UnityEditor;
using UniRx;

namespace Game.MusicSelect
{
    /// <summary>
    /// 譜的屬性（property）
    /// </summary>
    public class NoteProp : MonoBehaviour
    {
        // Image image;
        Text text;
        public ReactiveProperty<int> level;
        public ReactiveProperty<int> block;
        public Text levelText;
        public Text blockText;
        public Button deleteButton;
        //json文件名，无后缀
        public string fileName;
        //包含後綴名的音檔名稱
        public string fileNameExtension;

        void Awake()
        {
            // image = GetComponent<Image>();
            text = GetComponentInChildren<Text>();
            deleteButton.OnClickAsObservable()
            .Subscribe(_=>
            {
                DelectSourceFile();
            });

            level.Subscribe(x =>
            {
                levelText.color = GetLevelColor(x);
                levelText.text = "" + x;
            });
            block.Subscribe(x =>
            {
                blockText.text = x + "k";
            });
        }

        public void SetName(string n , string m)
        {
            fileName = n;
            fileNameExtension = m;
            text.text = n;
        }
        public void DelectSourceFile()
        {
            UnityTool.DeleteMusicFile(fileNameExtension);
            MusicSelector.Instance.Load();
        }

        public static Color GetLevelColor(int level)
        {
            Color color = default;
            if (level >= 15)
            {
                //紫色
                color = new Color(187 / 255.0f, 0 / 255.0f, 255 / 255.0f);
            }
            else if (level >= 10)
            {
                color = Color.red;
            }
            else if (level >= 5)
            {
                color = Color.yellow;
            }
            else
            {
                color = Color.green;
            }

            return color;
        }

    }
}
