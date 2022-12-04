using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoteEditor.Utility;
using NoteEditor.Model;
using Newtonsoft.Json;
using System.IO;
using Game;
namespace Game
{
    public class PlayerSettings : SingletonMonoBehaviour<PlayerSettings>
    {
        public float musicVolume = 0.5f, seVolume = 0.5f;

        public int clap = 1;

        public int playerOffset = 0;

        public int speed = 50;

        public string musicPath = "";

        public int coin;
        public SaveData saveData;

        //public int KEY0 = (int)KeyCode.A, KEY1 = (int)KeyCode.S, KEY2 = (int)KeyCode.D, KEY3 = (int)KeyCode.F, KEY4 = (int)KeyCode.G;
        //public List<CharaNameType> 
        public CharaNameType selectChara;

        void Awake()
        {
            
            LoadSettings();
            DontDestroyOnLoad(this);
        }

        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey("MusicVolume"))
                musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            if (PlayerPrefs.HasKey("SEVolume"))
                seVolume = PlayerPrefs.GetFloat("SEVolume");
            if (PlayerPrefs.HasKey("Clap"))
                clap = PlayerPrefs.GetInt("Clap");
            if (PlayerPrefs.HasKey("Offset"))
                playerOffset = PlayerPrefs.GetInt("Offset");
            if (PlayerPrefs.HasKey("Speed"))
                speed = PlayerPrefs.GetInt("Speed");
            if(PlayerPrefs.HasKey("Coin"))
                coin = PlayerPrefs.GetInt("Coin");
            if (PlayerPrefs.HasKey("SelectChara"))
                selectChara = (CharaNameType)PlayerPrefs.GetInt("SelectChara");

            /*
            if (PlayerPrefs.HasKey("KEY0"))
                KEY0 = PlayerPrefs.GetInt("KEY0");
            if (PlayerPrefs.HasKey("KEY1"))
                KEY1 = PlayerPrefs.GetInt("KEY1");
            if (PlayerPrefs.HasKey("KEY2"))
                KEY2 = PlayerPrefs.GetInt("KEY2");
            if (PlayerPrefs.HasKey("KEY3"))
                KEY3 = PlayerPrefs.GetInt("KEY3");
            if (PlayerPrefs.HasKey("KEY4"))
                KEY4 = PlayerPrefs.GetInt("KEY4");
            */

