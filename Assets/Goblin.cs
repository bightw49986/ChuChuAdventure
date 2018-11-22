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
            ResetTriggers();
            m_Animator.SetTrigger("Freezed");
            ResetTriggers();
        }

        protected override void OnCharacterKOed()
        {
            ResetTriggers();
            m_Animator.SetTrigger("Freezed");
            ResetTriggers();
        }

        protected override void OnCharacterDied()
        {
            ResetTriggers();
            m_Animator.SetTrigger("Died");
            ResetTriggers();
            Rigidbody rig = GetComponent<Rigidbody>();
            rig.useGravity = false;
            rig.constraints = RigidbodyConstraints.FreezeAll;
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                c.enabled = false;
            }
        }

        protected override void CheckGlobalConditions()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Died"))
            {
                if(CurrentState.StateID != (Enum)Npc.Died && !bTranfering)
                {
                    CurrentState = globalTransitions[Npc.Died];
                }
                return;
            }
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Freezed"))
            {
                if(CurrentState.StateID != (Enum)Npc.Freezed && !bTranfering)
                {
                    PerformTransition(Npc.Confront);
                }
                return;
            }
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 4)
            {
                if (m_AIData.IsInBattle && CurrentState.StateID != (Enum)Npc.Chase)
                {
                    ResetTriggers();
                    PerformTransition(Npc.Confront);
                }
                else
                {
                    ResetTriggers();
                    PerformTransition(Npc.Idle);
                }
            }
        }

        public void Jump_On()
        {
            Rigidbody rig = GetComponent<Rigidbody>();
            if (rig)
            {
                rig.useGravity = false;
                rig.isKinematic = true;
            }
        }

        public void Jump_Off()
        {
            Rigidbody rig = GetComponent<Rigidbody>();
            if (rig)
            {
                rig.useGravity = true;
                rig.isKinematic = false;
            }
        }

        public void Atk_On(int index)
        {
            m_BattleData.EnableAttackBox(index);
        }

        public void Atk_Off(int index)
        {
            m_BattleData.DisableAttackBox(index);
        }
    }

}
