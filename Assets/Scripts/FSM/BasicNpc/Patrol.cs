using System;
using AISystem;

namespace FSM
{
    namespace StatesLibrary
    {
        public partial class BasicNpc
        {
            /// <summary>
            /// 沿著Waypoint移動的行為
            /// </summary>
            public class Patrol : NpcFSMState
            {
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Patrol;
                    }
                }

                public Patrol(NpcFSM fsm) : base(fsm) { }

                internal override void CheckConditions()
                {
                    if (!m_FSM.m_AIData.IsInBattle)
                    {

                    }
                    if (m_FSM.m_AIData.IsInBattle)
                    {

                    }
                }

                internal override void OnStateEnter()
                {
                    m_FSM.m_AIData.Destination = m_FSM.m_AIData.IsInBattle ? AIData.DestinationState.BackToIdle : AIData.DestinationState.Patrol;
                }

                internal override void OnStateExit()
                {
                    m_FSM.ResetTriggers();
                }

                internal override void OnStateRunning()
                {
                    base.OnStateRunning();
                }
            }
        }
    }
}
