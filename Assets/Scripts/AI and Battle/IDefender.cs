using System;

namespace BattleSystem
{
    /// <summary>
    /// 定義要使用受擊盒的角色所需的資料
    /// </summary>
    public interface IDefender
    {
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
        void InitDefendBoxs();

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
        /// 倒地觸發的事件
        /// </summary>
        event Action KOed;
        void OnKOed();

        /// <summary>
        /// 死亡的事件
        /// </summary>
        event Action Died;
        void OnDied();
    }
}
