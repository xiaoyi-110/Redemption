using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : EntityActor
{
    public PlayerController player;
    private FSM<NPC> fsm;
    public enum NPCLevel { Tier1, Tier2 }

    [Header("NPC设置")]
    public NPCLevel npcLevel = NPCLevel.Tier1;


    [Header("对话设置")]
    public int npcID;


    [Header("状态持续时间")]
    public float hallucinationDuration = 10f;
    public float unconsciousDuration = 5f;

    private List<FSMStateBase<NPC>> stateList;
    public enum InitialNPCState
    {
        Normal,
        Unconscious,
        Hallucinating
    }

    public InitialNPCState initialState = InitialNPCState.Normal;

    #region State
    public NPCHallucinatingState hallucinatingState;
    public NPCNormalState normalState;
    public NPCUnconsciousState unconsciousState;

    #endregion

    protected override void Awake()
    {
        base.Awake();
       
    }

    protected override void Start()
    {
        base.Start();
        CreateFSM();
    }

    private void CreateFSM()
    {
        stateList = new List<FSMStateBase<NPC>>()
        {
            new NPCNormalState(),
            new NPCUnconsciousState(),
            new NPCHallucinatingState(),
        };

        fsm = new FSM<NPC>(this);

        foreach (var state in stateList)
        {
            fsm.AddState(state);
        }

        switch (initialState)
        {
            case InitialNPCState.Normal:
                fsm.ChangeState<NPCNormalState>();
                break;
            case InitialNPCState.Unconscious:
                fsm.ChangeState<NPCUnconsciousState>();
                break;
            case InitialNPCState.Hallucinating:
                fsm.ChangeState<NPCHallucinatingState>();
                break;
            default:
                fsm.ChangeState<NPCNormalState>();
                break;
        }
    }

    protected override void Update()
    {
        fsm.Update();
    }
    
    public virtual void Interact(PlayerController player)
    {
        DialogManager.Instance.StartDialogue(npcID);
    }

}
