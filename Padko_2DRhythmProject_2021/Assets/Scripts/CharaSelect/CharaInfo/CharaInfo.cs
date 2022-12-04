using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public class CharaInfo : MonoBehaviour
{
    public int maxHp;
    public string _name;
    public string skillsShow;
    public CharaNameType nameType;
    public ReactiveProperty<bool> isUnlock = new ReactiveProperty<bool>();
    public CharaType type;
    public int price;
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        var chara = GetAttr(nameType);
        maxHp = chara.maxHp;
        _name = chara._name;
        price = chara.price;
        type = chara.type;
        skillsShow = chara.skillsShow;
    }

    public static CharaInfo GetAttr(CharaNameType nameType)
    {
        CharaInfo chara = new CharaInfo();
        chara.nameType = nameType;
        switch (nameType)
        {
            case CharaNameType.User:
                chara.maxHp = 250;
                chara._name = "User";
                chara.price = 0;
                chara.type = CharaType.User;
                chara.skillsShow = "肝硬化：每天熬夜跟台習得的特殊能力，Miss後有3秒的時間不會扣血，並把音符判定轉為Good。";
                break;
            case CharaNameType.UserCat:
                chara.maxHp = 200;
                chara._name = "UserCat";
                chara.price = 300;
                chara.type = CharaType.User;
                chara.skillsShow = "夢醒時分：一直活在美夢中，幻想自己有老婆的貓型動物，每當Combo數為10的倍數時，\n" +
                    "從美夢中驚醒，下個音符判定造成爆擊獲得2倍的分數加成。";
                break;
            case CharaNameType.K:
                chara.maxHp = 500;
                chara._name = "超越K";
                chara.price = 400;
                chara.type = CharaType.User;
                chara.skillsShow = "吃土本能：當自身生命大於10，則消耗自己10點生命，獲得10倍的音符判定加成。";
                break;
            case CharaNameType.Orange:
                chara.maxHp = 100;
                chara._name = "Orange";
                chara.price = 100;
                chara.type = CharaType.User;
                chara.skillsShow = "不滅：無論被打爛多少次，都可以以分數的一半作為代價復活重生。";
                break;
            case CharaNameType.Rayray:
                chara.maxHp = 200;
                chara._name = "瑞瑞";
                chara.price = 99;
                chara.type = CharaType.User;
                chara.skillsShow = "鐵掌水上飄：彩虹SC全部空大，前七個音符判定無條件轉為Miss，" +
                    "在空大完後，進入覺醒姿態，\n每個SC都投入1.5倍金額，每個音符判定分數也乘上1.5倍。";
                break;
            case CharaNameType.Yaya:
                chara.maxHp = 150;
                chara._name = "yaya";
                chara.price = 500;
                chara.type = CharaType.User;
                chara.skillsShow = "絕對音感：優ya的身影，完美無缺的動作使得連續Perfect獲得超大幅加成，每多一個Perfect就上升10%的音符判定分數。\n" +
                    "禁忌之物：只要Miss，則不ya照片外流，只能封閉自我躲在小黑屋中，受到分數歸零的懲罰。";
                break;
            case CharaNameType.Padko:
                chara.maxHp = 300;
                chara._name = "平平子";
                chara.price = 3000;
                chara.type = CharaType.Padko;
                chara.skillsShow = "玩偶之家：炸蝦玩偶會抵消攻擊，將20個Great以下的音符判定轉為Perfect。\n" +
                    "最喜歡User：每當Perfect為5的倍數時，則對User進行5MA，該次音符判定的分數乘以5倍，每當5MA的次數5的倍數時，則再乘以5倍。\n" +
                    "過期食品愛好者：胃被腐爛物所侵蝕，只要打出Good以下的判定則中毒扣除10%的血量。\n" +
                    "User來打球!：將所有的音符替換為User。";
                break;
            case CharaNameType.Fish:
                chara.maxHp = 250;
                chara._name = "ICU春魚";
                chara.price = 500;
                chara.type = CharaType.Fish;
                chara.skillsShow = "自我安慰：每個Perfect都會回復5點生命值，當判定為Good無條件轉為Great。\n" +
                    "消逝的偶像夢：每當Miss數量為4的倍數，則額外承受4倍的傷害。";
                break;
        }
        return chara;
    }
    //public void SetAttr(string name, int maxHp, CharaType type, int price)
    //{
    //}
}

public enum CharaNameType
{
    User,
    UserCat,
    K,
    Orange,
    Rayray,
    Yaya,
    Padko,
    Fish
}
public enum CharaType
{
    None,
    User,
    Padko,
    Fish
}

