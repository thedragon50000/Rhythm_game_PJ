using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class TestLoading : MonoBehaviour
{
    /// <summary>
    /// Image����
    /// </summary>
    private Image image;

    void Start()
    {
        //image = this.transform.Find("Image").GetComponent<Image>();
        image = GetComponent<Image>();
        //?���P����??�w���P���ƥ�
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
    /// �HIO�覡?��[?
    /// </summary>
    private void LoadByIO()
    {
        var path =  Application.dataPath + "/StreamingAssets/" + "hold.png";
        double startTime = (double)Time.time;
        //?�ؤ��?���y
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //?�ؤ��?��???
        byte[] bytes = new byte[fileStream.Length];
        //?�����
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //?����?���y
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //?��Texture
        int width = 300;
        int height = 372;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);

        //?��Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
               
        var definit = FactoryManager.Instance.localAssetFactory.mCreateStoryEditorAsset.definit;

        //definit.charas[0].images["��"] = bytes;
        //texture.LoadImage(definit.charas[0].images["��"]);

        startTime = (double)Time.time - startTime;
        Debug.Log("IO�[?��?:" + startTime);
    }

    /// <summary>
    /// �HWWW�覡?��[?
    /// </summary>
    private void LoadByWWW()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        double startTime = (double)Time.time;
        //?�DWWW

        var path = "file://" + Application.dataPath + "/StreamingAssets/" + "hold.png";
        WWW www = new WWW(path);
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //?��Texture
            Texture2D texture = www.texture;

            //?��Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;

            startTime = (double)Time.time - startTime;
            Debug.Log("WWW�[?��?:" + startTime);
        }
    }
}