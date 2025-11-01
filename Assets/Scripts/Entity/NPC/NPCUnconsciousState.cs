using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUnconsciousState : NPCStateBase
{
    public override void OnEnter(FSM<NPC> fsm)
    {
        base.OnEnter(fsm);
        npc.Animator.SetBool("Unconscious", true);
    }

    public override void OnExit(FSM<NPC> fsm)
    {
        base.OnExit(fsm);
        npc.Animator.SetBool("Unconscious", false);
    }

}
