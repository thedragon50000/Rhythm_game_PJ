
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using UnityEditor;

public static class UnityTool
{
    public static GameObject FindChild(GameObject parent, string childName, bool isContainSelf = false)
    {
        if(isContainSelf)
        {
            if (parent.name == childName)
            {
                return parent.gameObject;
            }
        }
        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive:true);
        bool isFinded = false;
        Transform child = null;
        foreach(Transform t in children)
        {
            if(t.name == childName)
            {
                if(isFinded)
                {
                    Debug.LogWarning("在遊戲物體" + parent + "下存在不止一個子物體" + childName);
                    return child.gameObject;
                }
                isFinded = true;
                child = t;
            }
        }
        if(child == null)
        {
            Debug.Log(childName + "找不到");
        }
        return child.gameObject;
    }
    public static void DestoryChild(Transform parent)
    {

        foreach (Transform child in parent)
        {
            FactoryManager.Destroy(child.gameObject);
        }


    }

    public static List<GameObject> GetChildGameObjects(Transform parent)
    {
        List<GameObject> objs = new List<GameObject>();
        for(int i=0; i<parent.childCount; i++)
        {
            if(parent.GetChild(i).gameObject.activeSelf)
            {
                objs.Add(parent.GetChild(i).gameObject);
            }
        }
        return objs;
    }
    public static int CompareCount(int a, int b)
    {
        if(a>b)
            return a;
        else
            return b;
    }
    public static T KeyByValue<T, W>(SerializableDictionary<T, W> dict, W val)
    {
        T key = default;
        foreach (KeyValuePair<T, W> pair in dict)
        {
            if (EqualityComparer<W>.Default.Equals(pair.Value, val))
            {
                key = pair.Key;
                break;
            }
        }
        return key;
    }
    public static string DontReapeatKey<T>(SerializableDictionary<string,T> dict, string key)
    {
        int count = 0;
        string returnKey = key;
        
        for (int i = 0; i <= dict.Count; i++)
        {
            if (dict.ContainsKey(returnKey))
            {
                count++;
                returnKey = key + "(" + count + ")";
            }
            else
            {

                break;
            }
        }
        return returnKey;
    }
    public static string FileDontReapeatName(string path, string name)
    {
        int count = 0;
        string selectPath = "";// Path.Combine(path, name);

        string changeName = Path.GetFileNameWithoutExtension(name);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(name);
        string extension = Path.GetExtension(name);
        selectPath = Path.Combine(path, changeName + extension);
        Debug.Log("selectPath:" + selectPath);

        while (true)
        {
            if(count>=50)
            {
                Debug.Log("Name ERROR");
                break; 
            }

            if(File.Exists(selectPath))//有其他同樣名稱的可能會覆蓋掉
            {
                changeName = fileNameWithoutExtension + " (" + count + ")";
                selectPath = Path.Combine(path, changeName + extension);
                count++;
            }
            else
            {
                break;
            }


        }
        return changeName + extension;
    }

    public static void MoveDirectory(string source, string target)
    {
        var sourcePath = source.TrimEnd('\\', ' ');
        var targetPath = target.TrimEnd('\\', ' ');
        var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                             .GroupBy(s => Path.GetDirectoryName(s));
        foreach (var folder in files)
        {
            var targetFolder = folder.Key.Replace(sourcePath, targetPath);
            Directory.CreateDirectory(targetFolder);
            foreach (var file in folder)
            {
                var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                if (File.Exists(targetFile)) File.Delete(targetFile);
                File.Move(file, targetFile);
            }
        }
        Directory.Delete(source, true);
    }
    public static void CopyMusicFile(string fileNameExtension)
    {
        string path = Application.streamingAssetsPath;

        var sourceMusic = Path.Combine(path, fileNameExtension);
        var distMusic = Path.Combine(path, "Musics", fileNameExtension);

        var distJsonFold = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension));
        var sourceJsonPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(fileNameExtension, "json"));
        var distJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(fileNameExtension, "json"));

        var sourceInfoJsonPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension), "MusicInfo.json");
        var distInfoJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), "MusicInfo.json");

        var sourceDirectory = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension));
        var distDirectory = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension));

        string newMusicFileName = FileDontReapeatName(Path.Combine(path, "Musics"), fileNameExtension);

        string newMusicFilePath = Path.Combine(path, "Musics", newMusicFileName);
        string newJsonFoldPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(newMusicFileName));
        string newJsonFilePath = Path.Combine(newJsonFoldPath, Path.ChangeExtension(newMusicFileName, "json"));
        string newInfoJson = Path.Combine(newJsonFoldPath, "MusicInfo.json");

        
        if(Directory.Exists(newJsonFoldPath))
        {
            Directory.Delete(newJsonFoldPath, true);
        }
        File.Copy(distMusic, newMusicFilePath);
        Directory.CreateDirectory(newJsonFoldPath);
        File.Copy(distJsonPath, newJsonFilePath);
        File.Copy(distInfoJsonPath, newInfoJson);
        var musicData = DeserializeJson<MusicInfoData>(newInfoJson);
        musicData.name = newMusicFileName;
        SerializeJson<MusicInfoData>(musicData, newInfoJson);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif


    }
    public static void DeleteMusicFile(string fileNameExtension)
    {
        string path = Application.streamingAssetsPath;

        var sourceMusic = Path.Combine(path, fileNameExtension);
        var distMusic = Path.Combine(path, "Musics", fileNameExtension);

        var jsonFold = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension));
        var sourceJsonPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(fileNameExtension, "json"));
        var distJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(fileNameExtension, "json"));

        var sourceInfoJsonPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension), "MusicInfo.json");
        var distInfoJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), "MusicInfo.json");

        var sourceDirectory = Path.Combine(path, Path.GetFileNameWithoutExtension(fileNameExtension));
        var distDirectory = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension));

        if (File.Exists(distMusic))
        {
            File.Delete(distMusic);
        }
        if (Directory.Exists(distDirectory))
        {
            Directory.Delete(distDirectory, true);
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public static List<string> GetMusicPath(string fileNameExtension)
    {
        string path = Application.streamingAssetsPath;

        List<string> pathList = new List<string>();

        var distMusic = Path.Combine(path, "Musics", fileNameExtension);

        pathList.Add(distMusic);

        var distJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), Path.ChangeExtension(fileNameExtension, "json"));

        pathList.Add(distJsonPath);


        var distInfoJsonPath = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension), "MusicInfo.json");

        var distDirectory = Path.Combine(path, "Notes", Path.GetFileNameWithoutExtension(fileNameExtension));


        pathList.Add(distInfoJsonPath);

        pathList.Add(distDirectory);




        return pathList;
    }

    public static void SerializeJson<T>(T jsonT, string path) 
    {
        var infoJson = UnityEngine.JsonUtility.ToJson(jsonT);
        File.WriteAllText(path, infoJson, System.Text.Encoding.UTF8);
    }
    public static T DeserializeJson<T>(string path)
    {
        var jsonT = default(T);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            jsonT = UnityEngine.JsonUtility.FromJson<T>(json);
        }
        return jsonT;
    }
}

public enum MusicPathType
{
    music,
    noteJson,
    infoJson,
    directory
}