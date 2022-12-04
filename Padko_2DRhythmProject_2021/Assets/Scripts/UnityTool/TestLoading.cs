using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class TestLoading : MonoBehaviour
{
    /// <summary>
    /// Image控件
    /// </summary>
    private Image image;

    void Start()
    {
        //image = this.transform.Find("Image").GetComponent<Image>();
        image = GetComponent<Image>();
        //?不同的按??定不同的事件
        this.transform.Find("LoadByWWW").GetComponent<Button>().onClick.AddListener
        (
           delegate () { LoadByWWW(); }
        );

        this.transform.Find("LoadByIO").GetComponent<Button>().onClick.AddListener
        (
          delegate () { LoadByIO(); }
        );
    }

    /// <summary>
    /// 以IO方式?行加?
    /// </summary>
    private void LoadByIO()
    {
        var path =  Application.dataPath + "/StreamingAssets/" + "hold.png";
        double startTime = (double)Time.time;
        //?建文件?取流
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //?建文件?度???
        byte[] bytes = new byte[fileStream.Length];
        //?取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //?放文件?取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //?建Texture
        int width = 300;
        int height = 372;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);

        //?建Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
               
        var definit = FactoryManager.Instance.localAssetFactory.mCreateStoryEditorAsset.definit;

        //definit.charas[0].images["喜"] = bytes;
        //texture.LoadImage(definit.charas[0].images["喜"]);

        startTime = (double)Time.time - startTime;
        Debug.Log("IO加?用?:" + startTime);
    }

    /// <summary>
    /// 以WWW方式?行加?
    /// </summary>
    private void LoadByWWW()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        double startTime = (double)Time.time;
        //?求WWW

        var path = "file://" + Application.dataPath + "/StreamingAssets/" + "hold.png";
        WWW www = new WWW(path);
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //?取Texture
            Texture2D texture = www.texture;

            //?建Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;

            startTime = (double)Time.time - startTime;
            Debug.Log("WWW加?用?:" + startTime);
        }
    }
}