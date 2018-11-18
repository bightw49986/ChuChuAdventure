using System;

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
                    base.OnStateEnter();

                }

                internal override void OnStateExit()
                {

                }

                internal override void OnStateRunning()
                {
                    base.OnStateRunning();
                }

                protected internal override void OnAnimatorMove()
                {
                    if (!m_FSM.m_AIData.IsInBattle)
                    {

                    }
                    if (m_FSM.m_AIData.IsInBattle)
                    {

                    }
                }
            }
        }
    }
}
