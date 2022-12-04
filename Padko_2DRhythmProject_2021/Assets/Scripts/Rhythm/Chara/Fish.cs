using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Process;
public class Fish : Player
{

    public int missCount;


    public override int DamageSkill(int damage)
    {
        if(missCount >= 4)
        {
            damage *= 4;
            missCount = 0;
        }
        return base.DamageSkill(damage);
    }

    public override int JudgeChangeSkill(int judge)
    {
        if (judge == ComboPresenter.GOOD)
        {
            judge = ComboPresenter.GREAT;
        }
        return base.JudgeChangeSkill(judge);
    }

    public override int JudgeSkill(int judge)
    {
        if(judge == ComboPresenter.PERFECT)
        {
            PlayerController.Instance.SetHP(5);
        }

        if(judge == ComboPresenter.MISS)
        {
            missCount++;
            PlayerController.Instance.SetSkillText("消逝的偶像夢:" + missCount);

            //遭受四倍傷害
        }
        return base.JudgeSkill(judge);
    }




}
