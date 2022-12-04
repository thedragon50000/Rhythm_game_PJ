using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class OpenFileByWin32 : MonoBehaviour
{

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
            
        OpenFile();
        StartCoroutine(DownSprite("1234"));
    }
    public float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 2)
        {
            Chuandong(Application.streamingAssetsPath + "/" + "1234" + ".png");
        }
    }
    //開啟檔案
    public void OpenFile()
    {
        FileOpenDialog dialog = new FileOpenDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        // dialog.filter = "exe files\0*.exe\0All Files\0*.*\0\0";

        dialog.filter = "圖片檔案(*.png*.jpg)\0*.png;*.jpg";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //預設路徑

        dialog.title = "Open File Dialog";

        dialog.defExt = "png";//顯示檔案的型別
        //注意一下專案不一定要全選 但是0x00000008項不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DialogShow.GetOpenFileName(dialog))
        {
            if (dialog.file != null)
            {
                Chuandong(dialog.file);//dialog.file本地圖片的地址

            }
            else
            {

                return;
            }
        }
    }


    public Image image;
    public string url;
    //把路徑圖片轉換成 Image 
    public void Chuandong(string path)
    {
        image.sprite = ChangeToSprite(ByteToTex2d(byteToImage(path)));
        url = path;
        print(url);
    }

    //根據圖片路徑返回圖片的位元組流byte[]
    public static byte[] byteToImage(string path)
    {
        FileStream files = new FileStream(path, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();

        return imgByte;
    }
    //根據位元組流轉換成圖片
    public static Texture2D ByteToTex2d(byte[] bytes)
    {
        int w = 500;
        int h = 500;
        Texture2D tex = new Texture2D(w, h);
        tex.LoadImage(bytes);
        return tex;
    }
    //轉換為Image
    private Sprite ChangeToSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    // 開啟協程就可以儲存成圖片了     
    IEnumerator DownSprite(string name)
    {

        yield return new WaitForSeconds(0);
        Texture2D tex = ByteToTex2d(byteToImage(url));

        //儲存本地          
        Byte[] bytes = tex.EncodeToPNG();
        var path = Application.streamingAssetsPath + "/" + name + ".png";
        File.WriteAllBytes(path, bytes); //filed_photo.text 是儲存圖片的名字
#if UNITY_EDITOR
        AssetDatabase.Refresh(); //重新整理Editor  不重新整理顯示不出來
#endif

    }

}
