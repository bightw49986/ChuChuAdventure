﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BattleSystem
{
    /// <summary>
    /// 攻擊者的分類枚舉，0是玩家，1是獸人、哥布林、胖子，2是樹人跟石頭以及魔王
    /// </summary>
    public enum EAttackerType {Player = 0, MonsterGroup1 = 1, MonsterGroup2 = 2 }


    /// <summary>
    /// 定義要使用基本開關式攻擊盒(近戰)的角色所需的資料
    /// </summary>
    public interface IAttacker
    {
        /// <summary>
        /// 這個攻擊者的物件
        /// </summary>
        /// <value>GameObject</value>
        GameObject GO { get; set; }

        /// <summary>
        /// 這個攻擊者身上所有的攻擊盒
        /// </summary>
        /// <value>攻擊盒們</value>
        List<AttackBox> AttackBoxes { get; set; }

        /// <summary>
        /// 這個攻擊者身上每個攻擊盒對應的攻擊力
        /// </summary>
        /// <value>攻擊力</value>
        Dictionary<AttackBox, float> AtkValues { get; set; }

        /// <summary>
        /// 這個攻擊者的攻擊者分類
        /// </summary>
        /// <value>攻擊者分類(0是玩家，1是獸人、哥布林、胖子，2是石頭樹人跟Boss)</value>
        EAttackerType AttackerType { get; set; }

        /// <summary>
        /// 初始化身上每一個攻擊盒，實作上應在子物件中找到他們的reference，呼叫他們的InitAttackBox(this)，然後把它們加到AttackBoxes
        /// </summary>
        void InitAttackBoxes();

        /// <summary>
        /// 當AtkValue被Set時觸發事件，通知所有註冊的攻擊盒更新資訊
        /// </summary>
        event Action<float,AttackBox> AttackInfoUpdate;

        /// <summary>
        /// 應該被實作成AttackInfoUpdate的觸發器，並且塞在SetAtkValueToAttackBox裡面觸發
        /// </summary>
        /// <param name="fNewAttackValue">新的攻擊力</param>
        void OnAttackInfoUpdate(float fNewAttackValue, AttackBox attackBox);

        /// <summary>
        /// 給指定的攻擊盒設定新的攻擊力(實作記得防呆)
        /// </summary>
        /// <param name="fNewAttackValue">新的攻擊力</param>
        /// <param name="attackBox">要指定的攻擊盒</param>
        void SetAtkValueToAttackBox(float fNewAttackValue, AttackBox attackBox);

        /// <summary>
        /// 打開指定索引的攻擊盒
        /// </summary>
        /// <param name="iAttackBoxIndex">攻擊盒在AttackBoxes裡的索引</param>
        void EnableAttackBox(int iAttackBoxIndex);

        /// <summary>
        /// 關閉指定索引的攻擊盒
        /// </summary>
        /// <param name="iAttackBoxIndex">攻擊盒在AttackBoxes裡的索引</param>
        void DisableAttackBox(int iAttackBoxIndex);
    }


    /// <summary>
    /// 基本攻擊盒，運作方式為狀態機或其他外部腳本控制Enalbe開關
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AttackBox : MonoBehaviour
    {
        /// <summary>
        /// 這個攻擊盒的主人，從它註冊AttackInfoUpdate事件檢查攻擊力的變動
        /// </summary>
        /// <value>攻擊盒的擁有者</value>
        protected IAttacker Host { get;private set; }

        /// <summary>
        /// 這一下攻擊會造成多少傷害，在繼承的情況下覆寫這個屬性，可以在Getter那邊和CalculateFinalDamage()搭配，增加額外的傷害計算邏輯(先往下看，CalculateFinalDamage那邊有更多註解)
        /// </summary>
        /// <value>傷害值</value>
        protected virtual float DamageThisHit { get; set; }

        /// <summary>
        /// 這個攻擊盒的Collider
        /// </summary>
        /// <value>Collider</value>
        protected Collider Collider { get;private set; }

        /// <summary>
        /// 記錄這次攻擊已擊中的防禦盒，避免重複判定
        /// </summary>
        /// <value>被打中的盒子們</value>
        protected List<DefendBox> HittenBoxes { get; set; }

        /// <summary>
        /// 定義若允許在一次開關之間重複判定的判定間隔秒數(在get時已自動轉為秒數，無需乘上Time.deltaTime)
        /// </summary>
        /// <value>間隔秒數，若 = 0 代表不允許重複判定</value>
        protected float Interval { get { return m_fInterval * Time.deltaTime; } set { m_fInterval = value; } }
        private float m_fInterval;

        protected virtual void Awake()
        {
            Collider = GetComponent<Collider>();
        }

        /// <summary>
        /// 外部初始化攻擊盒所要呼叫的函式，宿主必須在初始化每一個攻擊盒時都將自己當作參數傳給攻擊盒
        /// </summary>
        /// <param name="host">攻擊盒的宿主，通常在宿主那是傳this</param>
        public void InitAttackBox(IAttacker host)
        {
            Host = host;
        }

        /// <summary>
        /// 當Enable時，註冊宿主的攻擊資料更新事件，讓攻擊資料保持同步
        /// </summary>
        protected virtual void OnEnable()
        {
            HittenBoxes = new List<DefendBox>();
            Host.AttackInfoUpdate += OnAttackInfoUpdate;
        }

        /// <summary>
        /// 宿主攻擊力改變時，更新攻擊力
        /// </summary>
        /// <param name="fNewAttackValue">新的攻擊力</param>
        protected void OnAttackInfoUpdate(float fNewAttackValue,AttackBox attackBox)
        {
            if (attackBox == this)
            DamageThisHit = fNewAttackValue;
        }

        /// <summary>
        /// 計算這次攻擊傳給受擊盒的最終數值，預設是直接回傳宿主攻擊力，但如果你有一些複雜的傷害數值邏輯要做，例如投射物越近打越痛之類的，可以在這邊override
        /// </summary>
        /// <returns>最終傷害值</returns>
        /// <param name="fBaseDamage">基本傷害值，通常是m_damageThisHit</param>
        protected virtual float CalculateFinalDamage(float fBaseDamage)
        {
            return fBaseDamage;
        }
        /*詳細說明
        舉例來說今天做一個新的獸人狂戰士AttackBox，繼承這個Class，然後他有特殊的傷害計算方式：如果宿主血量低於一半，傷害兩倍，就可以這樣寫：


        {
            ...
            
            protected override float DamageThisHit { get { return CalculateFinalDamage(m_fDamageThisHit);} set { m_fDamageThisHit = value;}}
            private float m_fDamageThisHit;

            ...

            然後 CalculateFinalDamage()那邊也覆寫成：
            protected override float CalculateFinalDamage(float fBaseDamage)
            {
                if (Host.HP < Host.MaxHP * 0.5) (這邊為了舉例方便假設Host也繼承了IDefender有HP跟MaxHP等屬性)
                return fBaseDamage * 2;
            }
            
            ...
        }
        這樣在下面PassDamage()要拿傷害傳給受擊盒的時候就直接拿DamageThisHit就好，不用再塞條件判定
        */


        protected virtual void Update()
        {
            if (Collider.enabled == false) //檢查若Collider沒有作用就關閉攻擊盒
            {
                Debug.Log( Host + " 的攻擊盒Collider關閉，關閉攻擊盒！");
                enabled = false;
            }
        }
       

        /// <summary>
        /// 當碰撞發生時，呼叫傷害傳遞
        /// </summary>
        /// <param name="other">撞到的Collider</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            PassDamage(other,Host.AttackerType);
        }

        /// <summary>
        /// 傷害傳遞
        /// </summary>
        /// <param name="other">撞到的Collider</param>
        void PassDamage(Collider other,EAttackerType damageType)
        {

            DefendBox hitTarget;
            if (other.gameObject.GetComponent<DefendBox>()) //檢查撞到的Collider有沒有受擊盒
            {
                hitTarget = other.gameObject.GetComponent<DefendBox>();
                if(HittenBoxes.Contains(hitTarget)==false) //檢查這個受擊盒不在這次攻擊已判定過的List裡面
                {
                    if (hitTarget.TakeDamageType.Contains(damageType) || hitTarget.Host.IsHarmless == false) //檢查這個受擊盒是否無敵，且會受到這個攻擊盒傷害類型的傷害
                    {
                        hitTarget.OnDamageOccured(DamageThisHit); //萬事俱備，傷害判定發生
                        HittenBoxes.Add(hitTarget); //把這個受擊盒加到這次攻擊已判定過的List裡面，防止重複判定
                        if (m_fInterval != 0f) //若有設置重複判定的時間，則開始等待重複判定的Coroutine
                        {
                            StartCoroutine(DamageOccurInterval(Interval,hitTarget));
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 等待下一次重複判斷的間隔
        /// </summary>
        /// <param name="fInterval">間隔秒數</param>
        /// <param name="boxToRemove">作用的受擊盒</param>
        IEnumerator DamageOccurInterval(float fInterval, DefendBox boxToRemove)
        {
            yield return new WaitForSeconds(fInterval);
            if (HittenBoxes!=null && HittenBoxes.Contains(boxToRemove))
            {
                HittenBoxes.Remove(boxToRemove);
            }
        }

        /// <summary> 
        /// 當Disable時，反註冊攻擊資料更新的事件
        /// </summary>
        protected void OnDisable()
        {
            Host.AttackInfoUpdate -= OnAttackInfoUpdate;
            HittenBoxes.Clear();
        }

    }
}
