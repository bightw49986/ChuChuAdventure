using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using PathFinding;
using System;

namespace AISystem
{
    public partial class AIData : MonoBehaviour
    {
        /// <summary>
        /// 更新玩家資訊
        /// </summary>
        void UpdatePlayerInfo()
        {
            m_vPlayerPos = player.Position; //找玩家位置
            vDirectionToPlayer = m_vPlayerPos - aStarAgent.Position; //算玩家的方向
            fSqrPlayerDis = Vector3.SqrMagnitude(vDirectionToPlayer); //算玩家的距離平方
            fPlayerDis = Mathf.Sqrt(fSqrPlayerDis); //算玩家的距離
        }

        /// <summary>
        /// 更新目的地資訊
        /// </summary>
        void UpdateDestinationInfo()
        {
            Vector3 vDestOnPlane = new Vector3(m_vDestination.x, 0, m_vDestination.z);
            Vector3 vPosOnPlane = new Vector3(aStarAgent.Position.x, 0, aStarAgent.Position.z);
            vDirectionToDest = vDestOnPlane - vPosOnPlane; //算目的地方向
            m_fSqrDistToDest = Vector3.SqrMagnitude(vDirectionToDest); //算目的距離平方
            m_fDistToDest = Mathf.Sqrt(m_fSqrDistToDest); //算目的地的距離
            m_fDestDot = Mathf.Clamp01(Vector3.Dot(transform.forward, vDirectionToDest.normalized)); //算目的地方向相對於角色的偏離方向
            m_fAbsDestDot = Mathf.Abs(m_fDestDot); //算偏離程度
            fTurnWeight = 1 - m_fAbsDestDot; //根據偏離程度決定轉向動畫權重
            fSqrTurnWeight = Mathf.Clamp01(fTurnWeight * fTurnWeight); //把權重平滑化
            m_FSM.m_Animator.SetFloat("fTurn", fSqrTurnWeight); //Set給animator
        }

        /// <summary>
        /// 更新膠囊探針資訊
        /// </summary>
        void UpdateProbe()
        {
            vCenter = transform.position + Vector3.up * fHeight * 0.5f;
            vCPoint1 = vCenter + Vector3.up * 0.15f;
            vCPoint2 = vCenter + Vector3.down * 0.15f;
        }

        void MoveToDestination()
        {
            switch (Destination)
            {
                case DestinationState.None:
                    {
                        m_FSM.m_Animator.ApplyBuiltinRootMotion();
                        return;
                    }

                case DestinationState.Player:
                    {
                        MoveToPlayer();
                        break;
                    }

                case DestinationState.BackToIdle:
                    {
                        MoveBackToIdle();
                        break;
                    }

                case DestinationState.Patrol:
                    {
                        Patrol();
                        break;
                    }
            }
            UpdateDestinationInfo();
            SteerToDestination();
        }

        void Patrol()
        {
            path = aStarAgent.GetPath(aStarAgent, nextNode);
            FollowPath();
        }

        void MoveBackToIdle()
        {
            path = aStarAgent.GetPath(aStarAgent, lastNodeBeforeBattle);
            FollowPath();
        }

        /// <summary>
        /// 朝玩家移動
        /// </summary>
        public void MoveToPlayer()
        {
            path = aStarAgent.GetPath(aStarAgent, player);
            if (CheckIfBlocked(player.transform.position) == false)//檢查與玩家之間有無障礙物
            {
                m_vDestination = player.transform.position;//無的話，把Destination設成玩家的位置
            }
            else
            {
                FollowPath();
            }
        }

        void FollowPath()
        {
            for (int i = path.Count - 1; i > 1; i--)
            {
                if (CheckIfBlocked(path[i]) == true) continue;
                m_vDestination = path[i];
            }
        }

        bool CheckIfBlocked(Vector3 vTargetPos)
        {
            vTargetPos.y = 0f;
            Vector3 vStart = transform.position;
            vStart.y = 0f;
            Vector3 vDir = vTargetPos - vStart;
            float fDis = Vector3.Magnitude(vDir);
            RaycastHit[] hits;
            hits = Physics.CapsuleCastAll(vCPoint1, vCPoint2, fWidth, vDir, fDis, CollisionLayer);
            return hits.Length != 0;
        }

        /// <summary>
        /// 轉向至目標
        /// </summary>
        void SteerToDestination()
        {
            Quaternion qDesireRotation = Quaternion.identity;
            Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            if (vDirectionToDest != Vector3.zero && forwardOnPlane != vDirectionToDest)
            {
                qDesireRotation = Quaternion.LookRotation(vDirectionToDest, Vector3.up);
            }
            else
            {
                qDesireRotation = m_FSM.m_Animator.rootRotation;
            }
            transform.position = m_FSM.m_Animator.rootPosition;
            transform.rotation = qDesireRotation;
        }

        /// <summary>
        /// 不許播腳步旋轉動畫
        /// </summary>
        public void StopTurnning()
        {
            m_FSM.m_Animator.SetLayerWeight(1, 0);
        }

        /// <summary>
        /// 允許播腳部旋轉動畫
        /// </summary>
        public void StartTurnning()
        {
            m_FSM.m_Animator.SetLayerWeight(1, 1);
        }

        /// <summary>
        /// 瞬間轉向player的位置，角度受到m_fMaxTurnDegree限制
        /// </summary>
        public void LookAtPlayerDirectly()
        {
            float fAngle = AIMethod.FindAngle(aStarAgent.Position, m_vPlayerPos, transform.up);
            fAngle = Mathf.Clamp(fAngle, -fMaxTurnDegree, fMaxTurnDegree);
            Quaternion qDesireRotation = Quaternion.AngleAxis(fAngle, Vector3.up) * transform.rotation;
            transform.rotation = qDesireRotation;
        }


        Coroutine ASTAR;
        List<Vector3> path = new List<Vector3>();
        bool bAstaring;
    }
}