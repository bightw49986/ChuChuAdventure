using System;
using System.Collections.Generic;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    Camera m_Cam;
    InputMotionController m_InputController;
    public Transform Target;
    Transform Player;


    [Header("Input Settings")]
    [Tooltip("鏡頭橫擺的DeadZone")] public float fPanDeadZone = 0.1f;
    [Tooltip("鏡頭縱擺的DeadZone")] public float fTiltDeadZone = 0.1f;
    [Tooltip("鏡頭橫擺在Input Setting當中的欄位")] public string CAMERA_PAN_AXIS = "Camera Pan";
    [Tooltip("鏡頭縱擺在Input Setting當中的欄位")] public string CAMERA_TILT_AXIS = "Camera Tilt";
    [Tooltip("鏡頭重置在Input Setting當中的欄位")] public string CAMERA_RESET_AXIS = "Camera Reset";
    [Tooltip("鏡頭縮放在Input Setting當中的欄位")] public string CAMERA_ZOOM_AXIS = "Mouse ScrollWheel";

    float m_fHInput, m_fVInput, m_fResetInput, m_fZoomInput;
    float m_fHInputAbs, m_fVInputAbs;

    [Header("Position Settings")]
    [Tooltip("注視點在目標位置的偏移量")] public Vector3 vTargetPosOffset = new Vector3(0f, 0.8f, 0f);
    [Tooltip("鏡頭距離注視點的距離")] public float fDistanceFromTarget = 4f;
    [Tooltip("人物跳躍時的鏡頭縮放")] public float fJumpZoom = 1.5f;
    [Tooltip("鏡頭拉近離注視點最短的距離")] public float fMinZoom = 2f;
    [Tooltip("鏡頭拉遠離注視點最近的距離")] public float fMaxZoom = 6f;
    [Tooltip("鏡頭縮放的速度")] public float fZoomSpeed = 100f;
    [Tooltip("尾隨鏡頭是否套用平滑效果")] public bool bApplySmoothDamp = true;
    [Tooltip("尾隨鏡頭的平滑度(數值越大越平滑)")] public float fFollowSmooth = 3f;
    [Tooltip("鏡頭切換目標的速度")] public float fSwitchTargetSpeed = 100f;
    [Tooltip("目標可以踩的地板Layer")] public LayerMask groundLayer;

    RaycastHit m_HitGround;
    float m_fGroundDetection = 5f;
    Vector3 m_vTargetPos;
    Vector3 m_vDestinationThisFrame;
    Vector3 m_vDestinationLastFrame;
    Vector3 m_vDestinationBeforeLastFrame;
    float m_fDotLast, m_fDotThis;

    [Header("Rotation Settings")]
    [Tooltip("攝影機相對於注視點的X旋轉值")] public float fXRotation = 0f;
    [Tooltip("攝影機相對於注視點的Y旋轉值")] public float fYRotation = 0f;
    [Tooltip("最低仰角")] public float fMinXRotation = 0f;
    [Tooltip("最大俯角")] public float fMaxXRotation = 40f;
    [Tooltip("縱向環繞的速度")] public float fXRotationSpeed = 50f;
    [Tooltip("橫向環繞的速度")] public float fYRotationSpeed = 50f;
    [Tooltip("沒有旋轉輸入時的回正時間")] public float fAdjustTime = 1f;

    [Header("Collision Handleing")]
    [Tooltip("攝影機要迴避的障礙物Layer")] public LayerMask CollisionLayer;
    [Tooltip("透明化偵測半徑")] public float fRadius = 0.5f;
    [Tooltip("透明化障礙物所需的時間")] public float fFadeTime = 0.5f;
    [Tooltip("透明化障礙物的透明度")] [Range(0, 1)] public float fAlpha = 0.4f;
    bool m_bColliding;
    float m_fAdjustedRotationX, m_fOriginRotationX, m_fAdjustedDistance, m_fOriginTargetDistance;
    Vector3[] m_vAdjustedClipPoints, m_vDesiredClipPoints;

    [Header("Draw Debug Line")]
    [Tooltip("畫出目的地的視野金字塔")] public bool bDrawDesiredCollisionLines;
    [Tooltip("畫出當前視野金塔")] public bool bDrawAdjustedCollisionLines;
    [Tooltip("畫出這一幀的目的地")] public bool bDrawDestinationThisFrame;
    [Tooltip("畫出上一幀的目的地")] public bool bDrawDestinationLastFrame;
    bool DRAWDESIRED, DRAWADJUSTED, DRAWDESTLAST, DRAWDESTTHIS;


    void OnDisable()
    {
        m_InputController.FixedUpdateDone -= OnFixedUpdateDone;
    }


    void Start()
    {
        InitCamera();
    }

    void InitCamera()
    {
        m_Cam = Camera.main;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        SetTarget(Player);
        InitSpeed();
        m_vTargetPos = Target.position + vTargetPosOffset;
        m_vDestinationBeforeLastFrame = transform.position;
        m_vDestinationLastFrame = m_vTargetPos + Quaternion.Euler(0, -180, 0) * Vector3.back * fDistanceFromTarget;
        SphereCollider col = gameObject.GetComponent<SphereCollider>();
        col.radius = fRadius;
        m_InputController.PlayerJumpStart += OnPlayerJumpStart;
        InitCollisionHandler(Camera.main);
        InitDebugLines();
    }

    void InitSpeed()
    {
        fAdjustTime *= Time.deltaTime;
        fZoomSpeed *= Time.deltaTime;
        fXRotationSpeed *= Time.deltaTime;
        fYRotationSpeed *= Time.deltaTime;
        fSwitchTargetSpeed *= Time.deltaTime;
    }

    void InitDebugLines()
    {
        DRAWDESIRED = bDrawDesiredCollisionLines;
        DRAWADJUSTED = bDrawAdjustedCollisionLines;
        DRAWDESTLAST = bDrawDestinationLastFrame;
        DRAWDESTTHIS = bDrawDestinationThisFrame;
    }

    void OnPlayerJumpStart()
    {
        m_InputController.PlayerJumpEnd += OnPlayerJumpEnd;
        fDistanceFromTarget +=fJumpZoom;
    }

    void OnPlayerJumpEnd()
    {
        fDistanceFromTarget -= fJumpZoom;
        m_InputController.PlayerJumpEnd -= OnPlayerJumpEnd;
    }


   

    public void SetTarget(Transform ta)
    {
        Target = ta;
        if (Target != null)
        {
            if (Target.GetComponent<InputMotionController>() == true)
            {
                m_InputController = Target.GetComponent<InputMotionController>();
                m_InputController.FixedUpdateDone += OnFixedUpdateDone;
            }
            else
            {
                Debug.LogError("The target needs a Input - Motion Controller.");
            }
        }
        else
        {
            Debug.LogError("Your camera needs a target.");
        }
    }

    Vector3 FindTargetsGround(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("你沒有目標卻呼叫了檢查目標位置的函式");
            return Vector3.zero;
        }
        Ray ray = new Ray(target.position + vTargetPosOffset, Vector3.down);
        return Physics.Raycast(ray, out m_HitGround, m_fGroundDetection, groundLayer) ? m_HitGround.point + vTargetPosOffset : m_vTargetPos;
    }


    void Update()
    {
        GetInput();
        Zoom();
    }

    void GetInput()
    {
        m_fHInput = CrossPlatformInputManager.GetAxis(CAMERA_PAN_AXIS);
        m_fHInputAbs = Mathf.Abs(m_fHInput);
        m_fVInput = CrossPlatformInputManager.GetAxis(CAMERA_TILT_AXIS);
        m_fVInputAbs = Mathf.Abs(m_fVInput);
        m_fResetInput = CrossPlatformInputManager.GetAxisRaw(CAMERA_RESET_AXIS);
        m_fZoomInput = CrossPlatformInputManager.GetAxis(CAMERA_ZOOM_AXIS);
    }

    void OrbitTarget()
    {
        if (m_fResetInput > 0)
        {
            ResetRotation();
            return;
        }
        if (m_fVInputAbs > fTiltDeadZone)
        {
            fXRotation += -m_fVInput * fXRotationSpeed;
        }
            
        if (m_fHInputAbs > fPanDeadZone)
        {
            fYRotation += m_fHInput * fYRotationSpeed;
        }
        fXRotation = Mathf.Clamp(fXRotation, fMinXRotation, fMaxXRotation);
    }

    void ResetRotation()
    {
        fXRotation = Mathf.LerpAngle(fXRotation, 0f, fSwitchTargetSpeed );
        fYRotation = Mathf.LerpAngle(fYRotation, -180, fSwitchTargetSpeed );
    }

    void Zoom()
    {
        fDistanceFromTarget += m_fZoomInput * fZoomSpeed;
        fDistanceFromTarget = Mathf.Clamp(fDistanceFromTarget, fMinZoom, fMaxZoom);
    }

    void OnFixedUpdateDone()
    {
        CalculateDestination(Target);
        MoveToDestination();
        LookAtTarget(Target);
        OrbitTarget();
        RefreshCameraClipPoints(transform.position, transform.rotation, ref m_vAdjustedClipPoints);
        RefreshCameraClipPoints(m_vDestinationThisFrame, transform.rotation, ref m_vDesiredClipPoints);
        CheckColliding(m_vTargetPos);
        AdjustDistance();
    }

    void CalculateDestination(Transform target)
    {
        m_vTargetPos = FindTargetsGround(target);
        if (m_bColliding)
        {
            m_vDestinationThisFrame = Quaternion.Euler(m_fAdjustedRotationX, fYRotation, 0) * Vector3.back * m_fAdjustedDistance;
            m_vDestinationThisFrame += m_vTargetPos;
        }

        else
        {
            m_vDestinationThisFrame = Quaternion.Euler(fXRotation, fYRotation, 0) * Vector3.back * fDistanceFromTarget;
            m_vDestinationThisFrame += m_vTargetPos;
        }

        m_fDotLast = Vector3.Dot(m_vDestinationLastFrame, m_vDestinationBeforeLastFrame);
        m_fDotThis = Vector3.Dot(m_vDestinationThisFrame, m_vDestinationLastFrame);
    }

    void MoveToDestination()
    {
        if (m_fDotThis<0||m_fDotLast<0) 
        {
            RefreshDestination();
            return;
        }

        if (bApplySmoothDamp)
        {
            Vector3 vel = Vector3.zero;
            Vector3 vRealDestination = Vector3.zero;
            if (m_vDestinationThisFrame == m_vDestinationLastFrame)
            {
                transform.position = Vector3.SmoothDamp(transform.position, m_vDestinationThisFrame, ref vel, fFollowSmooth * Time.deltaTime);
            }
            else
            {
                if(Mathf.Abs(m_fVInput)==0)
                {
                    float flo = 0;
                    fXRotation = Mathf.SmoothDamp(fXRotation, 0f, ref flo, fAdjustTime);
                }

                vRealDestination = m_vDestinationThisFrame + (m_vDestinationLastFrame - m_vDestinationThisFrame) * 0.5f;
                transform.position = Vector3.SmoothDamp(transform.position, vRealDestination, ref vel, fFollowSmooth * Time.deltaTime);
            }
            RefreshDestination();
        }
        else
        {
            transform.position = m_vDestinationThisFrame;
        }
    }

    void RefreshDestination()
    {
        m_vDestinationBeforeLastFrame = m_vDestinationLastFrame;
        m_vDestinationLastFrame = m_vDestinationThisFrame;
    }

    void LookAtTarget(Transform target)
    {
        Vector3 vTargetPos = target.position + vTargetPosOffset;
        Quaternion qTargetQuaternion = Quaternion.LookRotation(vTargetPos - transform.position);
        transform.rotation = qTargetQuaternion;
    }

    void InitCollisionHandler(Camera cam)
    {
        m_Cam = cam;
        m_vAdjustedClipPoints = new Vector3[5];
        m_vDesiredClipPoints = new Vector3[5];
        m_fOriginRotationX = fXRotation;
        m_fOriginTargetDistance = fDistanceFromTarget;
        RefreshCameraClipPoints(transform.position, transform.rotation, ref m_vAdjustedClipPoints);
        RefreshCameraClipPoints(m_vDestinationThisFrame, transform.rotation, ref m_vDesiredClipPoints);
    }

    void RefreshCameraClipPoints(Vector3 vCameraPosition, Quaternion qCameraRotation, ref Vector3[] vIntoArray)
    {
        if (m_Cam == null)
        {
            Debug.LogWarning("Camera missing!");
            return;
        }
        //清空ClipPoint array
        vIntoArray = new Vector3[5];

        //算出每個ClipPoint
        float z = m_Cam.nearClipPlane;
        float x = Mathf.Tan(m_Cam.fieldOfView / 3.41f) * z;
        float y = x / m_Cam.aspect;

        //左上
        vIntoArray[0] = qCameraRotation * new Vector3(-x, y, z) + vCameraPosition;
        //左下
        vIntoArray[1] = qCameraRotation * new Vector3(x, y, z) + vCameraPosition;
        //右上
        vIntoArray[2] = qCameraRotation * new Vector3(-x, -y, z) + vCameraPosition;
        //右下
        vIntoArray[3] = qCameraRotation * new Vector3(x, -y, z) + vCameraPosition;
        //攝影機位置
        vIntoArray[4] = vCameraPosition - transform.forward * z;
    }

    bool CollisionDetectedAtClipPoints(Vector3[] vClipPoints, Vector3 vTargetPosition)
    {
        //ClipPoint的四個角跟中心點都要能夠看到目標，依序做射線檢查
        for (int i = 0; i < vClipPoints.Length; i++)
        {
            Ray ray = new Ray(vTargetPosition, vClipPoints[i] - vTargetPosition);
            float fDistance = Vector3.Distance(vClipPoints[i], vTargetPosition);
            if (Physics.Raycast(ray, fDistance, CollisionLayer))
            {
                return true;
            }
        }
        return false;
    }

    float GetAdjustedDistanceFromTarget(Vector3 vTarget)
    {
        //有障礙物的話，回傳最短沒有障礙物的距離

        float fDistance = -1f;
        for (int i = 0; i < m_vDesiredClipPoints.Length; i++)
        {
            Vector3 vHitDirection = m_vDesiredClipPoints[i] - vTarget;
            Ray ray = new Ray(vTarget, vHitDirection);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, vHitDirection.magnitude, CollisionLayer))
            {
                if (fDistance == -1)
                {
                    fDistance = hitInfo.distance;
                }
                else
                {
                    if (hitInfo.distance < fDistance)
                    {
                        fDistance = hitInfo.distance;
                    }
                }
            }
        }
        if (fDistance == -1) return fDistanceFromTarget;

        return fDistance;
    }

    void CheckColliding(Vector3 vTargetPosition)
    {
        if (CollisionDetectedAtClipPoints(m_vDesiredClipPoints, vTargetPosition) == true)
        {
            if (m_bColliding == false)
            {
                OnObstacleDetected();
            }
            m_bColliding = true;

        }
        else
        {
            m_bColliding = false;
        }
    }


    void OnObstacleDetected()
    {
        StartCoroutine(WaitForNotColliding());
    }

    IEnumerator WaitForNotColliding()
    {
        m_fOriginRotationX = fXRotation;
        m_fOriginTargetDistance = fDistanceFromTarget;
        yield return new WaitUntil(() => m_bColliding==false);
        fXRotation = m_fOriginRotationX;
        fDistanceFromTarget = m_fOriginTargetDistance;
    }

    void AdjustDistance()
    {
        if (m_bColliding == true)
        {
            m_fAdjustedDistance = Mathf.Min(GetAdjustedDistanceFromTarget(m_vTargetPos), fMaxZoom);
            m_fAdjustedRotationX = m_fAdjustedDistance >= fMinZoom
                ? Mathf.Clamp(fXRotation + 30f, fMinXRotation, fMaxXRotation)
                : fXRotation;
            if (m_fAdjustedDistance < fMinZoom) m_fAdjustedDistance = fMinZoom;
        }
    }



    void OnDrawGizmos()
    {
        if (DRAWDESIRED)
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.DrawLine(m_vTargetPos, m_vDesiredClipPoints[i], Color.green);
            }
        }
        if (DRAWADJUSTED)
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.DrawLine(m_vTargetPos, m_vAdjustedClipPoints[i], Color.cyan);
            }
        }
        if (DRAWDESTTHIS)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_vDestinationThisFrame, 0.1f);
        }
        if (DRAWDESTLAST)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_vDestinationBeforeLastFrame, 0.1f);
        }
    }

    void OnTriggerStay(Collider collision)
    {
        float dist = Vector3.Distance(collision.gameObject.transform.position, transform.position);
        Renderer[] renderers;
        renderers = collision.gameObject.GetComponentsInChildren<Renderer>();
        if (renderers != null)
        {
            HashSet<Material> materials = new HashSet<Material>();
            foreach (var r in renderers)
            {
                materials.Add(r.material);
            }

            foreach (var m in materials)
            {
                if (m.HasProperty("_Color"))
                {
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    m.SetInt("_ZWrite", 0);
                    m.DisableKeyword("_ALPHATEST_ON");
                    m.DisableKeyword("_ALPHABLEND_ON");
                    m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    m.renderQueue = 3000;
                    float taAlpha = Mathf.Lerp(1f, fAlpha, dist / fRadius);
                    float offset = taAlpha - m.color.a;
                    if (offset != 0)
                    {
                        float alpha = m.color.a + offset * fFadeTime * Time.deltaTime;
                        m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
                    }
                    else
                        m.color = new Color(m.color.r, m.color.g, m.color.b, taAlpha);
                }
            }
        }
    }

    IEnumerator FadeBack(Renderer[] renderers)
    {
        HashSet<Material> materials = new HashSet<Material>();
        foreach (var r in renderers)
        {
            materials.Add(r.material);
        }
        foreach (var m in materials)
        {
            if (m.HasProperty("_Color"))
            {
                while (m.color.a != 1f)
                {
                    float alpha = m.color.a + 10 * Time.deltaTime;
                    if (alpha > 1) alpha = 1;
                    m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                m.SetInt("_ZWrite", 1);
                m.DisableKeyword("_ALPHATEST_ON");
                m.DisableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = -1;
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        Renderer[] renderers;
        renderers = collision.gameObject.GetComponentsInChildren<Renderer>();
        if (renderers != null)
        {
            StartCoroutine(FadeBack(renderers));
        }
    }
}

