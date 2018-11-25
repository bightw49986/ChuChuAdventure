using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public partial class InputMotionController : MonoBehaviour,IMoveable
{
    Player player;
    Camera m_cam;
    Rigidbody m_rig;
    CapsuleCollider m_collider;

    public bool bDrawDebugLines;

    [Header("Input Settings")]
    public float m_fDeadZone = 0.1f;
    [HideInInspector] public string FORWARD_AXIS = "Vertical";
    [HideInInspector] public string TURN_AXIS = "Horizontal";
    [HideInInspector] public string JUMP_AXIS = "Jump";
    [HideInInspector] public string DASH_AXIS = "Dash";


    public float m_fVInput, m_fHInput, m_fJInput, m_fDInput;
    float m_fVInputAbs, m_fHInputAbs;
    public float m_fMoveInput;

    [Header("Conditions")]
    public bool bGrounded;
    public bool m_bJumpEnd;

    [Header("Move Settings")]
    [Tooltip("起步時的速度")]public float fStartSpeed = 3f;
    [Tooltip("全速前進的速度")] public float fFullSpeed = 6f;
    [Tooltip("目前的速度")] public float m_fMoveSpeed;
    [Tooltip("順移位移")] public float fDashOffset = 4f;
    [Tooltip("瞬移冷卻時間")] public float fDashCD = 1.5f;
    [Tooltip("跳躍速度")] public Vector3 vJumpVel = new Vector3(0, 9, 0);
    [Tooltip("用來看現在的速度量")]Vector3 m_velocity;
    [Tooltip("用來看現在的移動方向")]Vector3 m_vForward;

    [Header("Turn Settings")]
    [Tooltip("旋轉到指定方向所需時間")] [Range(0, 1)] public float fTurnSpeed = 0.38f;
    [Tooltip("目標旋轉量的四元數")] public Quaternion m_qTargetRotation;

    [Header("Physic Settings")]
    [Tooltip("重力")] public Vector3 vGravity = new Vector3(0,20,0);
    [Tooltip("GroundCheck射線的長度")] float m_fGroundOffset;
    [Tooltip("地板的Layer")] public LayerMask groundLayer;
    [Tooltip("GroundCheck射線打到的點(沒打到的話會是(0,0,0))")] RaycastHit groundHitInfo;
    [Tooltip("Collider的中心點位置")] Vector3 m_vCenter;

    [Tooltip("畫面的正前方")] Vector3 m_vCamForward;
    [Tooltip("畫面的正右方")] Vector3 m_vCamRight;
    [Tooltip("畫面的右前方")] Vector3 m_vCamRightForward;
    [Tooltip("畫面的左前方")] Vector3 m_vCamLeftForward;


    /*
========FixedUpdate=======================================
    */

    /// <summary>
    /// 從人物中心點朝下打一條射線檢查有沒有站在地上
    /// </summary>
    void GroundCheck()
    {
        m_vCenter = transform.position + Vector3.up * m_fGroundOffset;
        Ray groundCheckRay = new Ray(m_vCenter, Vector3.down);
        bGrounded = Physics.Raycast(groundCheckRay, out groundHitInfo, m_fGroundOffset, groundLayer);
        Ray jumpRay = new Ray(m_vCenter, Vector3.down);
        m_bJumpEnd = Physics.Raycast(jumpRay, out groundHitInfo, m_fGroundOffset * 1.5f, groundLayer);
        if (bDrawDebugLines)
        {
            //Vector3 vPredict = m_vCenter + Vector3.down * m_fGroundOffset;
            Vector3 vPredict2 = m_vCenter + Vector3.down * m_fGroundOffset * 1.5f;
            // Debug.DrawLine(m_vCenter, vPredict, bGrounded ? Color.cyan : Color.red);
            Debug.DrawLine(m_vCenter, vPredict2, bGrounded ? Color.cyan : Color.red);
        }
        if (bGrounded)
        {
            m_velocity.y = 0;
        }
        if (bGrounded == false)
        {
            ApplyGravity();
        }
    }

    /// <summary>
    /// 施加重力
    /// </summary>
    void ApplyGravity()
    {
        if (player.bIgnoreGravity == true)
        {
            m_velocity.y = -0.25f;
        }

        else m_velocity -= vGravity;
    }



    /// <summary>
    /// 如果可以移動，計算xz Velocity
    /// </summary>
    void Move()
    {
        if (player.bInputMove)
        {
            m_fMoveSpeed = Mathf.Lerp(fStartSpeed, fFullSpeed, m_fMoveInput);
            if (m_fVInputAbs > m_fDeadZone || m_fHInputAbs > m_fDeadZone) //有輸入且大於DeadZone
            {
                Vector3 vel = m_vForward * m_fMoveSpeed * m_fMoveInput;
                if (m_bJumpEnd == false)
                {
                    //空中移動用
                    m_velocity.x = vel.x * 0.6f;
                    m_velocity.z = vel.z * 0.6f;
                }
                else
                {
                    m_velocity.x = vel.x;
                    m_velocity.z = vel.z;
                }
            }
            else
            {
                m_velocity.z = m_velocity.x = 0f;
            }
        }
        else
        {
            //若是不准移動但准許轉向
            //if (player.bAdjustTurn)
            //    m_velocity.z = m_velocity.x = 0.001f;
            //else
            m_velocity.z = m_velocity.x = 0;
        }
    }

    /// <summary>
    /// 計算y軸Velocity處理跳躍輸入
    /// </summary>
    void Jump()
    {
        if (m_fJInput > 0 && bGrounded) //如果按下跳躍且在地上，增加y Velocity
        {
            m_velocity.y = vJumpVel.y;
            StartCoroutine(OnPlayerJump());
        }
    }

    /// <summary>
    /// 玩家跳起來時觸發事件(這邊是給攝影機拉遠用的，之後改成滯空開始時觸發更好)
    /// </summary>
    public event Action PlayerJumpStart;
    IEnumerator OnPlayerJump()
    {
        if (PlayerJumpStart != null)
            PlayerJumpStart();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => bGrounded == true);
        //Player.bCanJump = true;
        OnPlayerJumpEnd();
    }
    /// <summary>
    /// 玩家落地時也觸發一個事件
    /// </summary>
    public event Action PlayerJumpEnd;
    void OnPlayerJumpEnd()
    {
        if (PlayerJumpEnd != null)
            PlayerJumpEnd();
    }


    /// <summary>
    /// 做一個OnFixedUpdateDine事件確保Camera可以在Player物理運算完之後才做追蹤
    /// </summary>
   
    public event Action Moved;

    public virtual void OnMoved() //呼叫這個方法時，引發事件
    {
        if (Moved != null) //若有該事件的註冊者，通知他們事件發生
            Moved();
    }

    /*
========Update============================================
    */

    /// <summary>
    /// 把輸入存起來，漸進式的用GetAxis，按鍵式的用GetAxisRaw
    /// </summary>
    void GetInput()
    {
        if (player.bInputMove)
        {
            m_fVInput = CrossPlatformInputManager.GetAxis(FORWARD_AXIS);
            m_fVInputAbs = Mathf.Abs(m_fVInput);
            m_fHInput = CrossPlatformInputManager.GetAxis(TURN_AXIS);
            m_fHInputAbs = Mathf.Abs(m_fHInput);
            m_fMoveInput = Mathf.Clamp01(m_fVInputAbs + m_fHInputAbs);
        }
        else m_fVInput = m_fHInput = m_fVInputAbs = m_fHInputAbs = m_fMoveInput = 0;

        if (player.bInputJump && bGrounded)
        {
            m_fJInput = CrossPlatformInputManager.GetAxisRaw(JUMP_AXIS);
        }
        else m_fJInput = 0;

        if (player.bInputDash)
        {
            m_fDInput = CrossPlatformInputManager.GetAxisRaw(DASH_AXIS);
        }
        else m_fDInput = 0;
    }

    /// <summary>
    /// 計算現在螢幕的方向
    /// </summary>
    void CalcuelateDirections()
    {
        m_vCamForward = Vector3.ProjectOnPlane(m_cam.transform.forward, Vector3.up);
        m_vCamRight = Vector3.ProjectOnPlane(m_cam.transform.right, Vector3.up);
        m_vCamRightForward = m_vCamForward + m_vCamRight;
        m_vCamLeftForward = m_vCamForward - m_vCamRight;
    }

    /// <summary>
    /// 計算行進的方向
    /// </summary>
    void CalcuelateForward()
    {
        if (bGrounded == false) //如果沒在地上，行進等於面朝方向
        {
            m_vForward = transform.forward;
        }
        else //在地上的話，行進方向等於坡面方向
        {
            m_vForward = Vector3.Cross(transform.right, groundHitInfo.normal);
        }
        if (bDrawDebugLines)
            Debug.DrawLine(m_vCenter, m_vCenter + m_vForward, Color.green);
    }

    /// <summary>
    /// 處理旋轉
    /// </summary>
    void Turn()
    {
        if (m_fHInputAbs > m_fDeadZone && m_fVInputAbs < m_fDeadZone) //處理左右旋轉
        {
            if (m_fHInput > 0f) //純向右轉
            {
                m_qTargetRotation = Quaternion.LookRotation(m_vCamRight, Vector3.up);
            }
            else if (m_fHInput < 0f) //純向左轉
            {
                m_qTargetRotation = Quaternion.LookRotation(-m_vCamRight, Vector3.up);
            }
        }

        if (m_fVInputAbs > m_fDeadZone && m_fHInputAbs < m_fDeadZone) //處理前後旋轉
        {
            if (m_fVInput > 0f) //純向前轉
            {
                m_qTargetRotation = Quaternion.LookRotation(m_vCamForward, Vector3.up);
            }
            else if (m_fVInput < 0f) //純向後轉
            {
                m_qTargetRotation = Quaternion.LookRotation(-m_vCamForward, Vector3.up);
            }
        }

        else if (m_fHInputAbs > m_fDeadZone && m_fVInputAbs > m_fDeadZone)
        {
            if (m_fVInput > 0 && m_fHInput > 0f) //向右前轉
            {
                m_qTargetRotation = Quaternion.LookRotation(m_vCamRightForward, Vector3.up);
            }
            if (m_fVInput > 0 && m_fHInput < 0f) //向左前轉
            {
                m_qTargetRotation = Quaternion.LookRotation(m_vCamLeftForward, Vector3.up);
            }
            if (m_fVInput < 0 && m_fHInput > 0f) //向右後轉
            {
                m_qTargetRotation = Quaternion.LookRotation(-(m_vCamLeftForward), Vector3.up);
            }
            if (m_fVInput < 0 && m_fHInput < 0f) //向左後轉
            {
                m_qTargetRotation = Quaternion.LookRotation(-(m_vCamRightForward), Vector3.up);
            }
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, m_qTargetRotation, fTurnSpeed);
    }

    /// <summary>
    /// 瞬移 undone
    /// </summary>
    void Dash()
    {
        if (m_fDInput > 0 && player.bInputDash && Mathf.Abs(m_velocity.x + m_velocity.z) > 0.1f)
        {
            StartCoroutine(DashCD(fDashCD));
            //player.bInputMove = false;
            player.bInputJump = false;
        }
        if (aboutVFXGhostTrail.openGhostTrail == true) transform.position += transform.forward * Time.deltaTime * 7f;
    }

    float m_fTimer;
    AboutVFXGhostTrail aboutVFXGhostTrail;
    /// <summary>
    /// 執行瞬移閃避 undone
    /// </summary>
    /// <param name="fCoolDown">瞬移的冷卻時間</param>
    IEnumerator DashCD(float fCoolDown)
    {
        player.bInputDash = false;
        aboutVFXGhostTrail.openGhostTrail = true;
        yield return new WaitForSeconds(0.25f);
        //player.bInputMove = true;
        player.bInputJump = true;
        aboutVFXGhostTrail.openGhostTrail = false;
        yield return new WaitForSeconds(fCoolDown);
        player.bInputDash = true;
    }
}
