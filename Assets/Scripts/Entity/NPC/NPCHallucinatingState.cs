using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHallucinatingState : NPCStateBase
{
    public override void OnEnter(FSM<NPC> fsm)
    {
        base.OnEnter(fsm);
        npc.Animator.SetBool("Hallucinating", true);
    }

    public override void OnExit(FSM<NPC> fsm)
    {
        base.OnExit(fsm);
        npc.Animator.SetBool("Hallucinating", false);
    }
}
