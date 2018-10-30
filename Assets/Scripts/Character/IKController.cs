using System;
using UnityEngine;

public class IKController : MonoBehaviour
{
    [Header("Conditions")]
    [SerializeField] [Tooltip("要不要調膝蓋")] bool bAdjustCalf;
    [SerializeField] [Tooltip("要不要調骨盆")] bool bAdjustPelvis;
    [SerializeField] [Tooltip("要不要平滑化過程")] bool bSmoothAdjust;
    [SerializeField] [Tooltip("要不要畫Debug線")] bool bDrawDebugLines;

    [Header("Pelvis")]
    [SerializeField] [Tooltip("拖曳左大腿的物件到這")] Transform pelvis;
    [Tooltip("骨盆IK前的位置")] Vector3 vPelvisOrigin;

    [Header("Left Leg")]
    [SerializeField] [Tooltip("拖曳左大腿的物件到這")] Transform leftLeg;
    [SerializeField] [Tooltip("拖曳左小腿的物件到這")] Transform leftCalf;
    [SerializeField] [Tooltip("拖曳左腳底板的物件到這")] Transform leftFoot;
    [SerializeField] [Tooltip("左腳IK位置的權重")][Range(0, 1)] float leftFootPositionWeight;
    [SerializeField] [Tooltip("左腳IK旋轉的權重")] [Range(0, 1)] float leftFootRotationWeight;
    [Tooltip("左膝校正的權重")] [Range(0, 1)] float leftKneePositionWeight;
    [Tooltip("左腳這一幀需不需要做IK檢查")] bool bLeftNeedIK;
    [Tooltip("左腳原本Local的y值")] float fLeftFootOriginY;
    [Tooltip("左腳射線打到的點")] RaycastHit leftIK; 
    [Tooltip("左腳IK修正後的位置(有算上腳高)")] Vector3 leftIKPoint;


    [Header("Right Leg")]
    [SerializeField] [Tooltip("拖曳右大腿的物件到這")] Transform rightLeg;
    [SerializeField] [Tooltip("拖曳右小腿的物件到這")] Transform rightCalf;
    [SerializeField] [Tooltip("拖曳右腳底板的物件到這")] Transform rightFoot;
    [SerializeField] [Tooltip("右腳IK位置的權重")] [Range(0, 1)] float rightFootPositionWeight;
    [SerializeField] [Tooltip("右腳IK旋轉的權重")] [Range(0, 1)] float rightFootRotationWeight;
    [Tooltip("右膝校正的權重")] [Range(0, 1)] float rightKneePositionWeight;
    [Tooltip("右腳這一幀需不需要做IK檢查")] bool bRightNeedIK;
    [Tooltip("右腳原本Local的y值")] float fRightFootOriginY;
    [Tooltip("右腳射線打到的點")] RaycastHit rightIK; 
    [Tooltip("拿來接右腳IK位置(有算上腳高)")] Vector3 rightIKPoint;

    [Header("IK Detection")]
    [SerializeField] [Tooltip("離地距離寬容範圍")] float fOffsetTorlerance = 0.3f;
    [SerializeField] [Tooltip("腳可以抬起的高度")]float fRayCastHeight = 0.5f;
    [SerializeField] [Tooltip("腳的位置離實際腳底板的落差")]float fFootHeight = 0.19f;
    [SerializeField] [Tooltip("內差偏向幅度")] [Range(0, 1)] float fLerpSmooth = 0.5f;

