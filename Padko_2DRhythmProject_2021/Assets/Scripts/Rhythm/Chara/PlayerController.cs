using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteEditor.Utility;
using System.Linq;
using Game.Process;
using UniRx;
using Game;
public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    public CharaInfo charaInfo;
    [HideInInspector]
    public ReactiveProperty<int> hp;
    public ReactiveProperty<int> maxHp;//傳給Player 然後再去做判斷好了
    public Slider hpSlider;
    public Text hpUIText;
    public bool isDead;
    public GameObject[] playerPrefabs;//從預置物中取出要生成的角色
    public List<Player> players = new List<Player>();//生成時除了第一隻之外 都給他替身特效Shader
    public Material playerShadow;
    public Transform spawnParent;
    public Transform[] spawnPoints;
    public Transform moveCenter;//移動完會回到的原點 也是角色一開始生成的位置
    public Text skillText;
    public GameObject gameOverObj;
    public Button exitButton;
    [HideInInspector]
    public Vector3[] movePos = new Vector3[8];
    private float moveDistance = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        charaInfo = CharaInfo.GetAttr(PlayerSettings.Instance.selectChara);
        moveCenter = spawnPoints[(int)NotesController.Instance.keyType];
        InitPlayer();
        InitMovePos();
        InitHP();
        hp.Where(_ => !isDead).Subscribe(hp =>
        {        
            int skillHp = hp;
            if (hp <= 0)
            {
                skillHp = DeadSkill(hp, maxHp.Value);
                if(skillHp <= 0)
                {
                    isDead = true;
                    //暫停音樂 播放死亡動畫 以及GameOver顯示
                    if(!players[0].animator.GetBool("Die"))
                    {
                        players[0].animator.SetBool("Die", true);
                        MusicController.Instance.PauseMusic();
                        gameOverObj.SetActive(true);
                    }
                }
            }
            this.hp.Value = Mathf.Clamp(skillHp , 0, maxHp.Value);
            hpUIText.text = this.hp.Value + "/" + maxHp;
            if (this.hp.Value == 0)
                hpSlider.value = 0;
            else
                hpSlider.value = (float)this.hp.Value / (float)maxHp.Value;
        }).AddTo(this);

        exitButton.OnClickAsObservable().Subscribe(_ =>
        {
            RhythmFacade.Instance.ExitScene();
        });

    }
    public void InitMovePos()
    {
        movePos[(int)Direction.up] = new Vector2(0, moveDistance);
        movePos[(int)Direction.down] = new Vector2(0, -moveDistance);
        movePos[(int)Direction.left] = new Vector2(-moveDistance, 0);
        movePos[(int)Direction.right] = new Vector2(moveDistance, 0);

        movePos[(int)Direction.upperLeft] = new Vector2(-moveDistance, moveDistance);
        movePos[(int)Direction.upperRight] = new Vector2(moveDistance, moveDistance);
        movePos[(int)Direction.downLeft] = new Vector2(-moveDistance, -moveDistance);
        movePos[(int)Direction.downRight] = new Vector2(moveDistance, -moveDistance);
    }

    public virtual void InitHP()
    {
        maxHp.Value = charaInfo.maxHp;
        hp.Value = maxHp.Value;
    }
    public void InitPlayer()
    {
        for(int i=0; i< NotesController.Instance.laneCount + 2; i++)
        {
            //playerPrefabs.Where(x=>x.GetComponent<Player>().)

            var spawnObj = IAssetFactory.SpawnGameObject(obj: playerPrefabs[(int)charaInfo.nameType], pos: moveCenter.position , parent: spawnParent);
            //1.生成 2.最後把第一隻移動到最前面的圖層 3.設置shader
            var player = spawnObj.GetComponent<Player>();
            if (i>0)
            {
                player.image.material = playerShadow;
                player.trans.gameObject.SetActive(false);
            }
            players.Add(spawnObj.GetComponent<Player>());
            players[0].transform.SetAsLastSibling();
            if(NotesController.Instance.laneCount == 6)
            {
                players[0].isTenshi = true;
                players[0].tenshiObj.SetActive(true);
            }
        }
    }
    public void DoAnimation(int noteBlock)
    {
        if (isDead)
            return;
        //偵測當前按下的按鍵數量
        int runKeyCount = PlayController.Instance.keyStates.Where(x => x != (int)KeyState.none).Count();
        if (runKeyCount == 1)
        {
            players[0].DoAnimation(movePos, noteBlock);
        }
        else
        {
            for (int i = 1; i < NotesController.Instance.laneCount + 2; i++) // 預防可能不夠用的情況下 再追加兩個
            {
                if (!players[i].doFlag)
                {
                    players[i].DoAnimation(movePos, noteBlock, isActive: true);
                    break;
                }
            }
        }
    }
    public void SetHP(int value, bool isPlayAni = true)
    {


        if(value>0)
        {
            //加血 播放回血特效
        }   
        else
        {
            //扣血 播放受傷動畫
            value = players[0].DamageSkill(value);
            if (value == 0)
            {
                return;
            }
            if (isPlayAni)
            {
                players[0].animator.SetBool("Damage", true);
            }      
        }
        this.hp.Value += value;

    }

    public void SetSkillText(string str)
    {
        skillText.text = str;
    }

    public void ComboSkill(int combo)//根據Combo數觸發的能力 
    {
        players[0].ComboSkill(combo);
    }
    public int JudgeChangeSkill(int judge)//判定變更的能力
    {
        return players[0].JudgeChangeSkill(judge);
    }
    public int JudgeSkill(int judge)//根據判定觸發的能力
    {
        return players[0].JudgeSkill(judge);
    }
    public double ScoreSkill(double score) {

        return players[0].ScoreSkill(score);
    }
    public int OutMissSkill(bool isMiss)//判定Miss時，所觸發的能力
    {
        return players[0].OutMissSkill(isMiss);

    }
    public int DamageSkill(int damage)//受傷時觸發的能力
    {
        return players[0].DamageSkill(damage);
    }

    public int DeadSkill(int hp, int maxHp = default)//死亡時能觸發的能力
    {
        return players[0].DeadSkill(hp, maxHp) ;
    }

    public void SwitchNoteAssetSkill(GameObject noteObj, GameObject holdNoteObj) //替換音符素材
    {
        players[0].SwitchNoteAssetSkill(noteObj, holdNoteObj);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}


