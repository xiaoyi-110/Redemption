using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCNormalState : NPCStateBase
{
    public override void OnEnter(FSM<NPC> fsm)
    {
        base.OnEnter(fsm);
        npc.Animator.SetBool("Normal", true);
    }

    public override void OnExit(FSM<NPC> fsm)
    {
        base.OnExit(fsm);
        npc.Animator.SetBool("Normal", false);
    }
}
