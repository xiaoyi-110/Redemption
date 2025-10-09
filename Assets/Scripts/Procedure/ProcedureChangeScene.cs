using System;
using DG.Tweening;   // DOTween namespace
using UnityEngine;

public class ProcedureChangeScene : ProcedureBase
{
    private string m_SceneName;
    private bool m_IsMenuScene;
    private bool m_IsChangeSceneComplete = false;

    public bool IsFadeInComplete;
    public bool IsFadeOutComplete;

    public override void OnEnter(FSM<ProcedureManager> fsm)
    {
        base.OnEnter(fsm);
        m_IsChangeSceneComplete = false;
        IsFadeInComplete = false;
        IsFadeOutComplete = false;

        // Get scene id from FSM data
        m_SceneName = fsm.GetData<string>(Constant.ProcedureData.NextSceneId);
        m_IsMenuScene = m_SceneName == Constant.SceneData.Menu;

        // Register events, start fade in
        //EventManager.Instance.RegisterEvent(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        //UIManager.Instance.StartTransitionFadeIn(this);

        Debug.Log($"[ProcedureChangeScene] Start changing scene to: {m_SceneName}");
    }

    public override void OnExit(FSM<ProcedureManager> fsm)
    {
        base.OnExit(fsm);

        // Unregister events, start fade out
        //EventManager.Instance.UnRegisterEvent(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        //UIManager.Instance.StartTransitionFadeOut();

        Debug.Log("[ProcedureChangeScene] Scene transition procedure finished.");
    }

    public override void OnUpdate(FSM<ProcedureManager> fsm)
    {
        if (!m_IsChangeSceneComplete || !IsFadeInComplete)
        {
            return;
        }

        if (m_IsMenuScene)
        {
            Debug.Log("[ProcedureChangeScene] Scene changed. Entering Menu Procedure.");
            fsm.ChangeState<ProcedureMenu>();
        }
        else
        {
            Debug.Log("[ProcedureChangeScene] Scene changed. Entering Level Procedure.");
            fsm.ChangeState<ProcedureLevel>();
        }
    }

    public void OnTransitionFadeInComplete()
    {
        IsFadeInComplete = true;
        Debug.Log("[ProcedureChangeScene] Fade-in complete, unloading old scenes...");
        ScenesManager.Instance.UnLoadAllScenes();
        ScenesManager.Instance.LoadScene(m_SceneName, this);
    }

    /*public void OnLoadSceneSuccess(object sender, EventArgs e)
    {
        LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
        if (ne.UserData != this)
        {
            return;
        }

        Debug.Log($"[ProcedureChangeScene] Scene {m_SceneName} loaded successfully.");
        m_IsChangeSceneComplete = true;
    }*/

    public static ProcedureChangeScene Create()
    {
        return new ProcedureChangeScene();
    }
}
