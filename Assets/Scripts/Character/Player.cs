using System.Collections.Generic;
using UnityEngine;
using BattleSystem;


/// <summary>
/// 咱們可愛的ChuChu
/// </summary>
public class Player : BattleData , PathFinding.ILocationData
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

    [Header("Conditions")]
    [Tooltip("主角當前可否移動")] public bool bCanMove = true;
    [Tooltip("主角現在狀態能否攻擊")] public bool bCanAttack = true;
    [Tooltip("主角現在狀態能否跳躍 ")] public bool bCanJump = true;
    [Tooltip("主角現在狀態能否瞬移")] public bool bCanDash = true;
    [Tooltip("主角是否忽略重力")] public bool bIgnoreGravity;

    [Tooltip("是否阻斷輸入")] public bool bBlockInput;
    //public static float fBlockTime = 0; //
    [Tooltip("是否記錄預輸入")] public bool bPreEnter;
    //public float fRecordTime = 0f;

    [Header("Player Ability Stats")]
    [Tooltip("最大攻速倍率")] [SerializeField] float m_MaxAttackSpeedRate = 2f;
    [Tooltip("攻速倍率")] [SerializeField] float m_AttackSpeedRate = 1f;
    [Tooltip("無雙值，集滿可以放無雙")] [SerializeField] float m_MaxPower = 10f;
    [Tooltip("目前累積無雙值")] [SerializeField] float m_Power;

    public float MaxAttackSpeedRate { get { return m_MaxAttackSpeedRate; } set { m_MaxAttackSpeedRate = value; OnStatsChanged(); } }
    public float AttackSpeedRate { get { return m_AttackSpeedRate; } set { m_AttackSpeedRate = value; OnStatsChanged(); } }
    public float MaxPower { get { return m_MaxPower; } set { m_MaxPower = value; OnStatsChanged(); } }
    public float Power { get { return m_Power; } set { m_Power = value; OnStatsChanged(); } }
    public override float MaxHp { get { return m_MaxHp; } set { m_MaxHp = value; OnStatsChanged(); } }
    public override float Hp { get { return m_Hp; } set { m_Hp = value; OnStatsChanged(); } }
    public override float MaxEndurance { get { return m_MaxEndurance; } set { m_MaxEndurance = value; OnStatsChanged(); } }
    public override float Endurance { get { return m_Endurance; } set { m_Endurance = value; OnStatsChanged(); } }


    protected override void Awake()
    {
        base.Awake();
        Power = 0f;
        AreaID = 1;
    }

    void Start()
    {
        EneableDefendBox(0);
    }

    protected override void OnStatsChanged()
    {
        if (m_Stats.ContainsKey("MaxAttackSpeedRate"))
            m_Stats["MaxAttackSpeedRate"] = MaxAttackSpeedRate;
        else
            m_Stats.Add("MaxAttackSpeedRate", MaxAttackSpeedRate);
        if (m_Stats.ContainsKey("AttackSpeedRate"))
            m_Stats["AttackSpeedRate"] = AttackSpeedRate;
        else
            m_Stats.Add("AttackSpeedRate", AttackSpeedRate);
        if (m_Stats.ContainsKey("MaxPower"))
            m_Stats["MaxPower"] = MaxPower;
        else
            m_Stats.Add("MaxPower", MaxPower);
        if ((m_Stats.ContainsKey("Power")))
            m_Stats["Power"] = Power;
        else
            m_Stats.Add("Power", Power);
        base.OnStatsChanged();
    }



    void InitPlayerStats()
    {
        AttackerType = EAttackerType.Player;
        TakeDamageFrom = new List<EAttackerType>{ EAttackerType.MonsterGroup1,EAttackerType.MonsterGroup2};
    }

    //public float fMouseInputSpeed = 1.0f;//滑鼠靈敏度
    //public float fLoseRecoverySpeedRate = 1f;//回血丟失速度倍率
}
