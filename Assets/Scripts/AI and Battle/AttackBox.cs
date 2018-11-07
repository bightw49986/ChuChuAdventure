using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BattleSystem
{
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
        protected BattleData Host;

        /// <summary>
        /// 要不要印攻擊盒Log
        /// </summary>
        [SerializeField] protected bool PrintLog;

        /// <summary>
        /// 攻擊盒的基本攻擊力
        /// </summary>
        public float BasicAtk = 1;

        /// <summary>
        /// 這一下攻擊會造成多少傷害，在繼承的情況下覆寫這個屬性，可以在Getter那邊和CalculateFinalDamage()搭配，增加額外的傷害計算邏輯(先往下看，CalculateFinalDamage那邊有更多註解)
        /// </summary>
        /// <value>傷害值</value>
        protected virtual float DamageThisHit
        {get { return CalculateFinalDamage(m_fDamageThisHit); } set {m_fDamageThisHit = value; } }
        float m_fDamageThisHit;

        /// <summary>
        /// 這個攻擊盒的的Collider
        /// </summary>
        /// <value>Collider</value>
        protected Collider Collider;

        /// <summary>
        /// 記錄這次攻擊已擊中的防禦盒，避免重複判定
        /// </summary>
        /// <value>被打中的盒子們</value>
        public List<DefendBox> HitBoxes;

        /// <summary>
        /// 定義若允許在一次開關之間重複判定的判定間隔秒數
        /// </summary>
        /// <value>間隔秒數，若 = 0 代表不允許重複判定</value>
        public float Interval;

        protected virtual void Awake()
        {
            Collider = GetComponent<Collider>();
            enabled = false;
        }

        /// <summary>
        /// 當Enable時，註冊宿主的攻擊資料更新事件，讓攻擊資料保持同步
        /// </summary>
        protected virtual void OnEnable()
        {
            HitBoxes = new List<DefendBox>();
            if (Host.AtkValues.ContainsKey(this))
            {
                DamageThisHit = Host.AtkValues[this];
            }
            Host.AttackInfoUpdate += OnAttackInfoUpdate;
            Collider.enabled = true;
            if (PrintLog)
                print("攻擊盒端: " + name + "開啟成功，註冊傷害更新事件(宿主: " + Host.name + ")");
        }

        /// <summary> 
        /// 當Disable時，反註冊攻擊資料更新的事件
        /// </summary>
        protected void OnDisable()
        {
            if (Host != null)
            {
                Host.AttackInfoUpdate -= OnAttackInfoUpdate;
                HitBoxes.Clear();
                Collider.enabled = true;
                if (PrintLog)
                    print("攻擊盒端: " + name + "關閉成功，反註冊傷害更新事件(宿主: " + Host.name + ")");
            }
        }

        /// <summary>
        /// 外部初始化攻擊盒所要呼叫的函式，宿主必須在初始化每一個攻擊盒時都將自己當作參數傳給攻擊盒
        /// </summary>
        /// <param name="host">攻擊盒的宿主，通常在宿主那是傳this</param>
        public void InitAttackBox(IAttacker host)
        {
            Host = (BattleData)host;
            if (PrintLog)
                print("攻擊盒端: " + name + " 初始化成功(宿主: " + Host.name + ")");
        }

        /// <summary>
        /// 宿主攻擊力改變時，更新攻擊力
        /// </summary>
        /// <param name="fNewAttackValue">新的攻擊力</param>
        protected void OnAttackInfoUpdate(float fNewAttackValue, AttackBox attackBox)
        {
            if (attackBox == this)
            {
                DamageThisHit = fNewAttackValue;
                if (PrintLog)
                    print("攻擊盒端: " + name + "成功更新傷害(宿主: " + Host.name + ")");
            }
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
                Debug.LogWarning(Host.name + " 的攻擊盒: " + name + "Collider關閉，關閉攻擊盒！");
                enabled = false;
            }
        }

        /// <summary>
        /// 當碰撞發生時，呼叫傷害傳遞
        /// </summary>
        /// <param name="other">撞到的Collider</param>
        protected virtual void OnTriggerStay(Collider other)
        {
            if (enabled)
            PassDamage(other, Host.AttackerType);
        }

        /// <summary>
        /// 傷害傳遞
        /// </summary>
        /// <param name="other">撞到的Collider</param>
        void PassDamage(Collider other, EAttackerType damageType)
        {

            DefendBox hitTarget;
            if ((hitTarget = other.gameObject.GetComponent<DefendBox>()) && hitTarget.enabled==true) //檢查撞到的Collider有沒有受擊盒
            {
                if (hitTarget.Host == Host)
                {
                    if (PrintLog)
                        print("攻擊盒端: " + name + "與同宿主防禦盒碰撞，不計算傷害(宿主: " + Host.name + ")");
                    return;//如果撞到自己的防禦盒就return(不允許自傷)
                }
                if (HitBoxes.Contains(hitTarget) == false) //檢查這個受擊盒不在這次攻擊已判定過的List裡面
                {
                    if (hitTarget.TakeDamageType.Contains(damageType) || hitTarget.Host.IsHarmless == false) //檢查這個受擊盒是否無敵，且會受到這個攻擊盒傷害類型的傷害
                    {   //萬事俱備，傷害判定發生
                        hitTarget.OnDamageOccured(DamageThisHit); //把傷害傳給DefendBox
                        Host.OnAttackSuccess(hitTarget); //告訴宿主這次攻擊有打中這個目標，避免重複判定
                        if (PrintLog)
                            print("攻擊盒端: " + name + "與防禦盒: " + hitTarget.name + "發生碰撞，傳送 " + DamageThisHit + "點傷害過去計算(宿主: " + Host.name + ")");
                    }
                    else
                    {
                        if (PrintLog)
                            print("攻擊盒端: " + name + "與防禦盒: " + hitTarget.name + "發生碰撞，但防禦盒端是無敵狀態，或是傷害類型不符，不計算傷害(宿主: " + Host.name + ")");
                    }
                }
                else
                {
                    if (PrintLog)
                        print("攻擊盒端: " + name + "與防禦盒: " + hitTarget.name + "在碰撞間隔內發生重複碰撞，不計算傷害(宿主: " + Host.name + ")");
                }
            }
        }

        /// <summary>
        /// 等待下一次重複判斷的間隔
        /// </summary>
        /// <param name="fInterval">間隔秒數</param>
        /// <param name="boxToRemove">作用的受擊盒</param>
        public IEnumerator DealDamageInterval(float fInterval, DefendBox boxToRemove)
        {
            yield return new WaitForSeconds(fInterval);
            if (HitBoxes != null && HitBoxes.Contains(boxToRemove))
            {
                HitBoxes.Remove(boxToRemove);
            }
        }
    }
}
