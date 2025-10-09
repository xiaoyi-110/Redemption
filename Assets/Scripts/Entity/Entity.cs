using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public EntityType EntityType { get; protected set; }
    protected bool m_IsPause;
    protected virtual void Awake()
    {
        m_IsPause = false;
    }
    protected virtual void Start()
    {
        EventManager.Instance.RegisterEvent<OnLevelPauseChangeEventArgs>( OnLevelPauseChange);
    }

    protected virtual void Update()
    {
        if (!m_IsPause) OnUpdate();
        //Debug.Log(Time.timeScale);
    }

    protected virtual void FixedUpdate()
    {
        if (!m_IsPause) 
            OnFixedUpdate();
    }

    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() {  }

    public virtual void OnLevelPauseChange(object sender, EventArgs e)
    {
        var ne = e as OnLevelPauseChangeEventArgs;
        m_IsPause = ne.IsPause;
    }
}
