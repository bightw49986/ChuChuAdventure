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
        /// <summary>
        /// 這個傷害承受者身上所有的受擊盒
        /// </summary>
        /// <value>受擊盒們</value>
        List<DefendBox> DefendBoxes { get; set; }

        /// <summary>
        /// 這個傷害承受者會承受的傷害來源類型
        /// </summary>
        /// <value>會受傷的類型</value>
        HashSet<EAttackerType> TakeDamageFrom { get; set; }

        /// <summary>
        /// 受擊者的最大生命值
        /// </summary>
        /// <value>最大生命值</value>
        float MaxHP { get; set; }

        /// <summary>
        /// 受擊者的當前生命值
        /// </summary>
        /// <value>當前生命值</value>
        float HP { get; set; }

        /// <summary>
        /// 受擊者的最大強韌值
        /// </summary>
        /// <value>最大強韌值</value>
        float MaxToughness { get; set; }

        /// <summary>
        /// 受擊者的當前強韌值
        /// </summary>
        /// <value>強韌值</value>
        float Toughness { get; set; }

        /// <summary>
        /// 強韌值在歸零或是沒有受到傷害幾秒後會開始恢復
        /// </summary>
        /// <value>不受傷害的秒數</value>
        float RecoverTime { get; set; }

        /// <summary>
        /// 開始恢復後，每秒恢復多少強韌值
        /// </summary>
        /// <value>每秒恢復的強韌值</value>
        float RecoverSpeed { get; set; }

        /// <summary>
        /// 受擊者是否處於無敵狀態
        /// </summary>
        bool IsHarmless { get; set; }

        /// <summary>
        /// 把會受到的傷害類型加到TakeDamageFrom
        /// </summary>
        /// <param name="types">傷害類型</param>
        void AddDamageTypes(params EAttackerType[] types);

        /// <summary>
        /// 把指定的傷害類型從TakeDamageFrom當中移除
        /// </summary>
        /// <param name="type">傷害類型</param>
        void RemoveDamageType(EAttackerType type);

        /// <summary>
        /// 初始化所有受擊盒，把受擊盒加入DefendBoxes清單
        /// </summary>
        void RegisterDefendBoxs();

        /// <summary>
        /// 開啟特定的受擊盒，註冊DamageOccured傷害事件
        /// </summary>
        /// <param name="iDefendboxIndex">受擊盒在DefendBoxes清單裡的index</param>
        void EneableDefendBox(int iDefendboxIndex);

        /// <summary>
        /// 關閉特定的受擊盒，反註冊DamageOccured傷害事件
        /// </summary>
        /// <param name="iDefendboxIndex">受擊盒在DefendBoxes清單裡的index</param>
        void DisableDefendBox(int iDefendboxIndex);

        /// <summary>
        /// 關閉所有受擊盒，死亡時呼叫，防止鞭屍
        /// </summary>
        void DisableAllDefendBoxes();

        /// <summary>
        /// 傷害事件發生時要做的事(計算傷害、判斷硬直與否、死亡與否)
        /// </summary>
        /// <param name="fDamage">傷害值</param>
        void OnDamageOccured(float fDamage);

        /// <summary>
        /// 一般擊中時觸發的事件
        /// </summary>
        event Action Hitted;
        void OnHitted();

        /// <summary>
        /// 硬直觸發的事件
        /// </summary>
        event Action Freezed;
        void OnFreezed();

        /// <summary>
        /// 死亡的事件
        /// </summary>
        event Action Died;
        void OnDied();
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
        /// 這個受擊盒被打中時要播放的特效
        /// </summary>
        /// <value>特效的Prefab</value>
        [SerializeField] protected GameObject HitFX { get; set; }

        /// <summary>
        /// 啟動時，初始化受擊資訊
        /// </summary>
        void OnEnable() //undone 跟物件池要特效備用
        {
            TakeDamageType = Host.TakeDamageFrom;
        }

        void OnDisable() //undone 把特效還給物件池
        {

        }

        public void InitDefendBox(IDefender host)
        {
            Host = host;
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
