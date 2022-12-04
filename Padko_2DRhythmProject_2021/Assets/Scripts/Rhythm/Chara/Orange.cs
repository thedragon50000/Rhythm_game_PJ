using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Process;
public class Orange : Player
{
    // Start is called before the first frame update
    public int deadCount = 0;


    public override int DeadSkill(int hp, int maxHp = default)
    {
        hp = maxHp;
        
        if (ComboPresenter.Instance.totalScore.Value > 0)
        {
            ComboPresenter.Instance.totalScore.Value /= 2;
            
        }
        deadCount++;
        PlayerController.Instance.SetSkillText("¦º¤`¼Æ:" + deadCount);

        return base.DeadSkill(hp);
    }


}
