using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
using UnityEngine;

namespace CameraSystem
{
    public partial class CameraController : MonoBehaviour
    {
        float m_fHInput, m_fVInput, m_fResetInput, m_fZoomInput;
        float m_fHInputAbs, m_fVInputAbs;
        float m_fOriginXRotation, m_fOriginTargetDist;
        float fAdjustedDistance, fFinalDist, fFinalXRotation;
        Vector3 m_vDest;
        Vector3 m_vAdjustedDest;
        Vector3 m_vLastDest;
        Vector3 m_vDestBeforeLast;
        float m_fDotLast, m_fDotThis;

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
            m_fOriginXRotation = m_bColliding &&fAdjustedDistance < fMinZoom ? fFinalXRotation: fXRotation;
        }

        void ProcessZoom()
        {

            fDistFromTarget += m_fZoomInput * fZoomSpeed;
            fDistFromTarget = Mathf.Clamp(fDistFromTarget, fMinZoom, fMaxZoom);
            m_fOriginTargetDist =m_bColliding ? fFinalDist: fDistFromTarget;           
        }

        void CalculateDestination()
        {
            m_vTargetPos = GetTargetPos(m_Target);
            m_vDest = m_vTargetPos + Quaternion.Euler(m_fOriginXRotation, fYRotation, 0) * Vector3.back * m_fOriginTargetDist;
        }

        void ResetRotation()
        {
            fXRotation = Mathf.LerpAngle(fXRotation, 0f, fSwitchTargetSpeed);
            fYRotation = Mathf.LerpAngle(fYRotation, -180, fSwitchTargetSpeed);
        }

        void MoveToFinalDest()
        {
            m_fDotLast = Vector3.Dot(m_vLastDest, m_vDestBeforeLast);
            m_fDotThis = Vector3.Dot(m_vAdjustedDest, m_vLastDest);
            if (m_fDotThis * m_fDotLast < 0)
            {
                RefreshDestination();
                return;
            }

            if (bApplySmoothDamp)
            {
                Vector3 vel = Vector3.zero;
                Vector3 vRealDestination = Vector3.zero;
                if (m_vAdjustedDest == m_vLastDest)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, m_vAdjustedDest, ref vel, fFollowSmooth * Time.deltaTime);
                }
                else
                {
                    if (m_fVInputAbs == 0 && m_bColliding == false)
                    {
                        float flo = 0;
                        fXRotation = Mathf.SmoothDamp(fXRotation, 0f, ref flo, fAdjustTime);
                    }
                    vRealDestination = m_vAdjustedDest + (m_vLastDest - m_vAdjustedDest) * 0.5f;
                    transform.position = Vector3.SmoothDamp(transform.position, vRealDestination, ref vel, fFollowSmooth * Time.deltaTime);
                }
                RefreshDestination();
            }
            else
            {
                transform.position = m_vAdjustedDest;
            }
        }

        void RefreshDestination()
        {
            m_vDestBeforeLast = m_vLastDest;
            m_vLastDest = m_vAdjustedDest; 
        }

        void LookAtTarget()
        {
            Quaternion qTargetQuaternion = Quaternion.LookRotation(m_vTargetPos - transform.position);
            transform.rotation = qTargetQuaternion;
        }
    }
}

