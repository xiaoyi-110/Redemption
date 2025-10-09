using System.Collections.Generic;
using UnityEngine;

public class ProcedureManager : MonoSingleton<ProcedureManager>
{
    private FSM<ProcedureManager> m_FSM;
    private List<FSMStateBase<ProcedureManager>> m_StateList;

    private void Start()
    {
        UIManager.Instance.Init();
        //AssetManager.Instance.Init(); // 假设你的AssetManager有Init()方法
        //AudioManager.Instance.Init();
        CreateFSM();
    }

    private void Update()
    {
        m_FSM.Update();
    }

    private void CreateFSM()
    {
        m_FSM = new FSM<ProcedureManager>(this);
        m_StateList = new List<FSMStateBase<ProcedureManager>>()
        {
            ProcedureLaunch.Create(),
            ProcedureChangeScene.Create(),
            ProcedureMenu.Create(),
            ProcedureLevel.Create(),
        };


        //register states
        foreach (var state in m_StateList)
        {
            state.OnInit(m_FSM);
            m_FSM.AddState(state);
        }

        //Launch first
        m_FSM.ChangeState<ProcedureLaunch>();
    }

    public void GoToMainMenu()
    {
        m_FSM.ChangeState<ProcedureMenu>();
    }
}