using System;

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
                    if (m_FSM.m_AIData.IsInBattle)
                    {

                    }

                    if (m_FSM.m_AIData.IsInBattle == false)
                    {
                        //JumpAtk
                        if (m_FSM.m_AIData.PlayerInJumpAtkRange() && m_FSM.m_AIData.JumpAtkReady)
                        {
                            m_FSM.m_AIData.JumpAttack();
                            StartTransition(Npc.Attack, 3);
                            return;
                        }
                    }

                    else
                    {
                        //JumpAtk
                        if (m_FSM.m_AIData.PlayerInJumpAtkRange() && m_FSM.m_AIData.JumpAtkReady)
                        {
                            m_FSM.m_AIData.JumpAttack();
                            StartTransition(Npc.Attack, 3);
                            return;
                        }

                        //Atk
                        if (m_FSM.m_AIData.PlayerInAtkRange() && m_FSM.m_AIData.AtkReady)
                        {
                            m_FSM.m_AIData.Attack();
                            StartTransition(Npc.Attack, 0);
                            return;
                        }

                        StartTransition(Npc.Confront);
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
