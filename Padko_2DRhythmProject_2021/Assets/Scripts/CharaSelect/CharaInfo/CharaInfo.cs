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
                chara.skillsShow = "�x�w�ơG�C�Ѽ��]��x�߱o���S���O�AMiss�ᦳ3���ɶ����|����A�ç⭵�ŧP�w�ରGood�C";
                break;
            case CharaNameType.UserCat:
                chara.maxHp = 200;
                chara._name = "UserCat";
                chara.price = 300;
                chara.type = CharaType.User;
                chara.skillsShow = "�ڿ��ɤ��G�@�����b���ڤ��A�۷Q�ۤv���ѱC���߫��ʪ��A�C��Combo�Ƭ�10�����ƮɡA\n" +
                    "�q���ڤ�����A�U�ӭ��ŧP�w�y���z����o2�������ƥ[���C";
                break;
            case CharaNameType.K:
                chara.maxHp = 500;
                chara._name = "�W�VK";
                chara.price = 400;
                chara.type = CharaType.User;
                chara.skillsShow = "�Y�g����G��ۨ��ͩR�j��10�A�h���Ӧۤv10�I�ͩR�A��o10�������ŧP�w�[���C";
                break;
            case CharaNameType.Orange:
                chara.maxHp = 100;
                chara._name = "Orange";
                chara.price = 100;
                chara.type = CharaType.User;
                chara.skillsShow = "�����G�L�׳Q����h�֦��A���i�H�H���ƪ��@�b�@���N���_�����͡C";
                break;
            case CharaNameType.Rayray:
                chara.maxHp = 200;
                chara._name = "���";
                chara.price = 99;
                chara.type = CharaType.User;
                chara.skillsShow = "�K�x���W�ơG�m�iSC�����Ťj�A�e�C�ӭ��ŧP�w�L�����ରMiss�A" +
                    "�b�Ťj����A�i�Jı�����A�A\n�C��SC����J1.5�����B�A�C�ӭ��ŧP�w���Ƥ]���W1.5���C";
                break;
            case CharaNameType.Yaya:
                chara.maxHp = 150;
                chara._name = "yaya";
                chara.price = 500;
                chara.type = CharaType.User;
                chara.skillsShow = "���ﭵ�P�G�uya�����v�A�����L�ʪ��ʧ@�ϱo�s��Perfect��o�W�j�T�[���A�C�h�@��Perfect�N�W��10%�����ŧP�w���ơC\n" +
                    "�T�Ҥ����G�u�nMiss�A�h��ya�Ӥ��~�y�A�u��ʳ��ۧڸ��b�p�«Τ��A��������k�s���g�@�C";
                break;
            case CharaNameType.Padko:
                chara.maxHp = 300;
                chara._name = "�����l";
                chara.price = 3000;
                chara.type = CharaType.Padko;
                chara.skillsShow = "�������a�G���������|��������A�N20��Great�H�U�����ŧP�w�ରPerfect�C\n" +
                    "�̳��wUser�G�C��Perfect��5�����ƮɡA�h��User�i��5MA�A�Ӧ����ŧP�w�����ƭ��H5���A�C��5MA������5�����ƮɡA�h�A���H5���C\n" +
                    "�L�����~�R�n�̡G�G�Q�G�ꪫ�ҫI�k�A�u�n���XGood�H�U���P�w�h���r����10%����q�C\n" +
                    "User�ӥ��y!�G�N�Ҧ������Ŵ�����User�C";
                break;
            case CharaNameType.Fish:
                chara.maxHp = 250;
                chara._name = "ICU�K��";
                chara.price = 500;
                chara.type = CharaType.Fish;
                chara.skillsShow = "�ۧڦw���G�C��Perfect���|�^�_5�I�ͩR�ȡA��P�w��Good�L�����ରGreat�C\n" +
                    "���u�������ڡG�C��Miss�ƶq��4�����ơA�h�B�~�Ө�4�����ˮ`�C";
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

