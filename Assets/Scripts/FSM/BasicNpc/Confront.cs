using System;
using UnityEngine;

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
                float fTired;
                bool bConfrontOnce;
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
                    if (!m_FSM.m_AIData.IsInBattle)
                    {
                        //應該沒有戰鬥外
                    }
                    if (m_FSM.m_AIData.IsInBattle) //戰鬥中
                    {
                        if (fTired >= 5f)
                        {
                            m_FSM.m_AIData.AtkReady = true;
                        }

                        if (m_FSM.m_AIData.PlayerInJumpAtkRange() == false) //超過跳躍攻擊的距離的話，追上去
                        {
                            if (SubState != 0) //如果不在標準動作(表示仍在移動)
                            {
                                m_FSM.StartCoroutine(TransferToSubState(0)); //先站好
                                return;
                            }
                            StartTransition(Npc.Chase);
                            return;
                        }
                        else // 還在一定距離的話
                        {
                            if (m_FSM.m_AIData.AtkReady) //如果攻擊或跳躍攻擊準備好
                            {
                                if (SubState != 0) //如果不在標準動作(表示仍在移動)
                                {
                                    m_FSM.StartCoroutine(TransferToSubState(0)); //先站好
                                    return;
                                }
                                if (m_FSM.m_AIData.PlayerInAtkRange()) //若敵人已經在攻擊範圍內，進攻擊
                                {
                                    StartTransition(Npc.Attack);
                                    return;
                                }
                                switch (m_FSM.BehaviorStyle) //不在攻擊範圍內的話，根據Npc的行為風格決定要衝向玩家還是走向玩家
                                {
                                    case NpcFSM.Style.Normal: //一般的話，隨機二選一
                                        if (pick % 2 == 0)
                                        {
                                            StartTransition(Npc.Chase);
                                        }
                                        else
                                        {
                                            StartTransition(Npc.Approach);
                                        }
                                        break;
                                    case NpcFSM.Style.Sinister: //狡詐型的敵人衝
                                        StartTransition(Npc.Chase);
                                        break;
                                    case NpcFSM.Style.Cautious: //謹慎型的敵人走
                                        StartTransition(Npc.Approach);
                                        break;
                                }
                                return;
                            }
                            else
                            {
                                if (stage == 0) //攻擊還沒準備好的情況下
                                {
                                    if (!bConfrontOnce) //如果還沒前後拉個距離，就先前後走一次
                                    {
                                        if (m_FSM.m_AIData.PlayerInAtkRange()) //比較近，就退後
                                        {
                                            m_FSM.StartCoroutine(TransferToSubState(2));
                                            return;
                                        }
                                        if (m_FSM.m_AIData.PlayerInJumpAtkRange())//比較遠，就前進
                                        {
                                            m_FSM.StartCoroutine(TransferToSubState(1));
                                            return;
                                        }
                                    }
                                    if (!bStrafeOnce) //如果還沒左右繞一次的話，繞一次
                                    {
                                        if (m_FSM.m_AIData.PlayerOnRightSide()) //在右邊，向左轉
                                        {
                                            m_FSM.StartCoroutine(TransferToSubState(3));
                                            return;
                                        }
                                        else //在左邊，向右轉
                                        {
                                            m_FSM.StartCoroutine(TransferToSubState(4));
                                            return;
                                        }
                                    }
                                    if (m_FSM.m_AIData.PlayerInChaseRange()) //都做過的話，根據行為決定要靠近還是再繞一次
                                    {
                                        switch (m_FSM.BehaviorStyle)
                                        {
                                            case NpcFSM.Style.Normal: //一般的情況前後左右四選一
                                                if (pick % 2 == 0)
                                                {
                                                    if (m_FSM.m_AIData.PlayerInAtkRange()) //比較近，就退後
                                                    {
                                                        m_FSM.StartCoroutine(TransferToSubState(2));
                                                        return;
                                                    }
                                                    if (m_FSM.m_AIData.PlayerInJumpAtkRange())//比較遠，就前進
                                                    {
                                                        m_FSM.StartCoroutine(TransferToSubState(1));
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (m_FSM.m_AIData.PlayerOnRightSide()) //在右邊，向左轉
                                                    {
                                                        m_FSM.StartCoroutine(TransferToSubState(3));
                                                        return;
                                                    }
                                                    else //在左邊，向右轉
                                                    {
                                                        m_FSM.StartCoroutine(TransferToSubState(4));
                                                        return;
                                                    }
                                                }
                                                break;
                                            case NpcFSM.Style.Sinister: //狡猾的敵人會靠近
                                                m_FSM.StartCoroutine(TransferToSubState(1));
                                                break;
                                            case NpcFSM.Style.Cautious: //謹慎的敵人會往後
                                                m_FSM.StartCoroutine(TransferToSubState(2));
                                                break;
                                        }
                                        return;
                                    }
                                }
                                if (stage == 1 || stage == 2) //正在前進或後退
                                {
                                    m_FSM.StartCoroutine(TransferToSubState(0)); //做完停下來
                                    bConfrontOnce = true;
                                    return;
                                }
                                if (stage == 3 || stage == 4) //正在左右轉
                                {
                                    m_FSM.StartCoroutine(TransferToSubState(0)); //做完停下來
                                    bStrafeOnce = true;
                                    return;
                                }
                            }
                        }

                    }
                }


                internal override void OnStateEnter()
                {
                    bConfrontOnce = bStrafeOnce = false;
                }

                internal override void OnStateExit()
                {
                    fTired = 0f;
                    bConfrontOnce = bStrafeOnce = false;
                    m_FSM.ResetTriggers();
                }

                internal override void OnStateRunning(int stage)
                {
                    fTired += Time.deltaTime;
                    Debug.Log("bConfrontOnce: " + bConfrontOnce);
                    Debug.Log("bStrafeOnce" + bStrafeOnce);
                }

                protected internal override void OnAnimatorMove()
                {
                    m_FSM.m_AIData.MoveToPlayer();
                }
            }
        }
    }
}
