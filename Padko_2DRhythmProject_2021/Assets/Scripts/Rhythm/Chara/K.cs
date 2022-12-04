using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K : Player
{
    public override double ScoreSkill(double score)
    {
        if(PlayerController.Instance.hp.Value > 10)
        {
            PlayerController.Instance.hp.Value -= 10;
            PlayerController.Instance.SetSkillText("¶W¶VªÌ");
            return score * 10;
        }
        else
        {
            PlayerController.Instance.SetSkillText("¦Y¤gªÌ");
        }
        return score ;
    }
}
