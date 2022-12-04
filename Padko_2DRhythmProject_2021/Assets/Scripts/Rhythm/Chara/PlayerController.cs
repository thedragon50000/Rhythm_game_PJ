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
    public ReactiveProperty<int> maxHp;//�ǵ�Player �M��A�h���P�_�n�F
    public Slider hpSlider;
    public Text hpUIText;
    public bool isDead;
    public GameObject[] playerPrefabs;//�q�w�m�������X�n�ͦ�������
    public List<Player> players = new List<Player>();//�ͦ��ɰ��F�Ĥ@�����~ �����L�����S��Shader
    public Material playerShadow;
    public Transform spawnParent;
    public Transform[] spawnPoints;
    public Transform moveCenter;//���ʧ��|�^�쪺���I �]�O����@�}�l�ͦ�����m
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
                    //�Ȱ����� ���񦺤`�ʵe �H��GameOver���
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
            //1.�ͦ� 2.�̫��Ĥ@�����ʨ�̫e�����ϼh 3.�]�mshader
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
        //������e���U������ƶq
        int runKeyCount = PlayController.Instance.keyStates.Where(x => x != (int)KeyState.none).Count();
        if (runKeyCount == 1)
        {
            players[0].DoAnimation(movePos, noteBlock);
        }
        else
        {
            for (int i = 1; i < NotesController.Instance.laneCount + 2; i++) // �w���i�ण���Ϊ����p�U �A�l�[���
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
            //�[�� ����^��S��
        }   
        else
        {
            //���� ������˰ʵe
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

    public void ComboSkill(int combo)//�ھ�Combo��Ĳ�o����O 
    {
        players[0].ComboSkill(combo);
    }
    public int JudgeChangeSkill(int judge)//�P�w�ܧ󪺯�O
    {
        return players[0].JudgeChangeSkill(judge);
    }
    public int JudgeSkill(int judge)//�ھڧP�wĲ�o����O
    {
        return players[0].JudgeSkill(judge);
    }
    public double ScoreSkill(double score) {

        return players[0].ScoreSkill(score);
    }
    public int OutMissSkill(bool isMiss)//�P�wMiss�ɡA��Ĳ�o����O
    {
        return players[0].OutMissSkill(isMiss);

    }
    public int DamageSkill(int damage)//���ˮ�Ĳ�o����O
    {
        return players[0].DamageSkill(damage);
    }

    public int DeadSkill(int hp, int maxHp = default)//���`�ɯ�Ĳ�o����O
    {
        return players[0].DeadSkill(hp, maxHp) ;
    }

    public void SwitchNoteAssetSkill(GameObject noteObj, GameObject holdNoteObj) //�������ů���
    {
        players[0].SwitchNoteAssetSkill(noteObj, holdNoteObj);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}


