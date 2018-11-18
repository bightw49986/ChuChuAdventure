using System;
using UnityEngine;

namespace FSM
{
    namespace StatesLibrary
    {
        public partial class BasicNpc
        {
            /// <summary>
            /// 緩緩接近目標的行為
            /// </summary>
            public class Approach : NpcFSMState
            {
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Approach;
                    }
                }

                public Approach(NpcFSM fsm) : base(fsm) { }

                internal override void CheckConditions()
                {
                    if (!m_FSM.m_AIData.IsInBattle) //戰鬥外
                    {
                        //敵人第一次進追擊範圍 Chase
                        if (m_FSM.m_AIData.PlayerInChaseRange())
                        {
                            m_FSM.m_AIData.EnterBattle();
                            StartTransition(Npc.Chase);
                            return;
                        }
                    }
                    if (m_FSM.m_AIData.IsInBattle) //戰鬥中
                    {
                        if(m_FSM.m_AIData.AtkReady)
                        {
                            if (m_FSM.m_AIData.PlayerInAtkRange()) //距離夠近
                            {
                                m_FSM.m_AIData.Attack();
                                StartTransition(Npc.Attack, 0);
                                return;
                            }
                            StartTransition(Npc.Chase);
                            return;
                        }
                        if (m_FSM.m_AIData.PlayerInJumpAtkRange())
                        {
                            StartTransition(Npc.Confront, 0);
                            return;
                        }
                      
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
                    //如果與玩家之間有障礙物，算出沒有障礙物的點，把目標位置改成那個點
                }

                protected internal override void OnAnimatorMove()
                {
                    m_FSM.m_AIData.MoveToPlayer();
                }
            }
        }
    }
}
