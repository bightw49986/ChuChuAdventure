﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourcesManagement;

namespace BattleSystem 
{
    public partial class BattleData : MonoBehaviour, IAttacker, IDefender
	{
        public virtual float MaxHp { get { return m_MaxHp; } set { m_MaxHp = value; OnStatsChanged(); } }
        public virtual float Hp { get { return m_Hp; } set { m_Hp = value; OnStatsChanged(); } }
        public virtual float MaxEndurance { get { return m_MaxEndurance; } set { m_MaxEndurance = value; OnStatsChanged(); } }
        public virtual float Endurance { get { return m_Endurance; } set { m_Endurance = value; OnStatsChanged(); } }

        /// <summary>
        /// 初始化BattleData
        /// </summary>
        void InitBattleData()
        {
            objectPool = GameObject.FindGameObjectWithTag("Main").GetComponent<ObjectPool>();
            InitStats();
            InitMeleeAttackBoxes();
            InitDefendBoxs();
        }

        /// <summary>
        /// 初始化數值
        /// </summary>
        void InitStats()
        {
            Hp = MaxHp;
            Endurance = MaxEndurance;
        }

        /// <summary>
        /// 初始化所有隸屬於此BattleData的攻擊盒
        /// </summary>
        void InitMeleeAttackBoxes()
        {
            if (InitMessage)
                print(gameObject.name + " 開始初始化近戰攻擊盒");
            MeleeAttackBoxes = new List<AttackBox_Melee>();
            AtkValues = new Dictionary<AttackBox, float>();
            AttackBox_Melee[] attackBoxes = GetComponentsInChildren<AttackBox_Melee>();
            for (int i = 0; i < attackBoxes.Length; i++)
            {
                if (MeleeAttackBoxes.Contains(attackBoxes[i]) == false)
                {
                    MeleeAttackBoxes.Add(attackBoxes[i]);
                    attackBoxes[i].InitAttackBox(this);
                    if (attackBoxes[i].BasicAtk <= 0 && UpdateMessage)
                    {
                        Debug.LogWarning("忘記指定 " + attackBoxes[i].gameObject.name + " 的基本攻擊力了!");
                    }
                    AtkValues.Add(attackBoxes[i], attackBoxes[i].BasicAtk);
                    DisableAttackBox(i);
                }
            }
            if (InitMessage)
                print(gameObject.name + "攻擊盒初始化完成；共初始了: " + attackBoxes.Length + "個攻擊盒");
        }

        /// <summary>
        /// 加入可以用的特效種類
        /// </summary>
        /// <param name="types">Types.</param>
        public void AddParticleAttackBoxes(params Enum[] types)
        {
            if (InitMessage)
                print(gameObject.name + " 開始初始化特效攻擊盒");
            if(ParticleAttackBoxes == null) ParticleAttackBoxes = new List<Enum>();
            foreach (var t in types)
            {
                if (ParticleAttackBoxes.Contains(t) == false)
                {
                    ParticleAttackBoxes.Add(t);
                }
            }
        }

        /// <summary>
        /// 初始化所有隸屬於此BattleData的防禦盒
        /// </summary>
        void InitDefendBoxs()
        {
            if (InitMessage)
                print(gameObject.name + " 開始初始化防禦盒");
            DefendBoxes = new List<DefendBox>();
            DefendBox[] defendBoxes = GetComponentsInChildren<DefendBox>();
            for (int i = 0; i < defendBoxes.Length; i++)
            {
                if (DefendBoxes.Contains(defendBoxes[i]) == false)
                {
                    DefendBoxes.Add(defendBoxes[i]);
                    defendBoxes[i].InitDefendBox(this);
                    DisableDefendBox(i);
                }
            }
            if (InitMessage)
                print(gameObject.name + " 防禦盒初始化完成；共初始了: " + defendBoxes.Length + "個防禦盒");
        }

        /// <summary>
        /// 打開指定的攻擊盒
        /// </summary>
        /// <param name="iAttackBoxIndex">該攻擊盒在AttackBoxes清單裡的Index</param>
        public void EnableAttackBox(int iAttackBoxIndex)
        {
            if (iAttackBoxIndex <= MeleeAttackBoxes.Count - 1 && MeleeAttackBoxes[iAttackBoxIndex] != null)
            {
                MeleeAttackBoxes[iAttackBoxIndex].enabled = true;
                if (SwitchMessage)
                    print(gameObject.name + " 打開他的" + iAttackBoxIndex + "號攻擊盒，該攻擊盒攻擊力為: " + AtkValues[MeleeAttackBoxes[iAttackBoxIndex]] + "點");
            }
            else
            {
                Debug.LogWarning(gameObject.name + " 要打開的攻擊盒不存在！");
            }
        }

        /// <summary>
        /// 關閉指定的攻擊盒
        /// </summary>
        /// <param name="iAttackBoxIndex">該攻擊盒在AttackBoxes清單裡的Index</param>
        public void DisableAttackBox(int iAttackBoxIndex)
        {
            if (MeleeAttackBoxes[iAttackBoxIndex] != null)
            {
                if (SwitchMessage)
                    print(gameObject.name + " 關閉他的" + iAttackBoxIndex + "號攻擊盒");
                MeleeAttackBoxes[iAttackBoxIndex].enabled = false;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " 要關閉的攻擊盒不存在！");
            }
        }

