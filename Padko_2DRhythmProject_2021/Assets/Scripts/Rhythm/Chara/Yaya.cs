using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Process;
public class Yaya : Player
{
    public float perfectBonus;
    int perfectCount;

    public override double ScoreSkill(double score)
    {
        perfectBonus = perfectCount * 0.1f;

        float rate = 100 + (perfectBonus * 100);
        Debug.Log("rate:" + rate);
        PlayerController.Instance.SetSkillText("µ´¹ï­µ·P:" + rate + "%");
        return score + (perfectBonus * score);
    }

    public override int JudgeSkill(int judge)
    {
        if (judge == ComboPresenter.PERFECT)
        {
            perfectCount++;
        }
        else
        {
            perfectCount = 0;
        }
        return base.JudgeSkill(judge);
    }

    public override int OutMissSkill(bool isMiss)
    {
        ComboPresenter.Instance.totalScore.Value = 0;
        perfectCount = 0;
        return base.OutMissSkill(isMiss);
    }

}
