using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInfoDataSerializer 
{
    public static string Serialize()
    {
        var musicInfo = new MusicInfoData();
        #region �ܧ�W�� �ثe���|���I���D�٨S�סA�ҥH�Ȥ����ѧ�W

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

        //    //���֦W��
        //    FileInfo musicFile = new FileInfo(musicFilePath);
        //    //string newName = Path.Combine(musicPath, _name.Value) + Path.GetExtension(musicFileName);
        //    string newName = UnityTool.FileDontReapeatName(musicPath, _name.Value);
        //    musicFile.Rename(newName);
        //    //��Ƨ��W��
        //    FileInfo jsonFoldFile = new FileInfo(jsonFoldPath);
        //    newName = UnityTool.FileDontReapeatName(notesFold, _name.Value, true);
        //    jsonFoldFile.Rename(newName);
        //    //json�ɮצW��
        //    FileInfo jsonFile = new FileInfo(jsonPath);
        //    newName = UnityTool.FileDontReapeatName(jsonFoldPath, _name.Value + ".json");
        //    jsonFile.Rename(newName);
        //}

        //while(true)
        //{
        //    //if(Directory.Exists()) //�p�G���s�b���ɮ״N�ܧ�W��+��1�a
        //}

        //�h���֦W�٨��� �ܧ󭵼��ɦW �H�θ�Ƨ��W�� ��json�ɦW


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
