using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, BaseState> _availableStates;
    
    public BaseState CurrentState { get; private set; }
    public Action<BaseState> OnStateChanged;
    
    public void SetStates(Dictionary<Type, BaseState> states)
    {
        _availableStates = states;
    }

    private void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = _availableStates.Values.First();
        }
        
        Debug.Log($"currernt state is: {CurrentState}");

        // questionmark below could be changed to a return before the line
        
        var nextState = CurrentState?.Tick();

        if (nextState != null && 
            nextState != CurrentState?.GetType())
        {
            SwitchToNewState(nextState);
        }
    }

    private void SwitchToNewState(Type nextState)
    {
        CurrentState = _availableStates[nextState];
        OnStateChanged?.Invoke(CurrentState);
    }
}
