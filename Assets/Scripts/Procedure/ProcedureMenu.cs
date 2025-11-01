using UnityEngine;

public class ProcedureMenu : ProcedureBase
{
    public enum ProcedureMenuState : byte
    {
        None = 0,
        StartGame,
        StartTeamIntro,
    }

    private ProcedureMenuState m_CurrentState;

    public override void OnEnter(FSM<ProcedureManager> fsm)
    {
        base.OnEnter(fsm);

        m_CurrentState = ProcedureMenuState.None;

        Debug.Log("[ProcedureMenu] Enter Menu Procedure.");

        AudioManager.Instance.Play("BGM");
        AudioManager.Instance.Play("flame");
    }

    public override void OnExit(FSM<ProcedureManager> fsm)
    {
        base.OnExit(fsm);

        Debug.Log("[ProcedureMenu] Exit Menu Procedure.");

        // UIManager.Instance.CloseUIForm(Constant.UIFormData.Menu, this);
    }

    public override void OnUpdate(FSM<ProcedureManager> fsm)
    {
        switch (m_CurrentState)
        {
            case ProcedureMenuState.StartGame:
                Debug.Log("[ProcedureMenu] StartGame selected. Switching to ChangeScene -> Level1.");
                fsm.SetData(Constant.ProcedureData.NextSceneId, Constant.SceneData.Level1);
                fsm.ChangeState<ProcedureChangeScene>();
                break;

            default:
                break;
        }
    }

    public void SetMenuState(ProcedureMenuState state)
    {
        m_CurrentState = state;
    }

    public static ProcedureMenu Create()
    {
        return new ProcedureMenu();
    }
}
