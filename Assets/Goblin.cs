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
        }

        protected override void OnCharacterKOed()
        {
            //暫無倒地
        }

        protected override void OnCharacterDied()
        {
            if(m_isDead ==false)
            {
                m_isDead = true;
                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (var c in colliders)
                {
                    c.enabled = false;
                }
            }
        }

        protected override void CheckGlobalConditions()
        {
            if (m_isDead && m_Animator.IsInTransition(0)==false && bAllowedGloblaTransitions)
            {
                PerformGlobalTransition(Npc.Died);
                return;
            }
            if(m_isFreezed && m_Animator.IsInTransition(0) == false && bAllowedGloblaTransitions)
            {
                PerformGlobalTransition(Npc.Freezed);
                return;
            }
        }
    }

}
