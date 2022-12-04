using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Process;
public class RayRay : Player
{
    public bool isAwakening;
    public int missCount  = 7;
    public GameObject effectObj;

    public override double ScoreSkill(double score)
    {
        if(isAwakening)
        {
            return score * 1.5f;
        }
        return score;
    }


    public override int JudgeChangeSkill(int judge)
    {
        if(missCount>0)
        {
            judge = ComboPresenter.MISS;
            PlayerController.Instance.SetSkillText("�Ťj x " + missCount);
            missCount--;
        }
        else
        {
            if(!isAwakening)
            {
                PlayerController.Instance.SetSkillText("ı��!");
                effectObj.SetActive(true);
                isAwakening = true;
            }
        }
        return base.JudgeChangeSkill(judge);
    }
}
