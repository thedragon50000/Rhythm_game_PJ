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
    //�}���ɮ�
    public void OpenFile()
    {
        FileOpenDialog dialog = new FileOpenDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        // dialog.filter = "exe files\0*.exe\0All Files\0*.*\0\0";

        dialog.filter = "�Ϥ��ɮ�(*.png*.jpg)\0*.png;*.jpg";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //�w�]���|

        dialog.title = "Open File Dialog";

        dialog.defExt = "png";//����ɮת����O
        //�`�N�@�U�M�פ��@�w�n���� ���O0x00000008�����n�ʤ�
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DialogShow.GetOpenFileName(dialog))
        {
            if (dialog.file != null)
            {
                Chuandong(dialog.file);//dialog.file���a�Ϥ����a�}

            }
            else
            {

                return;
            }
        }
    }


    public Image image;
    public string url;
    //����|�Ϥ��ഫ�� Image 
    public void Chuandong(string path)
    {
        image.sprite = ChangeToSprite(ByteToTex2d(byteToImage(path)));
        url = path;
        print(url);
    }

    //�ھڹϤ����|��^�Ϥ����줸�լybyte[]
    public static byte[] byteToImage(string path)
    {
        FileStream files = new FileStream(path, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();

        return imgByte;
    }
    //�ھڦ줸�լy�ഫ���Ϥ�
    public static Texture2D ByteToTex2d(byte[] bytes)
    {
        int w = 500;
        int h = 500;
        Texture2D tex = new Texture2D(w, h);
        tex.LoadImage(bytes);
        return tex;
    }
    //�ഫ��Image
    private Sprite ChangeToSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    // �}�Ҩ�{�N�i�H�x�s���Ϥ��F     
    IEnumerator DownSprite(string name)
    {

        yield return new WaitForSeconds(0);
        Texture2D tex = ByteToTex2d(byteToImage(url));

        //�x�s���a          
        Byte[] bytes = tex.EncodeToPNG();
        var path = Application.streamingAssetsPath + "/" + name + ".png";
        File.WriteAllBytes(path, bytes); //filed_photo.text �O�x�s�Ϥ����W�r
#if UNITY_EDITOR
        AssetDatabase.Refresh(); //���s��zEditor  �����s��z��ܤ��X��
#endif

    }

}
