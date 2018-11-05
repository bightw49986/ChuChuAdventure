using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BattleSystem
{
    public class BattleData : MonoBehaviour, IAttacker, IDefender
    {
        [Header("Attack Stats")]

        [Tooltip("基礎攻擊力")] public float BasicAtk = 1f;
        [Tooltip("這個角色的傷害類型")] public EAttackerType AttackerType = EAttackerType.MonsterGroup1;
        [Tooltip("這個角色當前擁有的攻擊盒")] public List<AttackBox> AttackBoxes;
        [Tooltip("每個攻擊盒各自對應的攻擊力(用SetAtkValueToAttackBox()來個別設定")] public Dictionary<AttackBox, float> AtkValues;

        [Header("Defend Stats")]
        [Tooltip("這個角色當前擁有的防禦盒")] public List<DefendBox> DefendBoxes;
        [Tooltip("這個角色會受到的傷害類型(用AddDamageTypes()來新增，用RemoveDamageType()來刪除)")] public List<EAttackerType> TakeDamageFrom = new List<EAttackerType> {EAttackerType.Player};
        [Tooltip("角色是否無敵")] public bool IsHarmless;
        [Tooltip("血量最大值")] public float MaxHP = 3f;
        [Tooltip("目前血量，歸零即死亡")] public float HP = 3f;
        [Tooltip("強韌度最大值")] public float MaxEndurance = 2f;
        [Tooltip("目前強韌度，歸零角色會硬直")] public float Endurance= 2f;
        [Tooltip("從硬直復原的時間，在硬直復原的時間內，如果強韌度又歸零，則觸發倒地")] public float RecoverTime;
        private float m_fRecoverTime;
        [Tooltip("若受到的傷害大於這個數值，則直接倒地")] public float KOLine = 2f;
        [Tooltip("倒地無敵的持續時間(秒)")] public float HarmlessTime = 0.3f;

        [Header("Log Setting")]
        [Tooltip("印出此BattleData初始化的訊息")] [SerializeField] bool InitMessage;
        [Tooltip("印出此BattleData開關戰鬥盒的訊息")] [SerializeField] bool SwitchMessage;
        [Tooltip("印出此BattleData受到傷害的訊息")] [SerializeField] bool HitMessage;
        [Tooltip("印出此BattleData更新其他屬性的訊息")] [SerializeField] bool UpdateMessage;

        protected virtual void Awake()
        {
            InitBattleData();
        }
        protected virtual void Start()
        {
            EneableDefendBox(0);
            EnableAttackBox(0);
        }
        protected virtual void Update()
        {
            RecoverEndurance();
        }

        public void InitBattleData()
        {
            InitAttackBoxes();
            InitDefendBoxs();
        }

        public void InitAttackBoxes()
        {
            if(InitMessage)
                print(gameObject.name + "開始初始化攻擊盒");
            AttackBoxes = new List<AttackBox>();
            AtkValues = new Dictionary<AttackBox, float>();
            AttackBox[] attackBoxes = GetComponentsInChildren<AttackBox>();
            for (int i = 0; i < attackBoxes.Length; i++)
            {
                if (AttackBoxes.Contains(attackBoxes[i]) == false)
                {
                    AttackBoxes.Add(attackBoxes[i]);
                    attackBoxes[i].InitAttackBox(this);
                    AtkValues.Add(attackBoxes[i], BasicAtk);
                    DisableAttackBox(i);
                }
            }
            if(InitMessage)
                print(gameObject.name + "攻擊盒初始化完成；共初始了: " + attackBoxes.Length + "個攻擊盒");
        }

        public void InitDefendBoxs()
        {
            if(InitMessage)
                print(gameObject.name + "開始初始化防禦盒");
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
            if(InitMessage)
                print(gameObject.name + " 防禦盒初始化完成；共初始了: " + defendBoxes.Length + "個防禦盒");
        }

        public void EnableAttackBox(int iAttackBoxIndex)
        {
            if (iAttackBoxIndex <= AttackBoxes.Count - 1 && AttackBoxes[iAttackBoxIndex] != null )
            {
                AttackBoxes[iAttackBoxIndex].enabled = true;
                if(SwitchMessage)
                    print(gameObject.name + "打開他的" + iAttackBoxIndex + "號攻擊盒，該攻擊盒攻擊力為: " + AtkValues[AttackBoxes[iAttackBoxIndex]] + "點");
            }
            else
            {
                Debug.LogWarning(gameObject.name + "要打開的攻擊盒不存在！");
            }
        }

        public void DisableAttackBox(int iAttackBoxIndex)
        {
            if (AttackBoxes[iAttackBoxIndex] != null)
            {
                if(SwitchMessage)
                    print(gameObject.name + "關閉他的" + iAttackBoxIndex + "號攻擊盒");
                AttackBoxes[iAttackBoxIndex].enabled = false;
            }
            else
            {
                Debug.LogWarning(gameObject.name + "要關閉的攻擊盒不存在！");
            }
        }

        public void SetAtkValueToAttackBox(float fNewAttackValue, AttackBox attackBox)
        {
            if (attackBox != null && AtkValues.ContainsKey(attackBox))
            {
                if (UpdateMessage)
                    print(gameObject.name + "的攻擊盒:" + attackBox.name + " 攻擊力更新了:" + AtkValues[attackBox] + " -> " + fNewAttackValue);
                AtkValues[attackBox] = fNewAttackValue;
                OnAttackInfoUpdate(fNewAttackValue, attackBox);
            }

        }

        public event Action<float, AttackBox> AttackInfoUpdate;
        public void OnAttackInfoUpdate(float fNewAttackValue, AttackBox attackBox)
        {
            if (AttackInfoUpdate != null)
            {
                AttackInfoUpdate(fNewAttackValue, attackBox);
            }
        }

        public void AddDamageTypes(params EAttackerType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (TakeDamageFrom.Contains(types[i]) == false)
                {
                    if(UpdateMessage)
                        print(gameObject.name + "增加了會受到的傷害類型:" + types[i]);
                    TakeDamageFrom.Add(types[i]);
                }
            }
        }

        public void RemoveDamageType(EAttackerType type)
        {
            if (TakeDamageFrom.Contains(type))
            {
                if(UpdateMessage)
                    print(gameObject.name + "移除了會受到的傷害類型:" + type);
                TakeDamageFrom.Remove(type);
            }
            else
            {
                Debug.LogWarning(gameObject.name + "要移除的傷害類型不存在!");
            }
        }

        public void EneableDefendBox(int iDefendboxIndex)
        {
            if (iDefendboxIndex <= DefendBoxes.Count - 1&& DefendBoxes[iDefendboxIndex] != null)
            {
                if(SwitchMessage)
                    print(gameObject.name + "打開他的 " + iDefendboxIndex + "號防禦盒，宿主血量剩餘: " + HP + " 點，強韌度: " + Endurance + " 點。" );
                DefendBoxes[iDefendboxIndex].enabled = true;
                DefendBoxes[iDefendboxIndex].DamageOccured += OnDamageOccured;
            }
            else
            {
                Debug.LogWarning(gameObject.name + "要打開的防禦盒不存在！");
            }
        }

        public void DisableDefendBox(int iDefendboxIndex)
        {
            if (DefendBoxes[iDefendboxIndex] != null)
            {
                if (SwitchMessage)
                    print(gameObject.name + "關閉他的 " + iDefendboxIndex + "號防禦盒");
                DefendBoxes[iDefendboxIndex].enabled = false;
                DefendBoxes[iDefendboxIndex].DamageOccured -= OnDamageOccured;
            }
            else
            {
                Debug.LogWarning(gameObject.name + "要關閉防禦盒，但防禦盒不存在");
            }
        }

        public void DisableAllDefendBoxes()
        {
            foreach (var d in DefendBoxes)
            {
                d.enabled = false;
                d.DamageOccured -= OnDamageOccured;
            }
            if(SwitchMessage)
                Debug.Log(gameObject.name + "關閉所有防禦盒");
        }

        public void OnDamageOccured(float fDamage)
        {
            if (fDamage <= 0f)
            {
                Debug.LogWarning("擊中傷害不能<=0");
                return;
            }

            float fPredictHP = HP - fDamage;
            float fPredictToughness = Endurance - fDamage;

            if(IsHarmless == true)
            {
                if (HitMessage)
                print(gameObject.name + "在被擊中時處於無敵狀態，故不產生傷害判定");
                return;
            }
            if (fPredictHP <= 0)
            {
                if (HitMessage)
                    print(gameObject.name + "被打死了，關閉所有防禦盒");
                DisableAllDefendBoxes();
                HP = 0f;
                Endurance = 0f;
                OnDied();
            }
            else
            {
                if (fDamage > KOLine)
                {
                    HP = fPredictHP;
                    Endurance = MaxEndurance;
                    if (HitMessage)
                        print(gameObject.name + "受到巨大傷害而倒地了，" + "目前HP： " + HP + "，強韌度：" + Endurance);
                    OnKOed();
                    m_fRecoverTime = RecoverTime;
                }
                else
                {
                    if (fPredictToughness <= 0)
                    {
                        HP = fPredictHP;
                        Endurance = MaxEndurance;
                        if (m_fRecoverTime > 0)
                        {
                            if (HitMessage)
                                print(gameObject.name + "連續受到而倒地了，" + "目前HP： " + HP + "，強韌度：" + Endurance);
                            OnKOed();
                        }
                        else
                        {
                            if (HitMessage)
                                print(gameObject.name + "被打硬直了，" + "目前HP： " + HP + "，強韌度：" + Endurance);
                            OnFreezed();
                        }
                        m_fRecoverTime = RecoverTime;
                    }
                    else
                    {
                        HP = fPredictHP;
                        Endurance = fPredictToughness;
                        if (HitMessage)
                            print(gameObject.name + "被擊中了，" + "目前HP： " + HP + "，強韌度：" + Endurance);
                        OnHitted();
                    }
                }
                StopCoroutine(HitInterval());
                StartCoroutine(HitInterval());
            }
        }

        public event Action Hitted;
        public void OnHitted()
        {
            if (Hitted != null) Hitted();
        }

        public event Action Freezed;
        public void OnFreezed()
        {
            if (Freezed != null) Freezed();
        }

        public event Action KOed;
        public void OnKOed()
        {
            if (KOed != null) KOed();
        }

        public event Action Died;
        public void OnDied()
        {
            if (Died != null) Died();
        }

        void RecoverEndurance()
        {
            m_fRecoverTime = m_fRecoverTime > 0 ? m_fRecoverTime - Time.deltaTime : 0f;
            if (m_fRecoverTime <= 0) Endurance = MaxEndurance;
        }

        IEnumerator HitInterval()
        {
            IsHarmless = true;
            if (HitMessage)
            {
                print(gameObject.name +  "遭擊中，無敵 " + HarmlessTime + "秒");
            }
            yield return new WaitForSeconds(HarmlessTime);
            IsHarmless = false;
        }
    }
}


