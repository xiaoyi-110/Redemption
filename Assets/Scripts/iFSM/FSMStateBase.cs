public abstract class FSMStateBase<T> : IState<T>
{
    protected FSM<T> fsm;   // Current FSM reference
    protected T owner;      // FSM owner (character/procedure manager)

    public virtual void SetStateMachine(FSM<T> fsm)
    {
        this.fsm = fsm;
        this.owner = fsm.Owner;
    }
    public virtual void OnInit(FSM<T> fsm) { }
    public abstract void OnEnter(FSM<T> fsm);
    public abstract void OnUpdate(FSM<T> fsm);
    public abstract void OnExit(FSM<T> fsm);
}
