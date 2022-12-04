using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadImageByByte : MonoBehaviour
{
    public RawImage showImage;
    public Button loadBt;

    private string imgPath;
    private byte[] imageByte;

    private void Awake()
    {
        imgPath = Application.streamingAssetsPath + "/king.jpg";
        imageByte = SetImageToByte(imgPath);
    }

    void Start()
    {
        loadBt.onClick.AddListener(() =>
        {
            showImage.texture = GetTextureByByte(imageByte);
        });
    }

    /// <summary>
    /// ??¤ù???¦r???
    /// </summary>
    public byte[] SetImageToByte(string imgPath)
    {
        FileStream fs = new FileStream(imgPath, FileMode.Open);
        byte[] imgByte = new byte[fs.Length];
        fs.Read(imgByte, 0, imgByte.Length);
        fs.Close();
        return imgByte;
    }

    /// <summary>
    /// ?¦r???????²z    
    /// </summary>
    public Texture2D GetTextureByByte(byte[] imgByte)
    {
        Texture2D tex = new Texture2D(100, 100);
        tex.LoadImage(imgByte);
        return tex;
    }
}
