using UnityEngine;

public abstract class PlayerStateBase : FSMStateBase<PlayerController>
{
    protected PlayerController player; // Reference to the player controller
    public override void SetStateMachine(FSM<PlayerController> fsm)
    {
        base.SetStateMachine(fsm);
        player = fsm.Owner;
    }

    public override void OnEnter(FSM<PlayerController> fsm)
    {
        //Debug.Log($"[Player] Enter State: {GetType().Name}");
    }

    public override void OnExit(FSM<PlayerController> fsm)
    {
        //Debug.Log($"[Player] Exit State: {GetType().Name}");
    }
}
