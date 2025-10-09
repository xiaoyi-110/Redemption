using UnityEngine;

public abstract class ProcedureBase : FSMStateBase<ProcedureManager>
{
    protected ProcedureManager manager;

    public override void SetStateMachine(FSM<ProcedureManager> fsm)
    {
        base.SetStateMachine(fsm);
        manager = fsm.Owner;
    }
 
    public override void OnEnter(FSM<ProcedureManager> fsm)
    {
        //Debug.Log($"[Procedure] Enter {GetType().Name}");
    }

    public override void OnExit(FSM<ProcedureManager> fsm)
    {
        //Debug.Log($"[Procedure] Exit {GetType().Name}");
    }
}
