using System;

namespace BattleSystem
{
    /// <summary>
    /// 攻擊者的分類枚舉，0是玩家，1是獸人、哥布林、胖子，2是樹人跟石頭以及魔王
    /// </summary>
    public enum EAttackerType { Player = 0, MonsterGroup1 = 1, MonsterGroup2 = 2 }

    /// <summary>
    /// 定義要使用基本開關式攻擊盒(近戰)的角色所需的資料
    /// </summary>
    public interface IAttacker
    {
        /// <summary>
        /// 當AtkValue被Set時觸發事件，通知所有註冊的攻擊盒更新資訊
        /// </summary>
        event Action<float, AttackBox> AtkValueChanged;

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
        void SetAtkToAttackBox(float fNewAttackValue, AttackBox attackBox);

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
}

