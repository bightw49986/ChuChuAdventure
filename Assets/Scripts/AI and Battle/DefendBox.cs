using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    /// <summary>
    /// 定義要使用受擊盒的角色所需的資料
    /// </summary>
    public interface IDefender
    {
        List<DefendBox> DefendBoxes { get; set; }
        HashSet<EAttackerType> TakeDamageFrom { get; set; }
        float MaxHP { get; set; }
        float HP { get; set; }
        float MaxToughness { get; set; }
        float Toughness { get; set; }
        float RecoverTime { get; set; }
        bool IsHarmless { get; set; }

        void InitDefendBoxs();
    }


    /// <summary>
    /// 防禦(受擊)盒的基本類別
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DefendBox : MonoBehaviour
    {
        /// <summary>
        /// 這個受擊盒的主人
        /// </summary>
        /// <value>受擊盒的擁有者</value>
        public IDefender Host { get; private set; }

        /// <summary>
        /// 這個受擊盒要接收哪些種類的傷害
        /// </summary>
        /// <value>The type of the take damage.</value>
        public HashSet<EAttackerType> TakeDamageType { get; set; }

        /// <summary>
        /// 這個受擊盒的Collider
        /// </summary>
        /// <value>Collider</value>
        public Collider Collider { get; private set; }



        /// <summary>
        /// 啟動時，初始化受擊資訊
        /// </summary>
        void OnEnable()
        {
            TakeDamageType = Host.TakeDamageFrom;
        }


        /// <summary>
        /// 根據攻擊盒那邊送來的傷害值計算傷害資料，判定傷害事件發生
        /// </summary>
        /// <param name="fDamage">傷害值</param>
        public void CalculateDamage(float fDamage)
        {

        }

        /// <summary>
        /// 受到傷害但沒發生硬直的事件
        /// </summary>
        public event Action DamageTaken;
        public virtual void OnDamageTaken()
        {
            if (DamageTaken != null) DamageTaken();
        }

        /// <summary>
        /// 受到傷害且強韌度不足，導致硬直發生的事件
        /// </summary>
        public event Action DamageTakenFreezed;
        public virtual void OnDamageTakenFreezed()
        {
            if (DamageTakenFreezed != null) DamageTakenFreezed();
        }

        /// <summary>
        /// 受到傷害且HP不足，導致宿主死亡的事件
        /// </summary>
        public event Action DamageTakenDied;
        public virtual void OnDamageTakenDied()
        {
            if (DamageTakenDied != null) DamageTakenDied();
        }
    }
}
