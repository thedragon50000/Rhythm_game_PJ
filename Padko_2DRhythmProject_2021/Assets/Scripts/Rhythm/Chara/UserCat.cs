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
            //Debug.Log("�����l�ڱC!");
            PlayerController.Instance.SetSkillText("����!");
        }
        else
        {

            PlayerController.Instance.SetSkillText("�����l�ڱC! x " + (combo % 10) );
            //PlayerController.Instance.SetSkillText("�����l�ڱC!");

            //if (PlayerController.Instance.skillText.text != "�����l�ڱC!")
            //{
            //    PlayerController.Instance.SetSkillText("�����l�ڱC!");
            //}

        }
    }


}
