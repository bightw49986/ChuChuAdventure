using UnityEngine;


namespace CameraSystem
{
    public enum CameraFollowingMode {Player = 0, NPC = 1 }

    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public partial class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("攝影機現在的跟隨模式")] public CameraFollowingMode CameraMode;

        [Header("Input Settings")]
        [Tooltip("鏡頭橫擺的DeadZone")] public float fPanDeadZone = 0.4f;
        [Tooltip("鏡頭縱擺的DeadZone")] public float fTiltDeadZone = 0.4f;
        [Tooltip("鏡頭橫擺在Input Setting當中的欄位")] public string CAMERA_PAN_AXIS = "Camera Pan";
        [Tooltip("鏡頭縱擺在Input Setting當中的欄位")] public string CAMERA_TILT_AXIS = "Camera Tilt";
        [Tooltip("鏡頭重置在Input Setting當中的欄位")] public string CAMERA_RESET_AXIS = "Camera Reset";
        [Tooltip("鏡頭縮放在Input Setting當中的欄位")] public string CAMERA_ZOOM_AXIS = "Mouse ScrollWheel";

        [Header("Position Settings")]
        [Tooltip("注視點在目標位置的偏移量")] public Vector3 vTargetPosOffset = new Vector3(0f, 1.5f, 0f);
        [Tooltip("鏡頭距離注視點的距離")] public float fDistFromTarget = 4f;
        [Tooltip("人物跳躍時的鏡頭縮放")] public float fJumpZoom = 0f;
        [Tooltip("鏡頭拉近離注視點最短的距離")] public float fMinZoom = 1.5f;
        [Tooltip("鏡頭拉遠離注視點最近的距離")] public float fMaxZoom = 6f;
        [Tooltip("鏡頭縮放的速度")] public float fZoomSpeed = 100f;
        [Tooltip("尾隨鏡頭是否套用平滑效果")] public bool bApplySmoothDamp = true;
        [Tooltip("尾隨鏡頭的平滑度(數值越大越平滑)")] public float fFollowSmooth = 3f;
        [Tooltip("鏡頭切換目標的速度")] public float fSwitchTargetSpeed = 100f;
        [Tooltip("目標可以踩的地板Layer")] public LayerMask groundLayer;

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

        [Header("Draw Debug Line")]
        [Tooltip("畫出目的地的視野金字塔")] public bool bDrawDesiredCollisionLines;
        [Tooltip("畫出這一幀的目的地")] public bool bDrawDestinationThisFrame;
        [Tooltip("畫出上一幀的目的地")] public bool bDrawDestinationLastFrame;

        void Start()
        {
            InitCamera();
        }

        void Update()
        {
            GetInput();
        }

        void OnTargetMove()
        {
            OrbitTarget();
            ProcessZoom();
            CalculateDestination();
            RefreshCameraClipPoints(m_vDest, transform.rotation, ref m_vDesiredClipPoints);
            CheckColliding(m_vTargetPos);
            AdjustDestination();
            MoveToFinalDest();
            LookAtTarget();
        }

        void OnDrawGizmos()
        {
            DrawDebugLines();
        }

        void OnTriggerStay(Collider collision)
        {
            TransparentObstacle(collision);
        }
        void OnTriggerExit(Collider collision)
        {
            ReverseTransparency(collision);
        }

    }

}


