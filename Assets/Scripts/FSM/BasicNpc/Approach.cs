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
                float fTired = 0f;

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
                    if (!m_FSM.m_AIData.IsInBattle) //戰鬥外Approach代表第一次發現敵人，緩緩靠近確認
                    {
                        //敵人第一次進追擊範圍 進入戰鬥
                        if (m_FSM.m_AIData.PlayerInChaseRange)
                        {
                            m_FSM.m_AIData.EnterBattle();
                            StartTransition(Npc.Chase);
                            return;
                        }
                    }
                    if (m_FSM.m_AIData.IsInBattle) //戰鬥中
                    {
                        if (m_FSM.m_AIData.PlayerInJumpAtkRange) //如果敵人在撲擊範圍內
                        {
                            if (m_FSM.m_AIData.AtkReady && m_FSM.m_AIData.PlayerInAtkRange) //攻擊準備好且可以攻擊的話攻擊
                            {
                                StartTransition(Npc.Attack, 0);
                                return;
                            }
                            if (fTired >= 5f)
                            StartTransition(Npc.Confront, 0); //否則就對峙
                            return;
                        }
                    }
                }

                internal override void OnStateEnter()
                {
                }

                internal override void OnStateExit()
                {
                    fTired = 0f;
                    m_FSM.ResetTriggers();
                }

                internal override void OnStateRunning()
                {
                    base.OnStateRunning();
                    fTired += Time.deltaTime;
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
