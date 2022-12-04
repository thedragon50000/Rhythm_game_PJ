using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Process;
using System.Linq;
public class AniFunc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void attackExit()
    {
        //var track = CreateFromTrackTimerData.instance;
        //if (track.judgmentStruct[track.aquaAttack - 1].state == judgmentPoint.judgmentState.holding)
        //{
        //    track.holding = true;
        //}
        //CreateFromTrackTimerData.Track.aquaAttack = 0;

        //var player = transform.parent.GetComponent<Player>();

        //if (PlayController.Instance.keyStates[player.keyBlock] != (int)KeyState.none)
        //{
        //    if (PlayController.Instance.keyStates[Player.Instance.keyBlock] != (int)KeyState.hold)
        //        PlayController.Instance.keyStates[Player.Instance.keyBlock] = (int)KeyState.hold;
        //    player.isHolding.Value = true;
        //}


    }
    public void damageExit()
    {
        //CreateFromTrackTimerData.instance.aquaDamage = false;
        //Debug.Log("Damage有執行嗎?");
        //PlayerController.Instance.players[0].animator.SetBool("Damage", false);
        GetComponent<Animator>().SetBool("Damage", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
