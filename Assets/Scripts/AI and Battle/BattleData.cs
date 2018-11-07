using System.Collections.Generic;
using UnityEngine;
using System;

namespace BattleSystem
{
    public partial class BattleData : MonoBehaviour, IAttacker, IDefender
    {
        protected Dictionary<string, float> m_Stats = new Dictionary<string, float>();

        [Header("Attack Stats")]
        [Tooltip("這個角色的傷害類型")] public EAttackerType AttackerType = EAttackerType.MonsterGroup1;
        [Tooltip("這個角色當前擁有的攻擊盒")] public List<AttackBox> AttackBoxes;
        [Tooltip("每個攻擊盒各自對應的攻擊力(用SetAtkValueToAttackBox()來個別設定")] public Dictionary<AttackBox, float> AtkValues;

        [Header("Defend Stats")]
        [Tooltip("這個角色當前擁有的防禦盒")] public List<DefendBox> DefendBoxes;
        [Tooltip("這個角色會受到的傷害類型(用AddDamageTypes()來新增，用RemoveDamageType()來刪除)")] public List<EAttackerType> TakeDamageFrom = new List<EAttackerType> {EAttackerType.Player};
        [Tooltip("角色是否無敵")] public bool IsHarmless;
        [Tooltip("從硬直復原的時間，在硬直復原的時間內，如果強韌度又歸零，則觸發倒地")] public float RecoverTime = 5f;
        protected float m_fRecoverTime;
        [Tooltip("血量最大值")] [SerializeField] protected float m_MaxHp = 10f;
        [Tooltip("目前血量，歸零即死亡")] [SerializeField] protected float m_Hp = 10f;
        [Tooltip("強韌度最大值")] [SerializeField] protected float m_MaxEndurance = 5f;
        [Tooltip("目前強韌度，歸零角色會硬直")][SerializeField] protected float m_Endurance = 5f;
        [Tooltip("若受到的傷害大於這個數值，則直接倒地")] public float KOLine = 2f;
        [Tooltip("受傷無敵的持續時間(秒)")] public float HitSafeTime = 0f;
        [Tooltip("倒地無敵的持續時間(秒)")] public float KOSafeTime = 0f;
        protected Coroutine hitInterval;

        [Header("Log Setting")]
        [Tooltip("印出此BattleData初始化的訊息")] [SerializeField] bool InitMessage;
        [Tooltip("印出此BattleData開關戰鬥盒的訊息")] [SerializeField] bool SwitchMessage;
        [Tooltip("印出此BattleData受到傷害的訊息")] [SerializeField] bool HitMessage;
        [Tooltip("印出此BattleData更新其他屬性的訊息")] [SerializeField] bool UpdateMessage;

        protected virtual void Awake()
        {
            InitBattleData();
        }

        protected virtual void Update()
        {
            RecoverEndurance();
        }

        public event Action<float, AttackBox> AttackInfoUpdate;

        public event Action Hit;

        public event Action Freezed;

        public event Action KOed;

        public event Action Died;

        /// <summary>
        /// UI註冊這個事件
        /// </summary>
        public event Action<Dictionary<string,float>> StatsChanged; 

    }
}


