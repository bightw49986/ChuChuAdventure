using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem;
using System;

public class Goblin : MonoBehaviour 
{
    Animator m_Animator;
    BattleData m_BattleData;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_BattleData = GetComponent<BattleData>();
    }


    void Start () 
    {
        m_BattleData.Died += OnDied;
        m_BattleData.Freezed += OnFreezed;
        for (int i = 0; i < m_BattleData.DefendBoxes.Count-1 ;i++)
        {
            m_BattleData.EneableDefendBox(i);
        }
	}

    void OnFreezed()
    {
        m_Animator.SetTrigger("Freezed");
    }

    void OnDied()
    {
        m_Animator.SetTrigger("Died");
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(var c in colliders)
        {
            c.enabled = false;
        }
    }
}
