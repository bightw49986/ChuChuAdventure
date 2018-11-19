using System;
using System.Collections;
using UnityEngine;


namespace FSM
{
    namespace StatesLibrary
    {
        public partial class BasicNpc
        {
            /// <summary>
            /// 中斷動作並產生硬直的行為
            /// </summary>
            public class Freezed : NpcFSMState
            {
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Freezed;
                    }
                }

                public Freezed(NpcFSM FSM) : base(FSM) { }

                internal override void CheckConditions()
                {
                    if (m_FSM.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >=  0.8)
                    {
                        StartTransition(Npc.Confront);
                    }
                }

                internal override void OnStateEnter()
                {
                    m_FSM.m_isFreezed = m_FSM.m_isKOed = m_FSM.m_isDead = false;
                }

                internal override void OnStateExit()
                {
                    base.OnStateExit();
                }

                internal override void OnStateRunning()
                {
                    base.OnStateRunning();
                }
            }
        }
    }
}
