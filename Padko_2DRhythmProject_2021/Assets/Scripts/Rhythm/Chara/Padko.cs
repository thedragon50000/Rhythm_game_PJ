using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Process;
using DG.Tweening;
using UniRx;
using System;

public class Padko : Player
{
    public ReactiveProperty<int> dollCount = new ReactiveProperty<int>(20);
    public ReactiveProperty<int> perfectCount = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> kissCount = new ReactiveProperty<int>(0);
    public Tween doColorClip;
    public CompositeDisposable colorCom = new CompositeDisposable();

    ReactiveProperty<string> skillStr = new ReactiveProperty<string>();
    //中毒特效 變綠扣血

    public ParticleSystem poisonEffect;

    public Sprite noteImage, holdNoteImage;


    protected override void Awake()
    {
        base.Awake();
        dollCount.Value = 20;
        skillStr.Subscribe(str =>
        {
            PlayerController.Instance.SetSkillText(str);
        });
        dollCount.Subscribe(x =>
        {
            SetSkillStr();
        });
        perfectCount.Subscribe(x =>
        {
            SetSkillStr();
        });
    }


    public override int JudgeSkill(int judge)
    {
        if(judge == ComboPresenter.PERFECT)
        {
            perfectCount.Value ++ ;
        }
        if( judge == ComboPresenter.GOOD)
        {
            poisonEffect.gameObject.SetActive(true);
            poisonEffect.Play();
            if (colorCom.Count > 0)
                colorCom.Clear();
            var hp = PlayerController.Instance.hp.Value;
            if (hp > 0)
                PlayerController.Instance.SetHP(-(int)(hp * 0.1f), false);

            Observable.Timer(TimeSpan.FromSeconds(0.3f))
            .Subscribe(_ =>
            {
                poisonEffect.gameObject.SetActive(false);
            }).AddTo(colorCom);
        }
        return base.JudgeSkill(judge);
    }
    public override double ScoreSkill(double score)
    {
        if (perfectCount.Value >= 5)
        {
            score *= 5;
            kissCount.Value++;
            if(kissCount.Value >= 5)
            {
                score *= 5;
                kissCount.Value = 0;
            }
            
            perfectCount.Value = 0;
        }
        return base.ScoreSkill(score);
    }

    public override int JudgeChangeSkill(int judge)
    {
        if(dollCount.Value > 0)
        {
            if (judge != ComboPresenter.PERFECT)
            {
                judge = ComboPresenter.PERFECT;
                dollCount.Value--;
            }
        }

        return base.JudgeChangeSkill(judge);
    }

    public override int OutMissSkill(bool isMiss)
    {
        if (dollCount.Value > 0)
        {
            dollCount.Value--;
            return ComboPresenter.PERFECT;
        }
        return base.OutMissSkill(isMiss);
    }
    public override void SwitchNoteAssetSkill(GameObject noteObj, GameObject holdNoteObj)
    {
        noteObj.GetComponent<Image>().sprite = noteImage;
        holdNoteObj.GetComponent<Image>().sprite = holdNoteImage;
    }


    public void SetSkillStr()
    {
        skillStr.Value = "玩偶替身：" + dollCount.Value;
        skillStr.Value += "\n最喜歡User：" + perfectCount.Value;
        skillStr.Value += "\nUser 5MA：" + kissCount;
    }



}
