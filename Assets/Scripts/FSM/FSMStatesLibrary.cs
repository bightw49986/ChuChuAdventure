using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem;
using FSM;
using BattleSystem;
using System;

namespace FSM
{
    /// <summary>
    /// 使用狀態機的物種
    /// </summary>
    public enum Species { Goblin = 0 }

    /// <summary>
    /// Npc的狀態行為
    /// </summary>
    public enum Npc { Freezed = -2 , Died =  -1 , Idle = 0, Patrol = 1, Chase  = 2, Confront = 3, Attack = 4 , Approach = 5 }



    namespace StatesLibrary
    {
        #region StatesLib
        public static class StatesLib
        {
            public static class BasicNpc
            {
                public static readonly List<Enum> AnyStates = new List<Enum> { Npc.Freezed, Npc.Died };

                public static readonly List<Enum> States = new List<Enum>
                { Npc.Idle, Npc.Patrol, Npc.Chase, Npc.Confront, Npc.Attack, Npc.Approach };

                public static readonly Dictionary<Enum, string> Triggers = new Dictionary<Enum, string>
                {
                    { Npc.Died,"Died" }, { Npc.Freezed, "Freezed"},{ Npc.Idle, "Idle0" }, { Npc.Patrol, "Patrol"}, { Npc.Chase, "Chase"},{ Npc.Approach, "Approach"}, { Npc.Attack, "Attack0" }, {Npc.Confront, "Confront"}
                };

                public static readonly Dictionary<Enum, List<Enum>> Transitions = new Dictionary<Enum, List<Enum>>
                {
                    {Npc.Died,new List<Enum>()},
                    {Npc.Freezed,new List<Enum>{Npc.Idle, Npc.Patrol, Npc.Chase, Npc.Confront, Npc.Attack, Npc.Approach}},
                    {Npc.Idle,new List<Enum>{ Npc.Idle,Npc.Approach,Npc.Patrol,Npc.Chase,Npc.Confront,Npc.Attack}},
                    {Npc.Patrol,new List<Enum>{Npc.Idle,Npc.Confront,Npc.Chase}},
                    {Npc.Approach,new List<Enum>{Npc.Idle,Npc.Chase,Npc.Attack}},
                    {Npc.Chase,new List<Enum>{Npc.Idle,Npc.Attack}},
                    {Npc.Attack,new List<Enum>{Npc.Idle,Npc.Confront,Npc.Approach}},
                    {Npc.Confront,new List<Enum>{Npc.Idle,Npc.Chase,Npc.Attack,Npc.Approach}}
                };

                public static readonly Dictionary<Enum, Dictionary<int, string>> SubStatesTriggers = new Dictionary<Enum, Dictionary<int, string>>
                {
                    { Npc.Idle, new Dictionary<int, string> { { 0, "Idle0" }, { 1, "Idle1" }, { 2, "Idle2" }, { 3, "Caution" }, { 4, "Ambush" }, { 5, "StandUp"} } },
                    { Npc.Attack, new Dictionary<int, string> { { 0, "Attack0" }, { 1, "Attack1" }, { 2, "Attack2" }, { 3, "JumpAttack" } } },
                    { Npc.Confront, new Dictionary<int, string> { { 0, "Confront"}, { 1, "CloseIn" }, { 2, "Backward" }, { 3, "StrafeLeft" }, { 4, "StrafeRight" } } }
                };
            }
        }
        #endregion

        #region Basic Npc States (Order by ID)

        public class BasicNpc
        {
            #region Died
            public class Died : NpcFSMState
            {
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Died;
                    }
                }

                public Died(NpcFSM fsm) : base(fsm) { }

                internal override void CheckConditions()
                {

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

            }
            #endregion

