using System;

namespace FSM
{
    namespace StatesLibrary
    {
        public partial class BasicNpc
        {
            /// <summary>
            /// 和目標保持一定距離並等待攻擊時機的行為
            /// </summary>
            public class Confront : NpcSubMachine
            {
                bool bConfrontOnce;
                bool bStrafeOnce;
                Random random = new Random();
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Confront;
                    }
                }

                public Confront(NpcFSM fsm) : base(fsm) { }

                protected override void AssignSubStatesTriggers()
                {
                    SubStatesTriggers = StatesLib.BasicNpc.SubStatesTriggers[Npc.Confront];
                }

                internal override void CheckConditions(int stage)
                {
                    int pick = random.Next();
                    if (!m_FSM.m_AIData.IsInBattle)
                    {
                        //應該沒有戰鬥外
                    }
                    if (m_FSM.m_AIData.IsInBattle)
                    {
                        if (m_FSM.m_AIData.AtkReady || m_FSM.m_AIData.JumpAtkReady)
                        {
                            if (SubState != 0)
                            {
                                m_FSM.StartCoroutine(TransferToSubState(0));
                                return;
                            }
                            if (m_FSM.m_AIData.PlayerInAtkRange())
                            {
                                m_FSM.m_AIData.Attack();
                                StartTransition(Npc.Attack);
                                return;
                            }
                            switch (m_FSM.BehaviorStyle)
                            {
                                case NpcFSM.Style.Normal:
                                    if (pick % 2 == 0)
                                    {
                                        StartTransition(Npc.Chase);
                                    }
                                    else
                                    {
                                        StartTransition(Npc.Approach);
                                    }
                                    break;
                                case NpcFSM.Style.Sinister:
                                    StartTransition(Npc.Chase);
                                    break;
                                case NpcFSM.Style.Cautious:
                                    StartTransition(Npc.Approach);
                                    break;
                            }
                            return;
                        }
                        else
                        {
                            if (stage == 0)
                            {
                                if (!bConfrontOnce)
                                {
                                    if (m_FSM.m_AIData.PlayerInAtkRange())
                                    {
                                        m_FSM.StartCoroutine(TransferToSubState(2));
                                        return;
                                    }
                                    if (m_FSM.m_AIData.PlayerInJumpAtkRange())
                                    {
                                        m_FSM.StartCoroutine(TransferToSubState(1));
                                        return;
                                    }
                                }
                                if (!bStrafeOnce)
                                {
                                    if (m_FSM.m_AIData.PlayerOnRightSide())
                                    {
                                        m_FSM.StartCoroutine(TransferToSubState(3));
                                        return;
                                    }
                                    else
                                    {
                                        m_FSM.StartCoroutine(TransferToSubState(4));
                                        return;
                                    }
                                }

                                if (m_FSM.m_AIData.PlayerInChaseRange())
                                {
                                    switch (m_FSM.BehaviorStyle)
                                    {
                                        case NpcFSM.Style.Normal:
                                            if (pick % 2 == 0)
                                            {
                                                StartTransition(Npc.Chase);
                                            }
                                            else
                                            {
                                                StartTransition(Npc.Approach);
                                            }
                                            break;
                                        case NpcFSM.Style.Sinister:
                                            StartTransition(Npc.Chase);
                                            break;
                                        case NpcFSM.Style.Cautious:
                                            StartTransition(Npc.Approach);
                                            break;
                                    }
                                    return;
                                }
                            }
                            if (stage == 1 || stage == 2)
                            {
                                m_FSM.StartCoroutine(TransferToSubState(0));
                                bConfrontOnce = true;
                                return;
                            }
                            if (stage == 3 || stage == 4)
                            {
                                m_FSM.StartCoroutine(TransferToSubState(0));
                                bStrafeOnce = true;
                                return;
                            }
                            if (!m_FSM.m_AIData.PlayerInJumpAtkRange())
                            {
                                m_FSM.StartCoroutine(TransferToSubState(0));
                                StartTransition(Npc.Chase);
                            }
                        }
                    }

                   

                }


                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    bStrafeOnce = false;

                }

                internal override void OnStateExit()
                {
                    bStrafeOnce = false;
                }

                internal override void OnStateRunning(int stage)
                {
                    base.OnStateRunning(stage);
                }

                protected internal override void OnAnimatorMove()
                {
                    m_FSM.m_AIData.MoveToPlayer();
                }
            }
        }
    }
}
