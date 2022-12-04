using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using SFB;
using UnityEditor;

using System.Collections.Generic;

using System.IO;
using System.Linq;

public class OpenFile : MonoBehaviour
{


    /// <summary>
    /// 使用 WWW 加载图片，并赋值给 _rawImage
    /// </summary>
    /// <param name="_url">图片地址</param>
    /// <param name="_rawImage"></param>
    /// <returns></returns>
    public static IEnumerator LoadTexture2DByWWW(string _url, Texture2D _rawImage)
    {
        
        WWW _www = new WWW(_url);
        yield return _www;
        if (_www.error == null)
        {
            var imageBytes = File.ReadAllBytes(_url);
            if(imageBytes!=null)
                _rawImage.LoadImage(imageBytes);
            //_rawImage = _www.texture;
            TestIMG.instance.IMGSwitch();
            print("保存成功");
        }
        else
        {
            Debug.LogError(_www.error);
        }
    }




    void Start()
    {
        //LocalDialog.OpenDirectory("png;*.jpg");
        //var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        string path = "";
        var definit = FactoryManager.Instance.localAssetFactory.mCreateStoryEditorAsset.definit;

        // Open file with filter
        // 带文件类型过滤的打开文件窗
        var extensions = new[] {
        new ExtensionFilter("Music Files", "mp3", "wav" ),
        new ExtensionFilter("Notes Files", ".padko")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        Debug.Log("paths:" + paths[0]);
        
        string musicPath = Path.Combine(Application.streamingAssetsPath, "Musics");
        Debug.Log("Extension:"  + Path.GetExtension(paths[0]));



        //path = WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", "", "wav", false));
        //path = WriteResult(path);
        //Debug.Log("1:" + WriteResult(path));
        //string path3 = FactoryManager.Instance.localAssetFactory.mSavePath + "/hold.png";
        //string path2 = Application.streamingAssetsPath + "/hold.png";
        //path2 = WriteResult(path2);
        //Debug.Log("2:" + WriteResult(path2));

        File.Copy(paths[0], Path.Combine(musicPath, Path.GetFileName(paths[0])) ,true);
        



        //File.Move(path2, path3);
        //LoadByIo(path2);
        /*
        if (File.Exists(WriteResult(path)))
        {

            //File.Copy(WriteResult(path), WriteResult(path2));
        }*/
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            var definit = FactoryManager.Instance.localAssetFactory.mCreateStoryEditorAsset.definit;
            string path2 = Application.streamingAssetsPath + "/hold.png";

        }
    }
    public string WriteResult(string[] paths)
    {
        if (paths.Length == 0)
        {
            return null;
        }

        string _path = "";
        
        foreach (var p in paths)
        {
            _path += p ;
        }
        return _path;
    }
    public string WriteResult(string paths)
    {
        if (paths.Length == 0)
        {
            return null;
        }
        return paths.Replace("\\", "/");
    }
    IEnumerator DownloadMovie(string pathA ,string pathB)
    {
        FileStream fromFileStream = null;
        FileStream toFileStream = null;
        byte[] buffer = new byte[32768];
        int read;

        fromFileStream = new FileStream(pathA, FileMode.Open);
        toFileStream = new FileStream(pathB, FileMode.Create);

        while ((read = fromFileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            toFileStream.Write(buffer, 0, read);

            yield return new WaitForSeconds(0.01f);
        }

        fromFileStream.Close();
        toFileStream.Close();
    }
}



[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;


}




public class LocalDialog
{
    //鏈接指定系統函數       打開文件對話框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOFN([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);//執行打開文件的操作
    }

    //鏈接指定系統函數        另存?對話框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    public static bool GetSFN([In, Out] OpenFileName ofn)
    {
        return GetSaveFileName(ofn);//執行保存選中文件的操作
    }
    public static void OpenDirectory(string type)
    {
        var openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "文件(*." + type + ")\0*." + type + "";

        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默認路徑
        openFileName.title = "選擇文件";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetSaveFileName(openFileName))//點擊系統對話框框保存按鈕
        {
            //TODO
            GetSFN(openFileName);
            Debug.Log(openFileName.file);
            //將這個圖片存到unity文件裡面，並生成一個
        }


    }

}






public class UteCopyAllFileToKHD
{

    private static string formPath = Application.dataPath + @"\Arts\Scenes";//原路径
    private static string targetPath = @"../../../SourceCode\Client\Project\Assets\Arts\Scenes";//目标路径   ../表示当前项目文件的父路径
    private static bool isNull = false;

    /*
    [MenuItem("Tools/拷贝文件夹")]
    static void init()
    {
        Copy();
    }*/

    private static void Copy()
    {
        isNull = false;
        if (!Directory.Exists(targetPath))
        {
            Debug.LogError("未找到文件夹");
        }
        CleanDirectory(targetPath);
        CopyDirectory(formPath, targetPath);
        if (!isNull)
        {
            Debug.Log("Arts\\Scenes目录文件导入成功！！");
        }
    }
    /// <summary>
    /// 拷贝文件
    /// </summary>
    /// <param name="srcDir">起始文件夹</param>
    /// <param name="tgtDir">目标文件夹</param>
    public static void CopyDirectory(string srcDir, string tgtDir)
    {
        DirectoryInfo source = new DirectoryInfo(srcDir);
        DirectoryInfo target = new DirectoryInfo(tgtDir);

        if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new Exception("父目录不能拷贝到子目录！");
        }

        if (!source.Exists)
        {
            return;
        }

        if (!target.Exists)
        {
            target.Create();
        }

        FileInfo[] files = source.GetFiles();
        DirectoryInfo[] dirs = source.GetDirectories();
        if (files.Length == 0 && dirs.Length == 0)
        {
            Debug.LogError("当前项目中Arts\\Scenes文件夹为空");
            isNull = true;
            return;
        }
        for (int i = 0; i < files.Length; i++)
        {
            File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
        }
        for (int j = 0; j < dirs.Length; j++)
        {
            CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
        }
    }

    //删除目标文件夹下面所有文件
    public static void CleanDirectory(string dir)
    {
        foreach (string subdir in Directory.GetDirectories(dir))
        {
            Directory.Delete(subdir, true);
        }

        foreach (string subFile in Directory.GetFiles(dir))
        {
            File.Delete(subFile);
        }
    }
}
