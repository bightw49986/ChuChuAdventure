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
        /// 初始化AI Data
        /// </summary>
        void InitAIData()
        {
            m_FSM = GetComponent<NpcFSM>(); if (!m_FSM) Debug.LogError("沒有FSM");
            aStarAgent = GetComponent<AStarAgent>(); if (!aStarAgent) Debug.LogError("沒有a star agent");
            InitAIStats();
            FetchPlayerData();
            OnAIDataInitialized();
        }

        /// <summary>
        /// 初始化玩家的資料
        /// </summary>
        void FetchPlayerData()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            player.Died += () => { PlayerIsDead = true; };
            UpdatePlayerInfo();
            UpdateProbe();
        }

        /// <summary>
        /// 當AI Data初始完觸發的事件
        /// </summary>
        public event Action AIDataInitialized;

        /// <summary>
        /// 當AI Data初始完觸發事件
        /// </summary>
        protected virtual void OnAIDataInitialized()
        {
            if (AIDataInitialized != null) AIDataInitialized();
        }

        /// <summary>
        /// 初始化數值
        /// </summary>
        void InitAIStats()
        {
            InitRangeStats();
            InitMoveStats();
        }

        /// <summary>
        /// 初始化範圍數值，把該乘的乘一乘
        /// </summary>
        void InitRangeStats()
        {
            m_vDestination = vDirectionToDest = m_vDestLastFrame = Vector3.zero;
            fSqrFaceCautionRange = fFaceCautionRange * fFaceCautionRange;
            fSqrBackCautionRange = fBackCautionRange * fBackCautionRange;
            fSqrChaseRange = fChaseRange * fChaseRange;
            fSqrJumpAtkRange = fJumpAtkRange * fJumpAtkRange;
            fSqrAtkRange = fAtkRange * fAtkRange;
            fSqrLimitRange = fLimitRange * fLimitRange;
        }

        /// <summary>
        /// 初始化移動數據，把該乘的乘一乘
        /// </summary>
        void InitMoveStats()
        {
            UpdateProbe();
            fMaxSpeed *= Time.deltaTime;
            fSqrMaxSpped = fMaxSpeed * fMaxSpeed;
            fMaxSteerSpeed *= Time.deltaTime;
            fSqrMaxSteerSpeed = fMaxSteerSpeed * fMaxSteerSpeed;
        }
    }
}