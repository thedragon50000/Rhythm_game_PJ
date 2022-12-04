using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteEditor.Utility;
using Game;
using System.Linq;
using UniRx;
using DG.Tweening;
public class CharaPartyUI : SingletonMonoBehaviour<CharaPartyUI>
{
    public List<CharaInfo> charasInfo = new List<CharaInfo>(); 
    public List<GameObject> charasPrefabs = new List<GameObject>();
    public CharaInfo showCharaInfo;
    public CharaNameType selectCharaNameType;
    int showCount;
    //public List<CharaInfo> unlockCharasInfo = new List<CharaInfo>();
    //public List<CharaInfo> lockCharasInfo = new List<CharaInfo>();

    //public List<CharaNameType> unlockCharas = new List<CharaNameType>();
    // public List<CharaName> unlockCharas = new List<CharaName>(); //如果值不存在
    public Text nameText;
    public Text typeText;
    public Text hpText;

    public Text skillsShowText;

    public Button lockButton;
    public Text lockPriceText;
    public ReactiveProperty<int> lockPrice = new ReactiveProperty<int>();
    public Button selectButton;
    public Text selectText;
    public Button leftButton;
    public Button rightButton;
    public Button exitButton;//離開時，進行儲存
    [HideInInspector]
    public ReactiveProperty<int> coin = new ReactiveProperty<int>();
    public Text coinText;

    [HideInInspector]
    public Tween doMove;
    public Transform contentTrans;
    public Transform centerTrans;//
    public Transform leftTrans; // 
    public Transform rightTrans; // 

    // Start is called before the first frame update
    void Start()
    {
        var saveData = PlayerSettings.Instance.saveData;
        foreach (var item in charasPrefabs)
        {
            var spawnObj = IAssetFactory.SpawnGameObject(obj: item, pos: rightTrans.position, parent: contentTrans);
            //生成
            var info = spawnObj.GetComponent<CharaInfo>();
            for(int i=0; i< saveData.listUnLockCharas.Count; i++)
            {
                if(info.nameType == saveData.listUnLockCharas[i])
                {
                    info.isUnlock.Value = true;
                    break;
                }
            }
            charasInfo.Add(info);
        }
        var unlockCharas = charasInfo.Where(x => x.isUnlock.Value).ToList();
        //if(unlockCharas.Count > 0)
        //{
        //    //SetSelectChara(unlockCharas[0]);
        //}
        coin.Value = PlayerSettings.Instance.coin;
        //coin.Value = 99999;
        coin.SubscribeToText(coinText);
        lockPrice.SubscribeToText(lockPriceText);
        showCount = (int)CharaNameType.User;
        selectCharaNameType = PlayerSettings.Instance.selectChara;
        SetShowChara();
        leftButton.OnClickAsObservable().Subscribe(_ =>
        {
            SetShowChara(Direction.left);
        });
        rightButton.OnClickAsObservable().Subscribe(_ =>
        {
            SetShowChara(Direction.right);
        });
        selectButton.OnClickAsObservable().Where(x => showCharaInfo.nameType != selectCharaNameType).Subscribe(_ =>
        {
            SetSelectCharaButtonColor(true);
        });
        lockButton.OnClickAsObservable().Subscribe(_ =>
        {
            if(coin.Value >= lockPrice.Value)
            {
                coin.Value -= lockPrice.Value;
                PlayerSettings.Instance.coin = coin.Value;
                showCharaInfo.isUnlock.Value = true;
                saveData.listUnLockCharas.Add(showCharaInfo.nameType);
                PlayerSettings.Instance.SaveJsonData();
                SwitchLockButton(true);
            }
            else
            {
                Debug.Log("金錢不足");
            }
        });
        exitButton.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerSettings.Instance.SaveJsonData();
            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.MainMenuScene;
        });

    }

    public void SetShowChara(Direction direction = default)
    {
        float doTime = 0.5f;
        if(showCharaInfo != null && direction != default)
        {
            if (direction == Direction.left)
            {
                charasInfo[showCount].transform.DOMove(leftTrans.position, doTime);
                showCount--;
                if (showCount < 0)
                {
                    showCount = charasInfo.Count - 1;
                }
            }
            else
            {
                charasInfo[showCount].transform.DOMove(rightTrans.position, doTime);
                showCount++;
                if (showCount > charasInfo.Count - 1)
                {
                    showCount = 0;
                }
            }
        }
        showCount = Mathf.Clamp(showCount, 0, charasInfo.Count - 1);
        showCharaInfo = charasInfo[showCount];
        if (direction == Direction.left)
        {
            showCharaInfo.transform.position = rightTrans.position;
        }
        else
        {
            showCharaInfo.transform.position = leftTrans.position;
        }
        doMove = showCharaInfo.transform.DOMove(centerTrans.position, doTime);
        ShowUI(showCharaInfo);
    }

    public void ShowUI(CharaInfo info)
    {
        nameText.text = info._name;
        typeText.text = info.type.ToString();
        hpText.text = "生命值：" + info.maxHp;
        skillsShowText.text = info.skillsShow;
        //Debug.Log("info.nameType:" + info.nameType);
        SwitchLockButton(info.isUnlock.Value);
        if (!info.isUnlock.Value)
        {
            lockPrice.Value = info.price;
            SetSelectCharaButtonColor(false);
        }
        else
        {
            //已經解鎖的情況下就是看是否要選取
            if(info.nameType == selectCharaNameType)
            {
                SetSelectCharaButtonColor(true);
            }
            else
            {
                SetSelectCharaButtonColor(false);
            }

        }
    }

    public void SwitchLockButton(bool isUnlock)
    {
        if(!isUnlock)
        {
            lockButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else
        {
            lockButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
    }


    public void SetSelectCharaButtonColor(bool isSelect)
    {
        if(isSelect)
        {
            selectButton.image.color = Color.gray;
            selectText.color = Color.white;
            selectText.text = "已選擇";
            selectCharaNameType = showCharaInfo.nameType;
            PlayerSettings.Instance.selectChara = selectCharaNameType;
        }
        else
        {
            selectButton.image.color = Color.white;
            selectText.color = Color.black;
            selectText.text = "可選擇";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
