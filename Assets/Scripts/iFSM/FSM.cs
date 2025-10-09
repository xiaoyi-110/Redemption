using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    public IState<T> currentState;
    private Dictionary<Type, IState<T>> stateDict = new Dictionary<Type, IState<T>>();
    private Dictionary<string, object> data = new Dictionary<string, object>();
    public T Owner { get; private set; }

    public FSM(T owner)
    {
        Owner = owner;
    }

    public void AddState(IState<T> state)
    {
        if (state == null) return;
        state.SetStateMachine(this);
        stateDict[state.GetType()] = state;
    }

    public void ChangeState<S>() where S : IState<T>
    {
        var type = typeof(S);
        if (!stateDict.TryGetValue(type, out var newState))
        {
            Debug.LogError($"FSM: State {type.Name} not found!");
            return;
        }

        currentState?.OnExit(this);
        
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void Update()
    {
        currentState?.OnUpdate(this);
    }

    public void SetData(string key, object value)
    {
        data[key] = value;
    }

    public TData GetData<TData>(string key)
    {
        if (data.TryGetValue(key, out object value))
        {
            return (TData)value;
        }
        Debug.LogWarning($"[FSM] No data found for key: {key}");
        return default;
    }
}
