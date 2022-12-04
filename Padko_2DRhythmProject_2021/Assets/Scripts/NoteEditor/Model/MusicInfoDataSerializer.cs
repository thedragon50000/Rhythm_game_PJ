using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInfoDataSerializer 
{
    public static string Serialize()
    {
        var musicInfo = new MusicInfoData();
        #region 變更名稱 目前路徑有點問題還沒修，所以暫不提供更名

        //EditData.Name.Value = musicInfo.name;

        //var jsonFileName = Path.ChangeExtension(EditData.Name.Value, "json");
        //var musicName = Path.GetFileNameWithoutExtension(EditData.Name.Value);
        //var musicFileName = EditData.Name.Value;
        //var notesFold = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes");
        //var jsonFoldPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes/" + musicName);
        //var jsonPath = Path.Combine(jsonFoldPath, jsonFileName);
        //var musicPath = Path.Combine(Settings.WorkSpacePath.Value, "Musics");
        //var musicFilePath = Path.Combine(musicPath, musicFileName);

        //if(_name.Value != EditData.Name.Value)
        //{

        //    EditData.Name.Value = _name.Value;
        //    Debug.Log("musicFilePath:" + musicFilePath);
        //    Debug.Log("jsonFoldPath:" + jsonFoldPath);
        //    Debug.Log("jsonPath:" + jsonPath);

        //    //音樂名稱
        //    FileInfo musicFile = new FileInfo(musicFilePath);
        //    //string newName = Path.Combine(musicPath, _name.Value) + Path.GetExtension(musicFileName);
        //    string newName = UnityTool.FileDontReapeatName(musicPath, _name.Value);
        //    musicFile.Rename(newName);
        //    //資料夾名稱
        //    FileInfo jsonFoldFile = new FileInfo(jsonFoldPath);
        //    newName = UnityTool.FileDontReapeatName(notesFold, _name.Value, true);
        //    jsonFoldFile.Rename(newName);
        //    //json檔案名稱
        //    FileInfo jsonFile = new FileInfo(jsonPath);
        //    newName = UnityTool.FileDontReapeatName(jsonFoldPath, _name.Value + ".json");
        //    jsonFile.Rename(newName);
        //}

        //while(true)
        //{
        //    //if(Directory.Exists()) //如果有存在的檔案就變更名稱+個1吧
        //}

        //去音樂名稱那邊 變更音樂檔名 以及資料夾名稱 跟json檔名


        //musicInfo.name = _name.Value;
        //EditData.Name.Value = musicInfo.name;
        #endregion
        return UnityEngine.JsonUtility.ToJson(musicInfo);
    }
    public static MusicInfoData Deserialize(string json)
    {
        var musicInfo = UnityEngine.JsonUtility.FromJson<MusicInfoData>(json);
        return musicInfo;
    }


}
