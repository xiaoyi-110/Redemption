using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;

public class NPCStateBase:FSMStateBase<NPC>
{

    protected NPC npc;
    public override void SetStateMachine(FSM<NPC> fsm)
    {
        base.SetStateMachine(fsm);
        npc = fsm.Owner;
    }

    public override void OnUpdate(FSM<NPC> fsm)
    {
    }

    public override void OnEnter(FSM<NPC> fsm)
    {

    }
    public override void OnExit(FSM<NPC> fsm)
    {
    }


    
}
