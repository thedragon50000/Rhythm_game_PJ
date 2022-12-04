using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCat : Player
{
    public bool isCrit;

    public override double ScoreSkill(double score) 
    { 
        if(isCrit)
        {
            isCrit = false;
            return score * 2;
        }

        return base.ScoreSkill(score);
    }
    public override void ComboSkill(int combo)
    {

        //Debug.Log("combo:" + combo);
        if(combo % 10 == 0 && !isCrit)
        {
            isCrit = true;
            //Debug.Log("平平子我婆!");
            PlayerController.Instance.SetSkillText("醒醒!");
        }
        else
        {

            PlayerController.Instance.SetSkillText("平平子我婆! x " + (combo % 10) );
            //PlayerController.Instance.SetSkillText("平平子我婆!");

            //if (PlayerController.Instance.skillText.text != "平平子我婆!")
            //{
            //    PlayerController.Instance.SetSkillText("平平子我婆!");
            //}

        }
    }


}
