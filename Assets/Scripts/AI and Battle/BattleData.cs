using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BattleSystem
{
    public class BattleData : IAttacker, IDefender
    {
        public GameObject GO { get; set; }
        public List<AttackBox> AttackBoxes { get; set; }
        public Dictionary<AttackBox, float> AtkValues { get; set; }
        public EAttackerType AttackerType { get; set; }
        public List<DefendBox> DefendBoxes { get; set; }

        public HashSet<EAttackerType> TakeDamageFrom { get; set; }
        public float MaxHP { get; set; }
        public float HP { get; set; }
        public float MaxToughness { get; set; }
        public float Toughness { get; set; }
        public float RecoverTime { get; set; }
        public float RecoverSpeed { get; set; }
        public bool IsHarmless { get; set; }

        public void InitBattleData(GameObject gameObject)
        {
            if (GO != null)
            {
                Debug.LogWarning("重複指派GO!");
                return;
            }
            GO = gameObject;
        }

        public void DisableAttackBox(int iAttackBoxIndex)
        {
            if (AttackBoxes[iAttackBoxIndex] != null)
            {
                AttackBoxes[iAttackBoxIndex].enabled = false;
            }
            else
            {
                Debug.LogWarning("攻擊盒不存在！");
            }
        }

        public void EnableAttackBox(int iAttackBoxIndex)
        {
            if (AttackBoxes[iAttackBoxIndex] != null)
            {
                AttackBoxes[iAttackBoxIndex].enabled = true;
            }
            else
            {
                Debug.LogWarning("攻擊盒不存在！");
            }
        }

        public void InitAttackBoxes()
        {
            AttackBox[] attackBoxes = GO.GetComponentsInChildren<AttackBox>();
            for (int i = 0; i < attackBoxes.Length; i++)
            {
                if (AttackBoxes.Contains(attackBoxes[i]) == false)
                {
                    AttackBoxes.Add(attackBoxes[i]);
                    attackBoxes[i].InitAttackBox(this);
                }
            }
        }





        public event Action<float, AttackBox> AttackInfoUpdate;
        public void OnAttackInfoUpdate(float fNewAttackValue, AttackBox attackBox)
        {
            if (AttackInfoUpdate != null) AttackInfoUpdate(fNewAttackValue, attackBox);
        }

        public void SetAtkValueToAttackBox(float fNewAttackValue, AttackBox attackBox)
        {
            if (AtkValues.ContainsKey(attackBox))
            {
                AtkValues[attackBox] = fNewAttackValue;
                OnAttackInfoUpdate(fNewAttackValue, attackBox);
            }

        }

        public void AddDamageTypes(params EAttackerType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (TakeDamageFrom.Contains(types[i]) == false)
                {
                    TakeDamageFrom.Add(types[i]);
                }
            }
        }

        public void RemoveDamageType(EAttackerType type)
        {
            if (TakeDamageFrom.Contains(type))
            {
                TakeDamageFrom.Remove(type);
            }
            else
            {
                Debug.LogWarning("傷害類型不存在!");
            }
        }

        public void RegisterDefendBoxs()
        {
            DefendBox[] defendBoxes = GO.GetComponentsInChildren<DefendBox>();
            for (int i = 0; i < defendBoxes.Length; i++)
            {
                if (DefendBoxes.Contains(defendBoxes[i]) == false)
                {
                    DefendBoxes.Add(defendBoxes[i]);
                    defendBoxes[i].InitDefendBox(this);
                }
            }

        }

        public void EneableDefendBox(int iDefendboxIndex)
        {
            if (DefendBoxes[iDefendboxIndex] != null)
            {
                DefendBoxes[iDefendboxIndex].enabled = true;
                DefendBoxes[iDefendboxIndex].DamageOccured += OnDamageOccured;
            }
            else
            {
                Debug.LogWarning("防禦盒不存在");
            }
        }

        public void DisableDefendBox(int iDefendboxIndex)
        {
            if (DefendBoxes[iDefendboxIndex] != null)
            {
                DefendBoxes[iDefendboxIndex].enabled = false;
                DefendBoxes[iDefendboxIndex].DamageOccured -= OnDamageOccured;
            }
            else
            {
                Debug.LogWarning("防禦盒不存在");
            }
        }

        public void DisableAllDefendBoxes()
        {
            foreach (var d in DefendBoxes)
            {
                d.enabled = false;
                d.DamageOccured -= OnDamageOccured;
            }
            Debug.Log("關閉所有防禦盒");
        }

        public void OnDamageOccured(float fDamage)
        {
            float fPredictHP = HP - fDamage;
            float fPredictToughness = Toughness - fDamage;
            if (fPredictHP <= 0)
            {
                DisableAllDefendBoxes();
                HP = 0f;
                Toughness = 0f;
                OnDied();
            }
            else
            {
                if (fPredictToughness <= 0)
                {
                    HP = fPredictHP;
                    Toughness = MaxToughness;
                    OnFreezed();
                }
                else
                {
                    HP = fPredictHP;
                    Toughness = fPredictToughness;
                    OnHitted();
                }
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

        public event Action Died;
        public void OnDied()
        {
            if (Died != null) Died();
        }
    }
}