    Animator animator;
    [SerializeField]  LayerMask slopeLayer;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        CheckRefs();
    }

    void CheckRefs()
    {
        if (pelvis==null || leftFoot == null || rightFoot == null || leftLeg == null || rightLeg == null || leftCalf == null || rightCalf == null)
        {
            Debug.LogError("左右腳大小腿跟腳底其中有一個忘記拖了");
        }
    }

    void Update()
    {
        DetectPosBeforeIK();
    }

    void DetectPosBeforeIK()
    {
        fLeftFootOriginY = leftFoot.position.y + fFootHeight;
        fRightFootOriginY = rightFoot.position.y + fFootHeight;
        vPelvisOrigin = pelvis.transform.position;
    }


    void OnAnimatorIK()
    {
        RefreshIKInfo();
        ApplyFootIK();
    }

    void LateUpdate()
    {
        AdjustPelvis();
    }

    void AdjustPelvis()
    {
        if(bAdjustPelvis)
        {
            if(bLeftNeedIK || bRightNeedIK)
            {
                Vector3 vFixedPos = new Vector3(pelvis.transform.position.x, vPelvisOrigin.y, pelvis.transform.position.z);
                pelvis.transform.position = Vector3.Slerp(pelvis.transform.position, vFixedPos, fLerpSmooth);
            }
        }
          
       
    }

    void RefreshIKInfo()
    {
        bLeftNeedIK = CalculateLeftFootIK();
        bRightNeedIK = CalculateRightFootIK();

    }

    bool CalculateLeftFootIK()
    {
        Vector3 vRayStartPoint = leftFoot.position + Vector3.up * fRayCastHeight;
        float fRayLength = fOffsetTorlerance + fRayCastHeight;
        if (bDrawDebugLines)
        {
            Debug.DrawLine(vRayStartPoint, vRayStartPoint + Vector3.down * fRayLength, Color.white);
        }

        Ray ray = new Ray(vRayStartPoint, Vector3.down);

        if (Physics.Raycast(ray, out leftIK, fRayLength, slopeLayer))
        {
            if (bDrawDebugLines)
            {
                Debug.DrawLine(vRayStartPoint, leftIK.point, Color.green);
            }
            leftIKPoint = leftIK.point + Vector3.up * fFootHeight;

            return true;
        }
        return false;
    }

    bool CalculateRightFootIK()
    {
        Vector3 vRayStartPoint = rightFoot.position + Vector3.up * fRayCastHeight;
        float fRayLength = fOffsetTorlerance + fRayCastHeight;

        if (bDrawDebugLines)
        {
            Debug.DrawLine(vRayStartPoint, vRayStartPoint + Vector3.down * fRayLength, Color.white);
        }

        Ray ray = new Ray(vRayStartPoint, Vector3.down);
        if (Physics.Raycast(ray, out rightIK, fRayLength, slopeLayer))
        {
            if (bDrawDebugLines)
            {
                Debug.DrawLine(vRayStartPoint, rightIK.point, Color.green);
            }
            rightIKPoint = rightIK.point + Vector3.up * fFootHeight;
            return true;
        }
        return false;
    }

    void AdjustLeftCalf()
    {
        Vector3 lFoot = leftFoot.parent.position;
        Vector3 vLeftHint = leftIK.point + (leftCalf.position - lFoot);
        animator.SetIKHintPosition(AvatarIKHint.LeftKnee, vLeftHint);
        leftKneePositionWeight = bSmoothAdjust ? Mathf.Lerp(leftKneePositionWeight, 1f, fLerpSmooth) : 1f;
        animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneePositionWeight);
    }

    void AdjustRightCalf()
    {
        Vector3 rFoot = rightFoot.parent.position;
        Vector3 vRightHint = rightIK.point + (rightCalf.position - rFoot);
        animator.SetIKHintPosition(AvatarIKHint.RightKnee, vRightHint);
        rightKneePositionWeight = bSmoothAdjust ? Mathf.Lerp(rightKneePositionWeight, 1f, fLerpSmooth) : 1f;
        animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneePositionWeight);
    }

    void ApplyFootIK()
    {
        ApplyLeftFootIK();
        ApplyRightFootIK();
    }

    void ApplyLeftFootIK()
    {
        if(bLeftNeedIK)
        {
            float IKHeight = leftIKPoint.y;
            if(Mathf.Abs(IKHeight - fLeftFootOriginY) > 0)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftIKPoint);
                Vector3 vNewDirection = Vector3.Cross(leftFoot.forward, leftIK.normal);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(vNewDirection));
                leftFootRotationWeight = bSmoothAdjust ? (leftFootPositionWeight = Mathf.Lerp(leftFootPositionWeight, 1f, fLerpSmooth)) : (leftFootPositionWeight = 1f);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);
                if (bAdjustCalf)
                {
                    AdjustLeftCalf();
                }
                return;
            }
        }
        ResetLeftWeight();
    }

    void ResetLeftWeight()
    {
        leftKneePositionWeight = bSmoothAdjust
            ? (leftFootRotationWeight = leftFootPositionWeight = Mathf.Lerp(leftFootPositionWeight, 0f, fLerpSmooth))
            : (leftFootRotationWeight = leftFootPositionWeight = 0f);
        animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneePositionWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);
    }

    void ApplyRightFootIK()
    {
        if (bRightNeedIK)
        {
            float IKHeight = rightIKPoint.y;
            if ( Mathf.Abs(IKHeight - fRightFootOriginY) > 0)
            {
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightIKPoint);
                Vector3 vNewDirection = Vector3.Cross(rightFoot.forward, rightIK.normal);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(vNewDirection));
                rightFootRotationWeight = bSmoothAdjust ? (rightFootPositionWeight = Mathf.Lerp(rightFootPositionWeight, 1f, fLerpSmooth)) : (rightFootPositionWeight = 1f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootPositionWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootRotationWeight);
                if (bAdjustCalf)
                {
                    AdjustRightCalf();
                }
                return;
            }
        }
        ResetRightWeight();
    }

    void ResetRightWeight()
    {
        rightKneePositionWeight = bSmoothAdjust
            ? (rightFootRotationWeight = rightFootPositionWeight = Mathf.Lerp(rightFootPositionWeight, 0f, fLerpSmooth))
            : (rightFootRotationWeight = rightFootPositionWeight = 0f);
        animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneePositionWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootPositionWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootRotationWeight);
    }
}