            if (PlayerPrefs.HasKey("MusicPath"))
                musicPath = PlayerPrefs.GetString("MusicPath");
        }

        public void SetMusicPath(string p)
        {
            musicPath = p;

            PlayerPrefs.SetString("MusicPath", musicPath);
        }

        public void SetSettings(float music, float se, int clap, int offset, int speed)
        {

            musicVolume = music;
            seVolume = se;
            this.clap = clap;
            this.playerOffset = offset;
            this.speed = speed;

            SaveSetting();
            //PlayerPrefs.SetFloat("MusicVolume", music);
            //PlayerPrefs.SetFloat("SEVolume", seVolume);
            //PlayerPrefs.SetInt("Clap", clap);
            //PlayerPrefs.SetInt("Offset", offset);
            //PlayerPrefs.SetInt("Speed", speed);


            //DontDestroyOnLoad(gameObject);
        }
        public void SaveSetting()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SEVolume", seVolume);
            PlayerPrefs.SetInt("Clap", clap);
            PlayerPrefs.SetInt("Offset", playerOffset);
            PlayerPrefs.SetInt("Speed", speed);
            PlayerPrefs.SetInt("Coin", coin);
            PlayerPrefs.SetInt("SelectChara", (int)selectChara);
        }

        public void InitSavaData()
        {
            saveData = SaveDataController.ReadData();
            if (saveData.listUnLockCharas.Count<=0)
            {
                saveData.listUnLockCharas.Add(CharaNameType.User); // 預設解鎖
            }
            //Debug.Log("saveData.unLockCharas:" + saveData.unLockCharas[0].ToString());
            //Debug.Log("selectChara:" +  selectChara.ToString());
            SaveSetting();
            SaveDataController.SaveData(saveData);
        }

        public void SaveJsonData()
        {
            SaveSetting();
            SaveDataController.SaveData(saveData);
        }
        public KeyCode GetKeyCode(int i, RhythmKeysType keyType = RhythmKeysType.five)
        { 
            
            if(keyType == RhythmKeysType.four)
            {
                if (i < 0 || i > 3)
                    return (KeyCode)saveData.rhythmKeys.fourKeys[0];
                return (KeyCode)saveData.rhythmKeys.fourKeys[i];
            }
            else if(keyType == RhythmKeysType.six)
            {
                if (i < 0 || i > 5)
                    return (KeyCode)saveData.rhythmKeys.sixKeys[0];
                return (KeyCode)saveData.rhythmKeys.sixKeys[i];
            }
            if (i < 0 || i > 4)
                return (KeyCode)saveData.rhythmKeys.fiveKeys[0];
            return (KeyCode)saveData.rhythmKeys.fiveKeys[i];
        }


        public void SetFourKeyCodes(KeyCode k0, KeyCode k1, KeyCode k2, KeyCode k3)
        {
            var fourKeys = new int[] { (int)k0, (int)k1, (int)k2, (int)k3 };
            saveData.rhythmKeys.fourKeys = fourKeys;
        }
        public void SetFiveKeyCodes(KeyCode k0, KeyCode k1, KeyCode k2, KeyCode k3, KeyCode k4)
        {
            var fiveKeys = new int[] { (int)k0, (int)k1, (int)k2, (int)k3 , (int)k4};
            saveData.rhythmKeys.fiveKeys = fiveKeys;
        }
        public void SetSixKeyCodes(KeyCode k0, KeyCode k1, KeyCode k2, KeyCode k3, KeyCode k4, KeyCode k5)
        {
            var sixKeys = new int[] { (int)k0, (int)k1, (int)k2, (int)k3, (int)k4 , (int)k5};
            saveData.rhythmKeys.sixKeys = sixKeys;
        }
        public void SetKeyCodes(KeyCode[] keys)
        {
            if(keys.Length == 4)
            {
                SetFourKeyCodes(keys[0], keys[1], keys[2], keys[3]);
            }
            else if (keys.Length == 5)
            {
                SetFiveKeyCodes(keys[0], keys[1], keys[2], keys[3], keys[4]);
            }
            else if(keys.Length == 6)
            {
                SetSixKeyCodes(keys[0], keys[1], keys[2], keys[3], keys[4], keys[5]);
            }
        }
    }

}
public class SaveData
{
    public List<MaxScoreData> listHighScoreDatas = new();
    public List<CharaNameType> listUnLockCharas = new();
    public RhythmKeys rhythmKeys = new();

}

public class SaveDataController
{
    public static void SaveData(SaveData data)//保存數據
    {
        string jsonStr = JsonConvert.SerializeObject(data);
        var path = Path.Combine(Application.persistentDataPath, "SaveData.json");

        File.WriteAllText(path, jsonStr, System.Text.Encoding.UTF8);
    }//
    public static SaveData ReadData()//读取数据 讀取完後進行寫入
    {
        //string str = PlayerPrefs.GetString("MaxScoreData");
        var path = Path.Combine(Application.persistentDataPath, "SaveData.json");
        SaveData saveData = null;
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            saveData = JsonConvert.DeserializeObject<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
            SaveData(saveData);
            PlayerSettings.Instance.selectChara = CharaNameType.User;
        }
        return saveData;
    }
}

public class RhythmKeys//遊戲中用到按鍵類
{
    public int[] fourKeys = new int[] { (int)KeyCode.D, (int)KeyCode.F, (int)KeyCode.J, (int)KeyCode.K };
    public int[] fiveKeys = new int[] { (int)KeyCode.D, (int)KeyCode.F, (int)KeyCode.Space, (int)KeyCode.J, (int)KeyCode.K };
    public int[] sixKeys = new int[] { (int)KeyCode.F, (int)KeyCode.D, (int)KeyCode.S, (int)KeyCode.J, (int)KeyCode.K, (int)KeyCode.L };

}
public enum RhythmKeysType
{
    four,
    five,
    six

}