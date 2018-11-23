using System.Collections.Generic;
using UnityEngine;
using BattleSystem;
using System;
using PathFinding;

public class Player : BattleData ,ILocationData
{
    public Vector3 Position { get { return GetPosOnPlane(); } set { gameObject.transform.position = value; }}

    public Vector3 GetPosOnPlane()
    {
        Ray ray = new Ray(gameObject.transform.position + Vector3.up * 2, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 10f, GetComponent<InputMotionController>().groundLayer);
        return hit.point == Vector3.zero ? transform.position : hit.point; 
    }

    public int AreaID { get; set; }

    [Header("Move Conditions")]
    [Tooltip("主角當前可否移動")] public bool bInputMove = true;
    [Tooltip("主角現在狀態能否跳躍 ")] public bool bInputJump = true;
    [Tooltip("主角現在狀態能否瞬移")] public bool bInputDash = true;
    [Tooltip("主角是否忽略重力")] public bool bIgnoreGravity = false;

    [Header("Combat Conditions")]
    [Tooltip("是否記錄預輸入")] public bool bPreEnter;
    [Tooltip("是否在戰鬥狀態")] public bool bInCombat = false;
    [Tooltip("目標到主角的向量")] public Vector3 enemyToPlayer;

    [Header("Madness")]
    [Tooltip("是否可累積無雙值")] public bool bCanIncreasePower = true;
    [Tooltip("無雙值，集滿可以放無雙")] [SerializeField] float m_MaxPower = 10f;
    public float MaxPower { get { return m_MaxPower; } set { m_MaxPower = value; OnStatsChanged(); } }
    [Tooltip("無雙時間")] [SerializeField] float m_fMadnessTime = 8f;
    public float MadnessTime { get { return m_fMadnessTime; } set { m_fMadnessTime = value; OnStatsChanged(); } }
    [Tooltip("目前累積無雙值")] [SerializeField] float m_Power = 0f;
    public float Power { get { return m_Power; } set { m_Power = value; OnStatsChanged(); } }
    [Tooltip("最大攻速倍率")] [SerializeField] float m_MadnessMoveSpeedRate = 1.5f;
    public float MadnessMoveSpeedRate { get { return m_MadnessMoveSpeedRate; } set { m_MadnessMoveSpeedRate = value; OnStatsChanged(); } }
    [Tooltip("攻速倍率")] [SerializeField] float m_MadnessAttackSpeedRate = 1.4f;
    public float MadnessAttackSpeedRate { get { return m_MadnessAttackSpeedRate; } set { m_MadnessAttackSpeedRate = value; OnStatsChanged(); } }

    [Header("Combat Rank")]
    [Tooltip("戰鬥評價level")] [SerializeField] int m_CombatRank;
    public int CombatRank { get { return m_CombatRank; } set { m_CombatRank = value; OnStatsChanged(); } }
    [Tooltip("戰鬥評價進度條")] [SerializeField] float m_CombatProcess;
    public float CombatProcess { get { return m_CombatProcess; } set { m_CombatProcess = value; OnStatsChanged(); } }
    [Tooltip("評價上升難度")] [SerializeField] float m_fRankDifficulty = 0.05f;
    public float RankDifficulty { get { return m_fRankDifficulty; } set { m_fRankDifficulty = value; OnStatsChanged(); } }
    [Tooltip("連續攻擊評價加成倍率")] [SerializeField] float m_fComboRate = 2f;
    [Tooltip("被擊倒無法提升評價的時間")] [SerializeField] float m_fBeKOedBlockTime = 5f;
    [Tooltip("脫戰時間")] [SerializeField] float m_fEscapeCombatTime = 5f;
    public float EscapeCombatTime { get { return m_fEscapeCombatTime; } set { m_fEscapeCombatTime = value; OnStatsChanged(); } }

    [Header("Hp Recovery")]
    [Tooltip("是否正在可回血狀態")] public bool bHPRecoverable = false;
    [Tooltip("每禎掉血量的值")] [SerializeField] float m_LostHPRecoverySpeed = 0.03f;
    public float LostHPRecoverySpeed { get { return m_LostHPRecoverySpeed; } set { m_LostHPRecoverySpeed = value; OnStatsChanged(); } }
    [Tooltip("失血後多久開始掉血")] [SerializeField] float m_TimeBeforeLostHPRecovery = 0.5f;
    public float TimeBeforeLostHPRecovery { get { return m_TimeBeforeLostHPRecovery; } set { m_TimeBeforeLostHPRecovery = value; OnStatsChanged(); } }
    [Tooltip("攻敵後依攻擊力回血的倍率")] [SerializeField] float m_AttackRecoverHPRate = 1.0f;
    public float AttackRecoverHPRate { get { return m_AttackRecoverHPRate; } set { m_AttackRecoverHPRate = value; OnStatsChanged(); } }
    [Tooltip("目前可回到的最大HP")] [SerializeField] float m_CurrentMaxHPRecoverable;
    public float CurrentMaxHPRecoverable { get { return m_CurrentMaxHPRecoverable; } set { m_CurrentMaxHPRecoverable = value; OnStatsChanged(); } }



    public float BeKOedBlockTime { get { return m_fBeKOedBlockTime; } set { m_fBeKOedBlockTime = value; OnStatsChanged(); } }
    public float ComboRate { get { return m_fComboRate; } set { m_fComboRate = value; OnStatsChanged(); } }
    public override float MaxHp { get { return m_MaxHp; } set { m_MaxHp = value; OnStatsChanged(); } }
    public override float Hp { get { return m_Hp; } set { m_Hp = value; OnStatsChanged(); } }
    public override float MaxEndurance { get { return m_MaxEndurance; } set { m_MaxEndurance = value; OnStatsChanged(); } }
    public override float Endurance { get { return m_Endurance; } set { m_Endurance = value; OnStatsChanged(); } }



    protected override void Awake()
    {
        base.Awake();
        Power = 0f;
        AreaID = 1;
        CurrentMaxHPRecoverable = MaxHp;
    }

    void Start()
    {
        EneableDefendBox(0);
    }

    public new event Action<Player> StatsChanged;

    protected override void OnStatsChanged()
    {
        if (StatsChanged != null)
        {
            StatsChanged(this);
        }
    }

    void InitPlayerStats()
    {
        AttackerType = EAttackerType.Player;
        TakeDamageFrom = new List<EAttackerType>{ EAttackerType.MonsterGroup1,EAttackerType.MonsterGroup2};
    }
}
