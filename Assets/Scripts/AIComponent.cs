using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIComponent : MonoBehaviour 
{
    //與Rigidbody相關的資訊，這個Class負責將Rigidbody的Velocity限制在自訂的速度範圍內

    public Rigidbody m_rig { get; private set; } //這個AI操控的Rigidbody
    public float m_fMass { get; private set; } //這個Rigidbody的質量(Mass)
    public bool m_bModifySpeed; //決定是否要讓這個AI Component主動限制Rigidbody的Velocity(注意如果為真，將失去設定m_fSpeed的功能，因為該屬性會在每幀由Rigidbody的Velocity換算過來)
    public Vector3 m_Velocity { get; private set; } //這個Rigidbody的當前速度向量(注意單位是每秒了所以不用再乘上deltaTime)
    public float m_fSpeed { get; private set; } //這個Rigidbody目前速度向量的長度值(不是角色的移動速度)
    public float m_fMaxSpeed { get; private set; } //這個Rigidbody的最大速度向量長度
    private float m_sqrMaxSpeed; //最大速度的平方，用來檢查Rigidbody的速度平方有沒有超過最大速度平方
    public float m_fMaxAcceleration { get; private set; } //最大加速度的長度，當賦予這個Rigidbody力道時，限制單一力道(加速度，例如Steering)的長度

    //碰撞偵測相關資訊(施工中)
    public float m_fProbeLength = 10f; //探針的長度
    public float m_fRadius; //偵測的範圍半徑
    public float m_fDotLastFrame; //上個frame時，Forward向量跟到障礙物的向量的內積
    public Transform m_NearestObstacle; //最近的障礙物


    void Awake()
    {
        m_rig = GetComponent<Rigidbody>();
        m_fMass = m_rig.mass;
        SetMaxSpeed(m_fMaxSpeed); //初始化最大速度
    }

    void FixedUpdate()
    {
        ModifySpeedInfo(); //限制剛體Velocity，使其合乎最大速度，並將該速度向量轉換成m_fSpeed速度資訊
    }


    /// <summary>
    /// 設定最大速度值，由於速度在Rigidbody中的單位是以秒計算，因此乘上Time.deltaTime，然後把它的平方也存起來，供比較時節省運算量
    /// </summary>
    /// <param name="fMaxSpeed">F max speed.</param>
    protected void SetMaxSpeed(float fMaxSpeed) 
    {
        m_fMaxSpeed = fMaxSpeed * Time.deltaTime;
        m_sqrMaxSpeed = m_fMaxSpeed * m_fMaxSpeed;
    }

    /// <summary>
    /// 設定每秒的最大速度變化量值
    /// </summary>
    /// <param name="fMaxAcceleration">F max acceleration.</param>
    protected void SetMaxAcceleration(float fMaxAcceleration)
    {
        m_fMaxAcceleration = fMaxAcceleration * Time.deltaTime;
    }

    /// <summary>
    /// 將目前剛體的速度與最大速度做比較，將其限制在速度範圍內
    /// </summary>
    void ClampRigVelocity()
    {
        Vector3 vel = m_rig.velocity;
        if (vel.sqrMagnitude > m_sqrMaxSpeed)
            m_rig.velocity = Vector3.ClampMagnitude(vel, m_fMaxSpeed);
    }
    /// <summary>
    /// 如果bModifySpeed為真的話，調整剛體速度，並將其速度轉成速率資訊
    /// </summary>
    void ModifySpeedInfo()
    {
        if(m_bModifySpeed)
        {
            ClampRigVelocity();
            m_Velocity = m_rig.velocity;
            m_fSpeed = m_Velocity.magnitude;
        }
    }

    /// <summary>
    /// 當你希望手動設置AI速度而不希望透過物理運算時，呼叫這個方法來Set m_fSpeed
    /// </summary>
    /// <param name="fSpeed">每秒的速率.</param>
    protected void SetSpeed(float fSpeed)
    {
        if (m_bModifySpeed == false)
            m_fSpeed = fSpeed * Time.deltaTime;
        else Debug.LogError("m_bModifySpeed為true時，無法主動設定m_fSpee ");
    }
}