        /// <summary>
        /// 指派新的攻擊力給指定的攻擊盒
        /// </summary>
        /// <param name="fNewValue">新的攻擊力</param>
        /// <param name="attackBox">指定的攻擊盒</param>
        public void SetAtkToAttackBox(float fNewValue, AttackBox attackBox)
        {
            if (attackBox != null && AtkValues.ContainsKey(attackBox))
            {
                if (UpdateMessage)
                    print(gameObject.name + " 的攻擊盒:" + attackBox.name + " 攻擊力更新了:" + AtkValues[attackBox] + " -> " + fNewValue);
                AtkValues[attackBox] = fNewValue;
                OnAttackInfoUpdate(fNewValue, attackBox);
            }

        }

        /// <summary>
        /// 從物件池拿來招式發射器
        /// </summary>
        /// <param name="poolKey">Pool key.</param>
        /// <param name="typeKey">Type key.</param>
        public void CastSkillBox(PoolKey poolKey, Enum typeKey)
        {
            if (ParticleAttackBoxes.Contains(typeKey) == false) return;
            GameObject attackFX = objectPool.AccessGameObjectFromPool(poolKey,typeKey);
            AttackBox_Skill skillBox = attackFX.GetComponent<AttackBox_Skill>();
            if (skillBox == null) return;
            skillBox.InitAttackBox(this);
        }

