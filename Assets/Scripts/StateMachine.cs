using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public class State
    {
        public string Name { get; }

        // Timings
        public float StartTime;
        public float ActiveTime => Time.time - StartTime; // Amount of seconds since this state was entered
        
        

        public State(string name)
        {
            Name = name;
        }

        public Action<StateMachine.State> OnStateEnter;
        public Action<StateMachine.State> OnStateUpdate;
        public Action<StateMachine.State> OnStateExit;
        
    }
    

    public StateMachine(State startState)
    {
        ActiveState = startState;
    }


    private State _activeState;
    public State ActiveState
    {
        get => _activeState;
        set
        {
            _activeState?.OnStateExit?.Invoke(_activeState);

            _activeState = value;
            _activeState.StartTime = Time.time;

            _activeState.OnStateEnter?.Invoke(_activeState);
        }
    }


    public void Update()
    {
        _activeState.OnStateUpdate?.Invoke(_activeState);
    }
}
