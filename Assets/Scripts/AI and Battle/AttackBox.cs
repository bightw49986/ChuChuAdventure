using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BattleSystem
{
    /// <summary>
    /// 基本攻擊盒，運作方式為狀態機或其他外部腳本控制Enalbe開關
    /// </summary>

    public abstract class AttackBox : MonoBehaviour
    {
        /// <summary>
        /// 這個攻擊盒的主人，從它註冊AttackInfoUpdate事件檢查攻擊力的變動
        /// </summary>
        /// <value>攻擊盒的擁有者</value>
        protected internal BattleData Host;

        /// <summary>
        /// 要不要印攻擊盒Log
        /// </summary>
        [SerializeField] protected bool PrintLog;

        /// <summary>
        /// 攻擊盒的基本攻擊力
        /// </summary>
        public float BasicAtk = 1;

        /// <summary>
        /// 這一下攻擊會造成多少傷害，覆寫CalculateFinalDamage函式來更改計算傷害的邏輯
        /// </summary>
        /// <value>傷害值</value>
        protected float DamageThisHit { get { return CalculateFinalDamage(m_fDamageThisHit); } set {m_fDamageThisHit = value; } }
        float m_fDamageThisHit;

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

        /// <summary>
        /// 擊中時播放特效
        /// </summary>
        public GameObject HitFX { get { return m_hitFX; } private set { m_hitFX = value; } }
        [SerializeField] GameObject m_hitFX;

        protected virtual void Awake()
        {
            enabled = false;
        }

        /// <summary>
        /// 當Enable時，註冊宿主的攻擊資料更新事件，讓攻擊資料保持同步
        /// </summary>
        protected void OnEnable()
        {
            StartDetection();
        }
        protected abstract void StartDetection();


        /// <summary> 
        /// 當Disable時，反註冊攻擊資料更新的事件
        /// </summary>
        protected void OnDisable()
        {
            StopDetection();
        }
        protected abstract void StopDetection();


        /// <summary>
        /// 外部初始化攻擊盒所要呼叫的函式，宿主必須在初始化每一個攻擊盒時都將自己當作參數傳給攻擊盒
        /// </summary>
        /// <param name="host">攻擊盒的宿主，通常在宿主那是傳this</param>
        public void InitAttackBox(IAttacker host)
        {
            Host = (BattleData)host;
            DamageThisHit = BasicAtk;
            if (PrintLog)
                print("攻擊盒端: " + name + " 初始化成功(宿主: " + Host.name + ")");
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

        /// <summary>
        /// 傷害傳遞
        /// </summary>
        /// <param name="other">撞到的Collider</param>
        protected void PassDamage(GameObject other, EAttackerType damageType)
        {

            DefendBox hitTarget;
            if ((hitTarget = other.GetComponent<DefendBox>()) && hitTarget.enabled == true) //檢查撞到的Collider有沒有受擊盒
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
                        Host.OnAttackSuccess(hitTarget, this); //告訴宿主這次攻擊有打中這個目標，避免重複判定
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
        /// 指定攻擊盒的擊中特效
        /// </summary>
        /// <param name="go">特效的Prefab</param>
        public void SetHitFX(GameObject go)
        {
            HitFX = go;
        }

    }
}
