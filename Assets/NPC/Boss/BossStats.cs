using BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : BattleData
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
    }

    System.Random random = new System.Random();//隨機機
    public float m_fDistanceToPlayer;
    public Vector3 m_vDistanceToPlayer;
    Vector3 m_JumpDistance;
    Vector3 m_JumpHeight;
    public float Dot { get; private set; }
    public float m_DistanceLimit=3f;
    GameObject m_player;
    Quaternion m_quaternion;

    readonly float fAttackSkillOneCD=5f;
    readonly float fAttackSkillTwoCD=6f;
    readonly float fAttackSkillThreeCD=7f;
    public bool CanUseAttack1 { get; private set; }
    public bool CanUseAttack2 { get; private set; }
    public bool CanUseAttack3 { get; private set; }
    public bool Attack1StandBy { get; private set; }
    public bool Attack2StandBy { get; private set; }
    public bool Attack3StandBy { get; private set; }
    public bool isJumping;
    //各種Coroutine 
    float m_BossMaxHp = 1000f;
    float m_BossHp = 1000f;
    float m_BossMaxEndurance = 10f;
    float m_BossEndurance = 10f;
    public override float MaxHp { get { return m_BossMaxHp; } set { m_BossMaxHp = value; } }
    public override float Hp { get { return m_BossHp; } set { m_BossHp = value; } }
    public override float MaxEndurance { get { return m_BossMaxEndurance; } set { m_BossMaxEndurance = value; } }
    public override float Endurance { get { return m_BossEndurance; } set { m_BossEndurance = value; } }
    public bool BCanTransform { get; private set; }//Boss能不能變形了
    float fStoneTime = 15f;//只有石頭的時間
    float fIceOrFireTime = 25f;//有屬性的時間

    public GameObject StoneGolem;
    public GameObject FireGolem;
    public GameObject IceGolem;

    protected override void Awake()
    {
        base.Awake();//攻受擊盒未合好
        StoneGolem.SetActive(true);
        m_player = GameObject.FindGameObjectWithTag("Player");
        CanUseAttack1 = CanUseAttack2 = CanUseAttack3= BCanTransform = true;
    }

    public void StartRunningCD(string currentState)
    {
        StartCoroutine(currentState);
    }

    IEnumerator Attack1()
    {
        CanUseAttack1 = false;
        yield return new WaitForSeconds(fAttackSkillOneCD);
        CanUseAttack1 = true;
    }
    IEnumerator Attack2()
    {
        CanUseAttack2 = false;
        yield return new WaitForSeconds(fAttackSkillTwoCD);
        CanUseAttack2 = true;
    }
    IEnumerator Attack3()
    {
        CanUseAttack3 = false;
        yield return new WaitForSeconds(fAttackSkillThreeCD);
        CanUseAttack3 = true;
    }


    public float jumpTimes;
    public float jumpTimer;
    Vector3 jumpEveryFrame;
    //public float initY;

    public void InitJumpData()
    {
        //initY = transform.position.y;
        jumpTimer = 0;
        //jumpEveryFrame = m_JumpDistance / jumpTimes;
        jumpTimes = 0.8f / Time.deltaTime;
    }



    public void Jump()//跳高
    {
        //h = 最高點高度
        //a = 到達最高點所需時間
        //y = -(h / a ^ 2) * (x - a) ^ 2 + h by舜哲
        //print("Jumping");
        jumpEveryFrame.y = -(0.4f / 0.16f) * (jumpTimer - 0.4f) * (jumpTimer - 0.4f) + 0.4f;
        Rotate();
        jumpEveryFrame.x = m_JumpDistance.x / jumpTimes * jumpTimer;
        jumpEveryFrame.z = m_JumpDistance.z / jumpTimes * jumpTimer;
        transform.position += jumpEveryFrame;
        jumpTimer += Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, jumpEveryFrame,out hit, jumpEveryFrame.magnitude, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point + Vector3.up * 0.05f;
            isJumping = false;
        }
        
        
    }
    public void Rotate()
    {
        m_quaternion = m_vDistanceToPlayer != Vector3.zero ? Quaternion.LookRotation(m_vDistanceToPlayer, Vector3.up) : transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, m_quaternion, 0.05f);
        Quaternion qRot = transform.rotation;
        qRot.x = qRot.z = 0;
        transform.rotation = qRot;
    }


    public void CheckTargetDistance()//是否需要歸零?
    {
        m_vDistanceToPlayer = m_player.transform.position - transform.position;
        m_fDistanceToPlayer = m_vDistanceToPlayer.magnitude;
        Dot = Vector3.Dot(m_vDistanceToPlayer.normalized, transform.forward);
        m_JumpDistance = m_vDistanceToPlayer * (1 - (m_DistanceLimit / m_fDistanceToPlayer));
        if (FireGolem.activeInHierarchy == true)
        {
            if (Dot > 0.3f && m_fDistanceToPlayer < 4f) Attack1StandBy = true;
            if (Dot > 0.9f ) Attack2StandBy = true;
            if (Dot > 0.5f && m_fDistanceToPlayer < 10f) Attack3StandBy = true;
        }
        else if (IceGolem.activeInHierarchy == true)
        {
            if (Dot > 0.4f && m_fDistanceToPlayer < 5f) Attack1StandBy = true;
            if (Dot > 0.8f ) Attack2StandBy = true;
            if (Dot > 0 && m_fDistanceToPlayer < 1.5f) Attack3StandBy = true;
        }
        else if(StoneGolem.activeInHierarchy == true)
        {
            if (Dot > 0 && m_fDistanceToPlayer < 1f) Attack1StandBy = true;
            if (Dot > 0.7f) Attack2StandBy = true;
            if (Dot > 0 && m_fDistanceToPlayer < 10f) Attack3StandBy = true;
        }
    }

    //變身術
    public void SwitchBoss()
    {
        if (StoneGolem.activeInHierarchy == true)
        {
            StartCoroutine(OnfireOrIce());
        }
        else if (FireGolem.activeInHierarchy == true || IceGolem.activeInHierarchy == true)
        {
            StartCoroutine(ExtinguishfireOrIce());
        }
    }
     //有一半機率變火或冰
    IEnumerator OnfireOrIce()
    {
        BCanTransform = false;
        StoneGolem.SetActive(false);
        if (random.NextDouble() > 0.5f)
        {
            FireGolem.SetActive(true);
        }
        else
        {
            IceGolem.SetActive(true);
        }        
        yield return new WaitForSeconds(fIceOrFireTime);
        BCanTransform = true;
    }
    //時間到就變回石頭
    IEnumerator ExtinguishfireOrIce()
    {
        BCanTransform = false;
        StoneGolem.SetActive(true);
        FireGolem.SetActive(false);
        IceGolem.SetActive(false);
        yield return new WaitForSeconds(fStoneTime);
        BCanTransform = true;
    }

    /// <summary>
    /// 選擇我方陣營與敵對陣營
    /// </summary>
    void InitPlayerStats()
    {
        AttackerType = EAttackerType.MonsterGroup2;
        TakeDamageFrom = new List<EAttackerType> { EAttackerType.MonsterGroup1, EAttackerType.Player};
    }
}