            #region Freezed
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
                    float i = m_FSM.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    if (i>0 && i%1 == 0f )
                    {

                    }
                        
                }

                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    m_FSM.m_isFreezed = m_FSM.m_isKOed = m_FSM.m_isDead = false;
                }

                internal override void OnStateExit()
                {

                }

                internal override void OnStateRunning()
                {
                    base.OnStateRunning();
                }

            }
            #endregion

            #region Idle
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

                public Idle(NpcFSM fsm, NpcFSM.StartPose startingPose) : base(fsm) { npcStartingPose = startingPose;}

                protected override void AssignSubStatesTriggers()
                {
                    SubStatesTriggers = StatesLib.BasicNpc.SubStatesTriggers[Npc.Idle];
                }

                internal override void CheckConditions(int stage)
                {

                    if (stage == 0 || stage == 1 || stage == 2) //發呆
                    {
                        //如果敵人進入視線 Caution
                        if (m_FSM.m_AIData.SomethingShowUp()) //fSqrPlayerDis < m_fSqrBackCaurionRange)
                        {
                            m_FSM.StartCoroutine(TransferToSubState(3));
                            return;
                        }
                        //如果敵人突然進入追擊範圍 Chase
                        if (m_FSM.m_AIData.TargetInChaseRange())
                        {
                            StartTransition(Npc.Chase);
                            return;
                        }
                        //如果敵人突然超近 Confront
                        if (m_FSM.m_AIData.TargetInJumpAtkRange())
                        {
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
                        if (m_FSM.m_AIData.TargetInJumpAtkRange()) //如果敵人進入跳躍攻擊範圍，嚕下去
                        {
                            m_FSM.StartCoroutine(TransferToSubState(5));
                            return;
                        }
                    }
                    if (stage == 5) //站起
                    {
                        if (m_FSM.m_Animator.IsInTransition(0)==false)
                        {
                            m_FSM.m_AIData.JumpAttack();
                            StartTransition(Npc.Attack, 3);
                            return;
                        }
                    }
                }

                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    switch (npcStartingPose)
                    {
                        case NpcFSM.StartPose.Stand:
                            {
                                int iPick = new System.Random().Next(0, 2);
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
            }
            #endregion

            #region Patrol
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
                    m_FSM.m_AIData.TurnToTarget();
                }
            }
            #endregion

            #region Chase
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


                    //JumpAtk
                    if (m_FSM.m_AIData.TargetInJumpAtkRange() && m_FSM.m_AIData.JumpAtkReady)
                    {
                        m_FSM.m_AIData.JumpAttack();
                        StartTransition(Npc.Attack, 3);
                        return;
                    }

                    //Atk
                    if(m_FSM.m_AIData.TargetInAtkRange() && m_FSM.m_AIData.AtkReady)
                    {
                        m_FSM.m_AIData.Attack();
                        StartTransition(Npc.Attack, 0);
                        return;
                    }
                }

                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    m_FSM.m_AIData.PlayerInSight = true;

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
                    m_FSM.m_AIData.TurnToTarget();
                }
            }
            #endregion

            #region Attack
            public class Attack : NpcSubMachine
            {
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Attack;
                    }
                }

                public Attack(NpcFSM fsm) : base(fsm) { }

                protected override void AssignSubStatesTriggers()
                {
                    SubStatesTriggers = StatesLib.BasicNpc.SubStatesTriggers[Npc.Attack];
                }

                internal override void CheckConditions(int stage)
                {
                    if (m_FSM.m_AIData.TargetStillInAtkRange())
                    {
                        if (stage == 0)
                        {
                            m_FSM.StartCoroutine(TransferToSubState(1));
                        }
                        if (stage == 1)
                        {
                            m_FSM.StartCoroutine(TransferToSubState(2));
                        }
                    }
                    else
                    {
                        Debug.Log("有進");
                        StartTransition(Npc.Confront);
                    }

                }

                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    m_FSM.m_AIData.PlayerInSight = true;
                    m_FSM.m_AIData.StopTurnToTarget();

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

                }
            }
            #endregion

            #region Confront
            public class Confront : NpcSubMachine
            {
                bool bStrafeOnce;
                System.Random random = new System.Random();
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
                    if (m_FSM.m_AIData.AtkReady || m_FSM.m_AIData.JumpAtkReady)
                    {
                        m_FSM.StartCoroutine(TransferToSubState(0));
                        if (m_FSM.m_AIData.TargetInAtkRange())
                        {
                            StartTransition(Npc.Attack);
                            return;
                        }
                        switch (m_FSM.BehaviorStyle)
                        {
                            case NpcFSM.Style.Normal:
                                Debug.Log("有進");
                                if (pick%2==0)
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
                    if (stage ==0)
                    {
                        if (bStrafeOnce)
                        {
                            if (m_FSM.m_AIData.TargetInAtkRange())
                            {
                                m_FSM.StartCoroutine(TransferToSubState(2));
                                return;
                            }
                            if (m_FSM.m_AIData.TargetInJumpAtkRange())
                            {
                                m_FSM.StartCoroutine(TransferToSubState(1));
                                return;
                            }
                        }

                        if (m_FSM.m_AIData.TargetInChaseRange())
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
                    if (stage ==1 || stage ==2)
                    {
                        if (bStrafeOnce)
                        {
                            m_FSM.StartCoroutine(TransferToSubState(0));
                            return;
                        }
                        if (m_FSM.m_AIData.TargetInJumpAtkRange())
                        {
                            if (m_FSM.m_AIData.TargetOnRightSide())
                            m_FSM.StartCoroutine(TransferToSubState(3));
                            else
                                m_FSM.StartCoroutine(TransferToSubState(4));
                            return;
                        }
                    }
                    if (stage==3 || stage ==4)
                    {
                        m_FSM.StartCoroutine(TransferToSubState(0));
                        bStrafeOnce = true;
                        return;
                    }

                    if (!m_FSM.m_AIData.TargetInChaseRange())
                    {
                        m_FSM.StartCoroutine(TransferToSubState(0));
                        StartTransition(Npc.Chase);
                    }
                }

                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    m_FSM.m_AIData.PlayerInSight = true;
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
                    m_FSM.m_AIData.LookAtTarget();
                }
            }
            #endregion

            #region Approach
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
                    //敵人第一次進追擊範圍 Chase
                    if (m_FSM.m_AIData.PlayerInSight ==false && m_FSM.m_AIData.TargetInChaseRange())
                    {
                        StartTransition(Npc.Chase);
                        return;
                    }
                    //attack
                    if (m_FSM.m_AIData.PlayerInSight ==true && m_FSM.m_AIData.TargetInAtkRange())
                    {
                        m_FSM.m_AIData.Attack();
                        StartTransition(Npc.Attack,0);
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
                    m_FSM.m_AIData.TurnToTarget();
                }
            }
            #endregion
        }
        #endregion
    }


}
