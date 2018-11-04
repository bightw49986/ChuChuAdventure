using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraSystem
{
    public partial class CameraController : MonoBehaviour
    {
        Transform m_Target;
        RaycastHit m_HitGround;
        float m_fGroundDetection = 3f;
        Vector3 m_vTargetPos;

        public void SetTarget(Transform ta)
        {
            if (ta == null) 
            {
                Debug.LogError("指派了Null給Camera當目標");
                return;
            }
            m_Target = ta;
            if (m_Target.GetComponent<InputMotionController>() == true)
            {
                CameraMode = CameraFollowingMode.Player;
                m_Mover = m_Target.GetComponent<InputMotionController>();
                m_Mover.Moved += OnTargetMove;
            }
            else
            {
                m_Mover.Moved -= OnTargetMove;
                CameraMode = CameraFollowingMode.NPC;
            }

        }

        Vector3 GetTargetPos(Transform ta)
        {
            if (ta == null)
            {
                Debug.LogError("攝影機沒有目標卻呼叫了檢查目標位置的函式");
                return Vector3.zero;
            }
            Ray ray = new Ray(ta.position + vTargetPosOffset, Vector3.down);
            return Physics.Raycast(ray, out m_HitGround, m_fGroundDetection, groundLayer) ? m_HitGround.point + vTargetPosOffset : m_vTargetPos;
        }
    }
}


