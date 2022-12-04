using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInfoData
{
    //這個資訊每首歌的檔名都一樣 方便在解壓縮的時候先拿出來參照

    public string name;
    public string author;
    public string source;//版權曲的來源

    //是做給哪個Vtuber的
    public VtuberType vtuberType;
    public int level;//難度

    //最高分那些的判定 要放到C槽保存資料夾裡面

}
