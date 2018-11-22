using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
    [RequireComponent(typeof(Collider))]
    public class AttackBox_Melee : AttackBox
    {
        /// <summary>
        /// 這個攻擊盒的的Collider
        /// </summary>
        /// <value>Collider</value>
        [HideInInspector] protected Collider Collider;

        protected override void Awake()
        {
            Collider = GetComponent<Collider>();
            base.Awake();
        }

        protected virtual void Update()
        {
            if (Collider.enabled == false) //檢查若Collider沒有作用就關閉攻擊盒
            {
                Debug.LogWarning(Host.name + " 的攻擊盒: " + name + "Collider關閉，關閉攻擊盒！");
                enabled = false;
            }
        }

        protected override void StartDetection()
        {
            HitBoxes = new List<DefendBox>();
            if (Host.AtkValues.ContainsKey(this))
            {
                DamageThisHit = Host.AtkValues[this];
            }
            Host.AtkValueChanged += OnAtkValueChanged;
            Collider.enabled = true;
            if (PrintLog)
                print("攻擊盒端: " + name + "開啟成功，註冊傷害更新事件(宿主: " + Host.name + ")");
        }

        protected override void StopDetection()
        {
            if (Host != null)
            {
                Host.AtkValueChanged -= OnAtkValueChanged;
                HitBoxes.Clear();
                Collider.enabled = true;
                if (PrintLog)
                    print("攻擊盒端: " + name + "關閉成功，反註冊傷害更新事件(宿主: " + Host.name + ")");
            }
        }

        /// <summary>
        /// 宿主攻擊力改變時，更新攻擊力
        /// </summary>
        /// <param name="fNewAttackValue">新的攻擊力</param>
        protected void OnAtkValueChanged(float fNewAttackValue, AttackBox attackBox)
        {
            if (attackBox == this)
            {
                DamageThisHit = fNewAttackValue;
                if (PrintLog)
                    print("攻擊盒端: " + name + "成功更新傷害(宿主: " + Host.name + ")");
            }
        }

        /// <summary>
        /// 等待下一次重複判斷的間隔
        /// </summary>
        /// <param name="fInterval">間隔秒數</param>
        /// <param name="boxToRemove">作用的受擊盒</param>
        public IEnumerator HitInterval(float fInterval, DefendBox boxToRemove)
        {
            yield return new WaitForSeconds(fInterval);
            if (HitBoxes != null && HitBoxes.Contains(boxToRemove))
            {
                HitBoxes.Remove(boxToRemove);
            }
        }

        /// <summary>
        /// 當碰撞發生時，呼叫傷害傳遞
        /// </summary>
        /// <param name="other">撞到的Collider</param>
        protected virtual void OnTriggerStay(Collider other)
        {
            if (enabled)
            {
                PassDamage(other.gameObject, Host.AttackerType);
            }
        }



    }
}
