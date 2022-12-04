using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteEditor.Utility;
using DG.Tweening;
using Game.Process;
using UniRx;
using System;

public class Player : MonoBehaviour
{
    public CharaNameType nameType;

    public Transform trans;

    public Image image;

    public Animator animator;

    public bool doFlag;
    public bool isDone;
    public ReactiveProperty<bool> isAttack;
    public ReactiveProperty<bool> isHolding;

    public Tweener doMoveClip;
    public Tweener doRotateClip;
    [HideInInspector]
    protected float doTime = 0.05f;

    [HideInInspector]
    public bool isTenshi = false;//6K進入天使型態

    public GameObject tenshiObj; //6K進入天使型態的翅膀

    public int keyBlock;
    public bool isActive;



    [HideInInspector]
    protected CompositeDisposable com = new CompositeDisposable();


    protected virtual void Awake()
    {
        tenshiObj.SetActive(isTenshi);
        isAttack.Subscribe(atk =>
        {
            animator.SetBool("Attack", atk);
            //Debug.Log("atk: " + atk);
        }).AddTo(this);
        isHolding.Subscribe(holding =>
        {
            animator.SetBool("Holding", holding);
            //Debug.Log("atk: " + atk);
        }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => isDone)
            .Subscribe(_ =>
            {
                if(PlayController.Instance.keyStates[keyBlock] != (int)KeyState.hold)
                {
                    BackCenter(isActive);
                }
            }).AddTo(this);
    }
    public void DoAnimation(Vector3[] movePos, int noteBlock, bool isActive = false)
    {
        doMoveClip.Kill();
        doRotateClip.Kill();
        com.Clear();
        isAttack.Value = false;
        isHolding.Value = false;
        doFlag = true;
        isDone = false;
        keyBlock = noteBlock;
        if (isActive)
        {
            trans.gameObject.SetActive(true);
            this.isActive = isActive;
        }
         
        var targetPos = NotesController.Instance.noteTracks[(int)NotesController.Instance.keyType].targetLines[noteBlock].position;
        float minDistance = float.MaxValue ;
        int targetIndex = 0;
        for(int i=0;i<8;i++)
        {
            var tempPos = movePos[i] + targetPos;
            float dist = (tempPos - PlayerController.Instance.moveCenter.position).sqrMagnitude;
            //float tempDistance = Vector3.Distance(tempPos, center.position);
            if(minDistance > dist)
            {
                minDistance = dist;
                targetIndex = i;
            }
        }
        doMoveClip = trans.DOMove(movePos[targetIndex] + targetPos, doTime);
        doRotateClip = trans.DORotateQuaternion(DirectionToAngleZ((Direction)targetIndex), doTime);
        //isAttack.Value = true;
        var doAtkAni = Observable.Timer(TimeSpan.FromSeconds((float)(doTime/3f)))
        .Subscribe(_ =>
        {
            isAttack.Value = true;//0.005
        }).AddTo(com);

        var exitDoAtkClip = Observable.Timer(TimeSpan.FromSeconds(doTime*1.5f))
        .Subscribe(_ =>
        {
            isAttack.Value = false;//0.075
        }).AddTo(com);
        
        var exitDoClip = Observable.Timer(TimeSpan.FromSeconds(doTime * 3f))
        .Subscribe(_ =>
        {
            //判斷這個時候是否還在按住 只要keyUP則
            if (PlayController.Instance.keyStates[noteBlock] == (int)KeyState.tap)
            {
                //if (PlayController.Instance.keyStates[noteBlock] != (int)KeyState.hold)
                //    PlayController.Instance.keyStates[noteBlock] = (int)KeyState.hold;

                //
                PlayController.Instance.keyStates[noteBlock] = (int)KeyState.hold;
                isHolding.Value = true;
            }
            isDone = true;//
        })
        .AddTo(com);
    }

    void BackCenter(bool isActive = false)
    {
        doMoveClip.Kill();
        doRotateClip.Kill();
        doMoveClip = trans.DOMove(PlayerController.Instance.moveCenter.position, doTime);
        doRotateClip = trans.DORotateQuaternion(PlayerController.Instance.moveCenter.rotation, doTime);
        isAttack.Value = false;
        isHolding.Value = false;
        doFlag = false;
        isDone = false;
        Observable.Timer(TimeSpan.FromSeconds(doTime))
        .Subscribe(_ =>
        {
            if (isActive)
            {
                trans.gameObject.SetActive(false);
            }
        }).AddTo(com);
    }

    public virtual void ComboSkill(int combo)//根據Combo數觸發的能力
    {

    }

    public virtual int JudgeChangeSkill(int judge)//轉換判定的能力
    {

        return judge;

    }

    public virtual int JudgeSkill(int judge)//根據判定觸發的能力
    {
        return judge;
    }
    public virtual double ScoreSkill(double score) { return score;  } //提高判定分數加成的能力



    public virtual int OutMissSkill(bool isMiss) {

        return ComboPresenter.MISS;
    }
    public virtual int DeadSkill(int hp, int maxHp = default)//死亡時能觸發的能力
    {
        return hp;
    }
    public virtual int DamageSkill(int damage)//受傷時能觸發的能力
    {
        return damage;
    }

    public virtual void SwitchNoteAssetSkill(GameObject noteObj, GameObject holdNoteObj) //替換音符素材
    {
        return;
    }
    protected virtual void Update()
    {
        
    }

    Quaternion DirectionToAngleZ(Direction dir)
    {

        //return dir * 45;
        Quaternion quaternion = Quaternion.Euler(0,0,0);

        if (dir == Direction.up)
        {
            trans.localScale = new Vector2(1, 1);
            quaternion = Quaternion.Euler(0, 0, 90);
        }
        else if (dir == Direction.down)
        {
            trans.localScale = new Vector2(1, 1);
            quaternion = Quaternion.Euler(0, 0, -90);
        }
        else if(dir == Direction.left || dir == Direction.right)
        {
            if(dir == Direction.left)
            {
                trans.localScale = new Vector2(-1,1);
            }
            else
            {
                trans.localScale = new Vector2(1, 1);
            }
            quaternion = Quaternion.Euler(0, 0, 0);
        }
        else if (dir == Direction.upperLeft || dir == Direction.upperRight)
        {
            if (dir == Direction.upperLeft)
            {
                trans.localScale = new Vector2(-1, 1);
                quaternion = Quaternion.Euler(0, 0, -45);
            }
            else
            {
                trans.localScale = new Vector2(1, 1);
                quaternion = Quaternion.Euler(0, 0, 45);
            }
        }
        else if (dir == Direction.downLeft || dir == Direction.downRight)
        {
            if (dir == Direction.downLeft)
            {
                trans.localScale = new Vector2(-1, 1);
                quaternion = Quaternion.Euler(0, 0, 45);
            }
            else
            {
                trans.localScale = new Vector2(1, 1);
                quaternion = Quaternion.Euler(0, 0, -45);
            }
        }
        return quaternion;
    }
}

public enum Direction
{
    up,
    down,
    left,
    right,
    upperLeft,
    upperRight,
    downLeft,
    downRight
}
public enum KeyState
{
    none,
    tap,
    hold
}

