using System;
using UnityEngine;

public class ProcedureLevel : ProcedureBase
{
    private ProcedureLevelState m_ProcedureLevelState;

    public enum ProcedureLevelState : byte
    {
        None = 0,
        BackToMenu
    }

    public override void OnInit(FSM<ProcedureManager> fsm)
    {
        base.OnInit(fsm);
    }

    public override void OnEnter(FSM<ProcedureManager> fsm)
    {
        base.OnEnter(fsm);
        m_ProcedureLevelState = ProcedureLevelState.None;

        Debug.Log("[ProcedureLevel] Enter Level Procedure.");

        // LevelManager.Instance.InitLevel();
        // UIManager.Instance.OpenUIForm(Constant.UIFormData.Level, this);
    }

    public override void OnExit(FSM<ProcedureManager> fsm)
    {
        base.OnExit(fsm);

        Debug.Log("[ProcedureLevel] Exit Level Procedure.");

        // LevelManager.Instance.ExitLevel();
        // UIManager.Instance.CloseUIForm(Constant.UIFormData.Level, this);
    }

    public override void OnUpdate(FSM<ProcedureManager> fsm)
    {
        switch (m_ProcedureLevelState)
        {
            case ProcedureLevelState.BackToMenu:
                fsm.SetData(Constant.ProcedureData.NextSceneId, Constant.SceneData.Menu);
                fsm.ChangeState<ProcedureChangeScene>();
                break;

            default:
                break;
        }
    }

    public static ProcedureLevel Create()
    {
        return new ProcedureLevel();
    }

    public void BackToMenu()
    {
        m_ProcedureLevelState = ProcedureLevelState.BackToMenu;
    }
}
