using System;
using AISystem;

namespace FSM
{
    namespace StatesLibrary
    {

        public partial class BasicNpc
        {
            /// <summary>
            /// 快速追向目標並避開障礙物的行為
            /// </summary>
            public class Chase : NpcFSMState
            {
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Chase;
                    }
                }

                public Chase(NpcFSM fsm) : base(fsm) { }



                internal override void CheckConditions()
                {

                    if (!m_FSM.m_AIData.IsInBattle)
                    {
                        //應該沒有戰鬥外
                    }
                    if (m_FSM.m_AIData.IsInBattle)//戰鬥中
                    {
                        if (m_FSM.m_AIData.PlayerInBattleRange==false) //如果敵人超過最大距離，發呆看著敵人離去的方向
                        {
                            //StartTransition(Npc.Idle);
                            //return;
                        }
                        else
                        {
                            if (m_FSM.m_AIData.PlayerInJumpAtkRange) //如果進到撲擊的範圍內
                            {
                                if (m_FSM.m_AIData.JumpAtkReady == false && m_FSM.m_AIData.AtkReady == false) //攻擊跟撲擊的冷卻都沒到就改成靠近
                                {
                                    StartTransition(Npc.Approach);
                                    return;
                                }

                                if (m_FSM.m_AIData.JumpAtkReady) //如果跳躍準備好了就檢查能不能跳躍攻擊，可以就跳 undone： 檢查可不可以跳
                                {
                                    StartTransition(Npc.Attack, 3);
                                    return;
                                }
                                if (m_FSM.m_AIData.PlayerInAtkRange) //如果進到攻擊範圍內了
                                {
                                    if (m_FSM.m_AIData.AtkReady) //且攻擊準備好 undone：檢查有沒有面向玩家
                                    {
                                        StartTransition(Npc.Attack, 0);
                                        return;
                                    }
                                    else
                                    {
                                        StartTransition(Npc.Confront);
                                    }
                                }
                            }
                        }
                    }
                }

                internal override void OnStateEnter()
                {
                    m_FSM.m_AIData.Destination = AIData.DestinationState.Player;
                }

                internal override void OnStateExit()
                {
                    base.OnStateExit();
                }

                internal override void OnStateRunning()
                {
                    base.OnStateRunning();
                    //如果與玩家之間有障礙物，算出沒有障礙物的點，把目標位置改成那個點
                }
            }
        }
    }
}
