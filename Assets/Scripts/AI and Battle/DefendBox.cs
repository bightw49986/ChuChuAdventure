using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
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
        public BattleData Host { get; private set; }

        /// <summary>
        /// 這個受擊盒要接收哪些種類的傷害
        /// </summary>
        /// <value>The type of the take damage.</value>
        public List<EAttackerType> TakeDamageType { get; set; }

        /// <summary>
        /// 這個受擊盒的Collider
        /// </summary>
        /// <value>Collider</value>
        public Collider Collider { get; private set; }

        /// <summary>
        /// 這個受擊盒被打中時要播放的特效
        /// </summary>
        /// <value>特效的Prefab</value>
        [SerializeField] protected GameObject HitFX { get; set; }

        protected virtual void Awake()
        {
            Collider = GetComponent<Collider>();
            enabled = false;
        }

        /// <summary>
        /// 啟動時，初始化受擊資訊
        /// </summary>
        void OnEnable() //undone 跟物件池要特效備用
        {
            if (Host != null)
            {
                TakeDamageType = Host.TakeDamageFrom;
            }
        }

        void OnDisable() //undone 把特效還給物件池
        {

        }

        public void InitDefendBox(IDefender host)
        {
            if (host != null)
            Host = (BattleData)host;
        }

        /// <summary>
        /// 根據攻擊盒那邊送來的傷害值計算傷害資料，判定最終扣血量
        /// </summary>
        /// <param name="fBaseDamage">傷害值</param>
        protected virtual float CalculateDamage(float fBaseDamage)
        {
            //預設是直接傳攻擊盒那邊傳來的值，繼承的話可覆寫自己的運算邏輯
            return fBaseDamage;
        }

        /// <summary>
        /// 播擊中特效
        /// </summary>
        protected virtual void PlayHitFX() //undone 物件池處理、多特效支援
        {
            Instantiate(HitFX, transform.position, Quaternion.identity);
        }

        /// <summary>
        /// 傷害發生時觸發的事件，由攻擊盒觸發
        /// </summary>
        public event Action<float> DamageOccured;
        public virtual void OnDamageOccured(float fDamage)
        {
            //PlayHitFX(); //播擊中特效
            if (DamageOccured != null)
                DamageOccured(CalculateDamage(fDamage)); //把傷害值傳給CalculateDamage計算，然後通知事件的註冊者扣血量
        }
    }
}
