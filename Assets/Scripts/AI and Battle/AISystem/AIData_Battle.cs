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
         /// 進入戰鬥，紀錄進入戰鬥前最後的位置
         /// </summary>
        public void EnterBattle()
        {
            IsInBattle = true;
            lastNodeBeforeBattle = aStarAgent.FindNearestNode(aStarAgent);
        }

        /// <summary>
        /// 攻擊，攻擊進入冷卻時間
        /// </summary>
        public void Attack()
        {
            StartCoroutine(AtkCD(fAttackFrequency));
        }

        /// <summary>
        /// 計算攻擊冷卻時間
        /// </summary>
        /// <returns>The cd.</returns>
        /// <param name="fCD">F cd.</param>
        IEnumerator AtkCD(float fCD)
        {
            AtkReady = false;
            yield return new WaitForSeconds(fCD);
            AtkReady = true;
        }

        /// <summary>
        /// 跳躍攻擊，攻擊跟跳躍攻擊進冷卻時間
        /// </summary>
        public void JumpAttack()
        {
            StartCoroutine(JumpAtkCD(fJumpAttackFrequency));
            StartCoroutine(AtkCD(fAttackFrequency));
        }

        /// <summary>
        /// 計算跳躍攻擊冷卻時間
        /// </summary>
        /// <returns>The atk cd.</returns>
        /// <param name="fCD">F cd.</param>
        IEnumerator JumpAtkCD(float fCD)
        {
            JumpAtkReady = false;
            yield return new WaitForSeconds(fCD);
            JumpAtkReady = true;
        }
    }
}