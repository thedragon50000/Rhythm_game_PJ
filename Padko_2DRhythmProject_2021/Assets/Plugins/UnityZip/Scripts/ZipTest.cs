using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using zipLib;

/*
 * 测试用例
 * 把UI/下的内容压缩并放到StreamingAssets/UI.zip
 * 压缩功能只在PC下使用, 真机测试的时候请下载PC下压缩生成zip包
 *
 * 解压把StreamingAssets/UI.zip解压到StreamingAssets/UI/下
 * 我用了Logs-Viewer可以观察真机下的内存占用
 */

public class ZipTest : MonoBehaviour
{
    public static float sTime = 0;

	void Start ()
	{

    }

    public ZipHelper.ZipCallback ZipCB { get; set; }
    public ZipHelper.UnzipCallback UnZipCB { get; set; }

    public void OnClickZip()
    {
#if !UNITY_EDITOR
        return;
#endif

        ZipCB = new ZipResult();

        sTime = Time.realtimeSinceStartup;
        string[] paths = { Path.Combine(Application.dataPath, "UI/") };
        ZipHelper.Zip(paths, Path.Combine(Application.dataPath, "StreamingAssets/UI.zip"), null, ZipCB);
    }

    public void OnClickUnZip()
    {
        Debug.Log("Start UnZip");
        UnZipCB = new UnZipResult();

        sTime = Time.realtimeSinceStartup;
#if UNITY_EDITOR
        ZipHelper.UnzipFile(Path.Combine(Application.dataPath, "StreamingAssets/UI.zip"),
                            Path.Combine(Application.dataPath, "StreamingAssets/"), null, UnZipCB);
#else
        StartCoroutine(AndroidUnZip());
#endif
    }

    IEnumerator AndroidUnZip()
    {
        using (var _Request = UnityWebRequest.Get(Application.streamingAssetsPath + "/UI.zip"))
        {
            yield return _Request.SendWebRequest();
            if (_Request.isDone)
            {
                Debug.Log("Load File Suc.");
                ZipHelper.UnzipFile(_Request.downloadHandler.data, Application.temporaryCachePath, null, UnZipCB);
            }
        }
    }
}


