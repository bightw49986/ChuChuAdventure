using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FSMSystem
{


    private List<FSMState> m_states;
    private Dictionary<eFSMTransition, FSMState> m_GlobalMap;
    private eFSMStateID m_currentStateID;
    public eFSMStateID CurrentStateID { get { return m_currentStateID; } }
    private FSMState m_currentState;
    public FSMState CurrentState { get { return m_currentState; } }
    private AIData m_Data;
    public FSMSystem(AIData data)
    {
        m_Data = data;
        m_states = new List<FSMState>();
        m_GlobalMap = new Dictionary<eFSMTransition, FSMState>();
    }

    public void AddGlobalTransition(eFSMTransition t, FSMState s)
    {
        m_GlobalMap.Add(t, s);
    }

    public void PerformGlobalTransition(eFSMTransition t)
    {
        if(m_GlobalMap.ContainsKey(t))
        {
            m_currentState.DoBeforeLeave(m_Data);
            m_currentState = m_GlobalMap[t];
            m_currentState.DoBeforeEnter(m_Data);
            m_currentStateID = m_currentState.m_StateID;
        }
    }

    public void AddState(FSMState s)
    {
        if (s == null)
        {
            return;
        }

        if (m_states.Count == 0)
        {
            m_states.Add(s);
            m_currentState = s;
            m_currentStateID = s.m_StateID;
            return;
        }

        foreach (FSMState state in m_states)
        {
            if (state.m_StateID == s.m_StateID)
            {
                return;
            }
        }
        m_states.Add(s);
    }

    public void DeleteState(eFSMStateID id)
    {
        if (id == eFSMStateID.NullStateID)
        {
            return;
        }

        foreach (FSMState state in m_states)
        {
            if (state.m_StateID == id)
            {
                m_states.Remove(state);
                return;
            }
        }
    }

    public void PerformTransition(eFSMTransition trans)
    {
        if (trans == eFSMTransition.NullTransition)
        {
            return;
        }

        FSMState state = m_currentState.TransitionTo(trans);
        if (state == null)
        {
            return;
        }

        // Update the currentStateID and currentState		
        m_currentState.DoBeforeLeave(m_Data);
        m_currentState = state;
        m_currentStateID = state.m_StateID;
        m_currentState.DoBeforeEnter(m_Data);

    } 

    public void DoState()
    {
        m_currentState.CheckCondition(m_Data);
        m_currentState.Do(m_Data);
    }

}
