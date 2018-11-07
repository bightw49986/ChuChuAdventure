using System;
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
        public Collider Collider;

        /// <summary>
        /// 這個受擊盒被打中時要播放的特效
        /// </summary>
        /// <value>特效的Prefab</value>
        [SerializeField] protected GameObject HitFX;

        /// <summary>
        /// 要不要印防禦盒Log
        /// </summary>
        [SerializeField] protected bool PrintLog;

        protected virtual void Awake()
        {
            if (Collider == null)
            {
                Collider = GetComponent<Collider>();
            }
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
                if (PrintLog)
                    print("防禦盒端: " + name + " 開啟成功，更新受傷類型(宿主: " + Host.name + ")");
            }
        }

        void OnDisable() //undone 把特效還給物件池
        {
        }


        public void InitDefendBox(IDefender host)
        {
            if (host != null)
            {
                Host = (BattleData)host;
                if (PrintLog)
                    print("防禦盒端: " + name + " 初始化成功(宿主: " + Host.name + ")");
            }

        }

        /// <summary>
        /// 根據攻擊盒那邊送來的傷害值計算傷害資料，判定最終扣血量
        /// </summary>
        /// <param name="fBaseDamage">傷害值</param>
        protected virtual float CalculateDamage(float fBaseDamage)
        {
            if (PrintLog)
                print("防禦盒端: " + name + " 計算傷害(宿主: " + Host.name + ")");
            //預設是直接傳攻擊盒那邊傳來的值，繼承的話可覆寫自己的運算邏輯
            return fBaseDamage;
        }

        /// <summary>
        /// 播擊中特效
        /// </summary>
        protected virtual void PlayHitFX() //undone 物件池處理、多特效支援
        {
            if (HitFX == null)
            {
                if (PrintLog)
                    Debug.LogError("特效未指派就被叫用！");
                return;
            }
            Instantiate(HitFX, transform.position, Quaternion.identity);
            if (PrintLog)
                print("防禦盒端: " + name + " 噴出受傷特效" + HitFX.name + "(宿主: " + Host.name + ")");
        }

        /// <summary>
        /// 傷害發生時觸發的事件，由攻擊盒觸發
        /// </summary>
        public event Action<float> DamageOccured;
        public virtual void OnDamageOccured(float fDamage)
        {
            PlayHitFX(); //播擊中特效
            if (DamageOccured != null)
            {
                DamageOccured(CalculateDamage(fDamage)); //把傷害值傳給CalculateDamage計算，然後通知事件的註冊者扣血量
                if (PrintLog)
                    print("防禦盒端: " + name + " 把傷害" + fDamage + "傳給宿主(宿主: " + Host.name + ")");
            }

        }
    }
}