        /// <summary>
        /// 增加這個BattleData會受到的傷害類型
        /// </summary>
        /// <param name="types">要增加的類型</param>
        public void AddDamageTypes(params EAttackerType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (TakeDamageFrom.Contains(types[i]) == false)
                {
                    if (UpdateMessage)
                        print(gameObject.name + " 增加了會受到的傷害類型:" + types[i]);
                    TakeDamageFrom.Add(types[i]);
                }
            }
        }

        /// <summary>
        /// 刪除這個BattleData會受到的傷害類型
        /// </summary>
        /// <param name="type">要刪除的類型</param>
        public void RemoveDamageType(EAttackerType type)
        {
            if (TakeDamageFrom.Contains(type))
            {
                if (UpdateMessage)
                    print(gameObject.name + " 移除了會受到的傷害類型:" + type);
                TakeDamageFrom.Remove(type);
            }
            else
            {
                Debug.LogWarning(gameObject.name + " 要移除的傷害類型不存在!");
            }
        }

        /// <summary>
        /// 打開指定的防禦盒
        /// </summary>
        /// <param name="iDefendboxIndex">該防禦盒在DefendBoxes裡的index</param>
        public void EneableDefendBox(int iDefendboxIndex)
        {
            if (iDefendboxIndex <= DefendBoxes.Count - 1 && DefendBoxes[iDefendboxIndex] != null)
            {
                if (SwitchMessage)
                    print(gameObject.name + " 打開他的 " + iDefendboxIndex + "號防禦盒，宿主血量剩餘: " + Hp + " 點，強韌度: " + Endurance + " 點。");
                DefendBoxes[iDefendboxIndex].enabled = true;
                DefendBoxes[iDefendboxIndex].DamageOccured += OnDamageOccured;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " 要打開的防禦盒不存在！");
            }
        }

        /// <summary>
        /// 關閉指定的防禦盒
        /// </summary>
        /// <param name="iDefendboxIndex">該防禦盒在DefendBoxes裡的index</param>
        public void DisableDefendBox(int iDefendboxIndex)
        {
            if (DefendBoxes[iDefendboxIndex] != null)
            {
                if (SwitchMessage)
                    print(gameObject.name + " 關閉他的 " + iDefendboxIndex + "號防禦盒");
                DefendBoxes[iDefendboxIndex].enabled = false;
                DefendBoxes[iDefendboxIndex].DamageOccured -= OnDamageOccured;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " 要關閉防禦盒，但防禦盒不存在");
            }
        }

        /// <summary>
        /// 關閉此BattleData所有的防禦盒
        /// </summary>
        public void DisableAllDefendBoxes()
        {
            foreach (var d in DefendBoxes)
            {
                d.enabled = false;
                d.DamageOccured -= OnDamageOccured;
            }
            if (SwitchMessage)
                Debug.Log(gameObject.name + " 關閉所有防禦盒");
        }

        /// <summary>
        /// 隨時間恢復強韌度
        /// </summary>
        void RecoverEndurance()
        {
            m_fRecoverTime = m_fRecoverTime > 0 ? m_fRecoverTime - Time.deltaTime : 0f;
            if (m_fRecoverTime <= 0) Endurance = MaxEndurance;
        }

        /// <summary>
        /// 被打中時進入無敵狀態
        /// </summary>
        /// <returns>無敵狀態的持續時間</returns>
        IEnumerator HitInterval(float fHarmlessTime)
        {
            IsHarmless = true;
            if (HitMessage)
            {
                print(gameObject.name + " 遭擊中，無敵 " + fHarmlessTime + "秒");
            }
            yield return new WaitForSeconds(fHarmlessTime);
            IsHarmless = false;
        }

        /// <summary>
        /// 當攻擊力數值改變，觸發事件通知攻擊盒
        /// </summary>
        /// <param name="fNewAtkValue">新的攻擊力</param>
        /// <param name="attackBox">攻擊盒</param>
        public void OnAttackInfoUpdate(float fNewAtkValue, AttackBox attackBox)
        {
            if (AtkValueChanged != null)
            {
                AtkValueChanged(fNewAtkValue, attackBox);
            }
        }

        /// <summary>
        /// 當傷害產生，計算傷害，判斷結果
        /// </summary>
        /// <param name="fDamage">傷害值</param>
        public void OnDamageOccured(float fDamage, DefendBox damagedPart)
        {
            if (fDamage <= 0f)
            {
                Debug.LogWarning("擊中傷害不能<=0");
                return;
            }

            float fPredictHP = Hp - fDamage;
            float fPredictToughness = Endurance - fDamage;

            if (IsHarmless == true)
            {
                if (HitMessage)
                    print(gameObject.name + " 在被擊中時處於無敵狀態，故不產生傷害判定");
                return;
            }
            if (fPredictHP <= 0)
            {
                if (HitMessage)
                    print(gameObject.name + " 被打死了，關閉所有防禦盒");
                DisableAllDefendBoxes();
                Hp = 0f;
                Endurance = 0f;
                OnDied();
            }
            else
            {
                if (fDamage > KOLine)
                {
                    Hp = fPredictHP;
                    Endurance = MaxEndurance;
                    if (HitMessage)
                        print(gameObject.name + " 受到巨大傷害而倒地了，" + "目前HP： " + Hp + "，強韌度：" + Endurance);
                    OnKOed();
                    m_fRecoverTime = RecoverTime;
                }
                else
                {
                    if (fPredictToughness <= 0)
                    {
                        Hp = fPredictHP;
                        Endurance = MaxEndurance;
                        if (m_fRecoverTime > 0)
                        {
                            if (HitMessage)
                                print(gameObject.name + " 連續受到而倒地了，" + "目前HP： " + Hp + "，強韌度：" + Endurance);
                            OnKOed();
                        }
                        else
                        {
                            if (HitMessage)
                                print(gameObject.name + " 被打硬直了，" + "目前HP： " + Hp + "，強韌度：" + Endurance);
                            OnFreezed(damagedPart);
                        }
                        m_fRecoverTime = RecoverTime;
                    }
                    else
                    {
                        Hp = fPredictHP;
                        Endurance = fPredictToughness;
                        if (HitMessage)
                            print(gameObject.name + " 被擊中了，" + "目前HP： " + Hp + "，強韌度：" + Endurance);
                        OnHit();
                    }
                }
            }
        }

        /// <summary>
        /// 當單次攻擊成功打中目標，通知身上其他攻擊盒不能在對目標造成傷害
        /// </summary>
        /// <param name="hitTarget">打中的防禦盒</param>
        /// <param name="attackBox">哪個攻擊盒打中</param>
        public void OnAttackSuccess(DefendBox hitTarget,AttackBox attackBox)
        {
            if(attackBox.GetType()==typeof(AttackBox_Melee))
            {
                foreach (var atkBox in MeleeAttackBoxes)
                {
                    if (atkBox.HitBoxes.Contains(hitTarget) == false)
                    {
                        atkBox.HitBoxes.Add(hitTarget);
                        if (atkBox.Interval != 0)
                        {
                            StartCoroutine(atkBox.HitInterval(atkBox.Interval, hitTarget));
                        }
                    }
                }
            }
            if (AttackSuccess != null) AttackSuccess(hitTarget, attackBox);
        }

        /// <summary>
        /// 當被打中時觸發事件
        /// </summary>
        public void OnHit()
        {
            if (Hit != null) Hit();
        }

        /// <summary>
        /// 當被打中，產生硬直時觸發事件
        /// </summary>
        public void OnFreezed(DefendBox damagedPart)
        {
            if (hitInterval != null)
                StopCoroutine(hitInterval);
            hitInterval = StartCoroutine(HitInterval(HitSafeTime));
            if (Freezed != null) Freezed(damagedPart);
        }

        /// <summary>
        /// 當被擊倒時觸發事件
        /// </summary>
        public void OnKOed()
        {
            if (hitInterval != null)
                StopCoroutine(hitInterval);
            hitInterval = StartCoroutine(HitInterval(KOSafeTime));
            if (KOed != null) KOed();
        }

        /// <summary>
        /// 當死亡時觸發事件
        /// </summary>
        public void OnDied()
        {
            if (hitInterval != null)
                StopCoroutine(hitInterval);
            if (Died != null) Died();
        }

        /// <summary>
        /// 當有數值改變觸發事件
        /// </summary>
        protected virtual void OnStatsChanged()
        {
            if (StatsChanged != null) //若有人註冊，則觸發事件
                StatsChanged(this);
        }
    }
}
