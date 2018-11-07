using UnityEngine;

public partial class InputMotionController : MonoBehaviour,IMoveable
{
    void Awake()
    {
        player = GetComponent<Player>();
        m_rig = GetComponent<Rigidbody>();
        m_cam = Camera.main;
        m_collider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        m_qTargetRotation = transform.rotation;
        m_fGroundOffset = m_collider.center.y;
        vGravity *= Time.deltaTime;
    }

    void FixedUpdate()
    {
        GroundCheck();
        Jump();
        Move();
        m_rig.velocity = m_velocity;
        OnMoved();
    }

    void Update()
    {
        GetInput();
        CalcuelateDirections();
        CalcuelateForward();
        Turn();
        Dash();
    }

    void OnDrawGizmos()
    {
        if (bDrawDebugLines)
        {
            Gizmos.color = bGrounded ? Color.cyan : Color.white;
            Gizmos.DrawWireSphere(groundHitInfo.point, 0.1f);
        }
    }
}
