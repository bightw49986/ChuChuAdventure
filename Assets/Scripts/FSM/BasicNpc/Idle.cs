using System;

namespace FSM
{
    namespace StatesLibrary
    {
        public partial class BasicNpc
        {
            /// <summary>
            /// 站在原地不動的行為
            /// </summary>
            public class Idle : NpcSubMachine
            {
                NpcFSM.StartPose npcStartingPose;

                public override Enum StateID
                {
                    get
                    {
                        return Npc.Idle;
                    }
                }

                public Idle(NpcFSM fsm, NpcFSM.StartPose startingPose) : base(fsm) { npcStartingPose = startingPose; }

                protected override void AssignSubStatesTriggers()
                {
                    SubStatesTriggers = StatesLib.BasicNpc.SubStatesTriggers[Npc.Idle];
                }

                internal override void CheckConditions(int stage)
                {
                    if (!m_FSM.m_AIData.IsInBattle) //戰鬥外的初始狀態
                    {
                        if (stage == 0 || stage == 1 || stage == 2) //發呆
                        {
                            //如果敵人進入視線 Caution
                            if (m_FSM.m_AIData.PlayerShowedUp())
                            {
                                m_FSM.StartCoroutine(TransferToSubState(3));
                                return;
                            }
                            //如果敵人突然進入追擊範圍 Chase
                            if (m_FSM.m_AIData.PlayerInChaseRange())
                            {
                                m_FSM.m_AIData.EnterBattle();
                                StartTransition(Npc.Chase);
                                return;
                            }
                            //如果敵人突然超近 Confront
                            if (m_FSM.m_AIData.PlayerInJumpAtkRange())
                            {
                                m_FSM.m_AIData.EnterBattle();
                                StartTransition(Npc.Confront);
                                return;
                            }

                        }
                        if (stage == 3) //察覺有異，切Approach
                        {
                            StartTransition(Npc.Approach);
                            return;
                        }
                        if (stage == 4) //蹲著
                        {
                            if (m_FSM.m_AIData.PlayerInJumpAtkRange()) //如果敵人進入跳躍攻擊範圍，嚕下去
                            {
                                m_FSM.StartCoroutine(TransferToSubState(5));
                                return;
                            }
                        }
                        if (stage == 5) //站起
                        {
                            if (m_FSM.m_Animator.IsInTransition(0) == false)
                            {
                                m_FSM.m_AIData.EnterBattle();
                                m_FSM.m_AIData.JumpAttack();
                                StartTransition(Npc.Attack, 3);
                                return;
                            }
                        }
                    }
                    if (m_FSM.m_AIData.IsInBattle) //戰鬥中發呆
                    {
                        //undone 原地轉到面朝目標，攻擊冷卻到了就Chase或Approach，沒到就Confront，如果玩家死了就愣個幾秒回Patrol
                    }

                }

                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    switch (npcStartingPose)
                    {
                        case NpcFSM.StartPose.Stand:
                            {
                                int iPick = new Random().Next(0, 2);
                                m_FSM.m_Animator.SetTrigger(SubStatesTriggers[iPick]);
                                SubState = iPick;
                                break;
                            }

                        case NpcFSM.StartPose.Crouch:
                            {
                                m_FSM.m_Animator.SetTrigger(SubStatesTriggers[4]);
                                SubState = 4;
                                break;
                            }
                    }
                }

                internal override void OnStateExit()
                {

                }

                internal override void OnStateRunning(int stage)
                {
                    base.OnStateRunning(stage);
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
