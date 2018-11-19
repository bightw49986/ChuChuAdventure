using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem;
using System;


namespace FSM
{
    public class Goblin : NpcFSM
{
        void Start()
        {
            for (int i = 0; i < m_BattleData.DefendBoxes.Count - 1; i++)
            {
                m_BattleData.EneableDefendBox(i);
            }
        }


        protected override void OnCharacterHit()
        {
            //暫無特效
        }

        protected override void OnCharacterFreezed(DefendBox damagedPart)
        {
            if(m_isFreezed ==false)
            m_isFreezed = true;
            m_Animator.SetTrigger("Freezed");
            OriginState = CurrentState;
            CurrentState = globalTransitions[Npc.Freezed];
            CurrentState.OnStateEnter();
        }

        protected override void OnCharacterKOed()
        {
            if (m_isFreezed == false)
                m_isFreezed = true;
            m_Animator.SetTrigger("Freezed");
            OriginState = CurrentState;
            CurrentState = globalTransitions[Npc.Freezed];
            CurrentState.OnStateEnter();
        }

        protected override void OnCharacterDied()
        {
            if(m_isDead ==false)
            {
                m_isDead = true;
                m_Animator.SetTrigger("Died");
                OriginState = CurrentState;
                CurrentState = globalTransitions[Npc.Died];
                CurrentState.OnStateEnter();
                Rigidbody rig = GetComponent<Rigidbody>();
                rig.useGravity = false;
                rig.constraints = RigidbodyConstraints.FreezeAll;
                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (var c in colliders)
                {
                    c.enabled = false;
                }

            }
        }

        protected override void CheckGlobalConditions()
        {
            if (m_isDead)
            {
                m_Animator.SetTrigger("Died");
                OriginState = CurrentState;
                CurrentState = globalTransitions[Npc.Died];
                CurrentState.OnStateEnter();
                return;
            }
            //if(m_isFreezed)
            //{
            //    m_Animator.SetTrigger("Freezed");
            //    OriginState = CurrentState;
            //    CurrentState = globalTransitions[Npc.Freezed];
            //    CurrentState.OnStateEnter();
            //    return;
            //}
        }
    }

}
