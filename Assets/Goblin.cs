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

        void LateUpdate()
        {
            m_isFreezed = m_isKOed = m_isDead = false;
        }

        protected override void OnCharacterHit()
        {
            //暫無特效
        }

        protected override void OnCharacterFreezed(DefendBox damagedPart)
        {
            m_isFreezed = true;
        }

        protected override void OnCharacterKOed()
        {
            //暫無倒地
        }

        protected override void OnCharacterDied()
        {
            m_isDead = true;
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                c.enabled = false;
            }
        }

        protected override void CheckGlobalConditions()
        {
            if (m_isDead)
            {
                PerformGlobalTransition(Npc.Died);
                return;
            }
            if(m_isFreezed)
            {
                PerformGlobalTransition(Npc.Freezed);
                return;
            }
        }
    }

}
