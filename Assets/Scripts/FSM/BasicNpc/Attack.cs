﻿using UnityEngine;
using System;

namespace FSM
{
    namespace StatesLibrary
    {

        public partial class BasicNpc
        {
            /// <summary>
            /// 修正方向然後攻擊目標的行為(修正方向的Function在開關攻擊盒的事件做)
            /// </summary>
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
                    if (!m_FSM.m_AIData.IsInBattle) //戰鬥外，以後有時間也許可以做對空氣揮劍裝B的敵人
                    {

                    }
                    if (m_FSM.m_AIData.IsInBattle) //戰鬥中
                    {
                        if (m_FSM.m_AIData.PlayerStillInAtkRange()) //如果敵人還在攻擊範圍內就繼續攻擊
                        {
                            if (stage == 0)
                            {
                                m_FSM.StartCoroutine(TransferToSubState(1));
                                return;
                            }
                            if (stage == 1)
                            {
                                m_FSM.StartCoroutine(TransferToSubState(2));
                                return;
                            }
                        }

                        StartTransition(Npc.Confront);

                    }



                }

                internal override void OnStateEnter()
                {
                    base.OnStateEnter();
                    m_FSM.m_AIData.LookAtTargetDirectly();
                }

                internal override void OnStateExit()
                {
                    m_FSM.m_AIData.Attack();
                    m_FSM.m_AIData.JumpAttack();
                }

                internal override void OnStateRunning(int stage)
                {
                    base.OnStateRunning(stage);
                    if(m_FSM.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 ==0)
                    {
                        m_FSM.m_AIData.LookAtTargetDirectly();
                    }
                }

                protected internal override void OnAnimatorMove()
                {
                    m_FSM.m_Animator.ApplyBuiltinRootMotion();
                }
            }
        }
    }
}