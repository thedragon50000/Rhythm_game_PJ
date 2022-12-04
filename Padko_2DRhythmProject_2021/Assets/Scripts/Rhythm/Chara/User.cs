using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Process;
using UniRx;
using System;
using UnityEngine.UI;
public class User : Player
{
    public bool isDamageInvalid;//傷害無效化 把Miss全轉為Good


    protected override void Awake()
    {
        base.Awake();
    }
    public override int DamageSkill(int damage)
    {
        if (isDamageInvalid)
            return 0;
        if(!isDamageInvalid)
        {
            isDamageInvalid = true;
            PlayerController.Instance.SetSkillText("肝硬化");


            Observable.Timer(TimeSpan.FromSeconds(3))
            .Subscribe(_ =>
            {
                PlayerController.Instance.SetSkillText("");
                isDamageInvalid = false;
            }).AddTo(this);
        }
        return base.DamageSkill(damage);
    }
    public override int JudgeChangeSkill(int judge)
    {
        return base.JudgeChangeSkill(judge);
    }

    public override int OutMissSkill(bool isMiss)
    {
        if (isDamageInvalid)
            return ComboPresenter.GOOD;

        return base.OutMissSkill(isMiss);
    }



}
