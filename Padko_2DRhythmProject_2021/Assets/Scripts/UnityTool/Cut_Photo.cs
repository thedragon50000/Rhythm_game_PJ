using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 切換圖片顯示, 讀取unity本地圖片檔案顯示在ui
/// </summary>
public class Cut_Photo : MonoBehaviour
{
    public Dictionary<int, Sprite> spriteDic = new Dictionary<int, Sprite>();
    public Button button_zuo;
    public Button button_you;
    public Image image;
    private int aa = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        /*
        GetTexture();
        // image = this.transform.GetComponent<Image>();
        image.sprite = spriteDic[aa];

        Debug.Log(spriteDic.Count);
        button_zuo.onClick.AddListener(delegate {

            if (aa != 0)
            {
                aa--;
            }
            else
            {
                aa = (spriteDic.Count - 1);
            }
            image.sprite = spriteDic[aa];
        });
        button_you.onClick.AddListener(delegate {
            if (aa != (spriteDic.Count - 1))
            {
                aa++;
            }
            else
            {
                aa = 0;
            }
            image.sprite = spriteDic[aa];
        });
        */
    }


    //獲取圖片資訊
    public void GetTexture()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/DataerTion");
        FileInfo[] filess = dir.GetFiles("*.png");//獲取所有檔案的資訊
        int i = 0;
        foreach (FileInfo file in filess)
        {
            FileStream fs = new FileStream(Application.streamingAssetsPath + "/DataerTion/" + file.Name, FileMode.Open);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(buffer);
            tex.Apply();
            spriteDic.Add(i, ChangeToSprite(tex));
            i++;
        }
    }
    //轉換為Image
    private Sprite ChangeToSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    // Update is called once per frame
    void Update()
    {

    }
}