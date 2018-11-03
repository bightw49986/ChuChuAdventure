using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraSystem
{
    public partial class CameraController : MonoBehaviour
    {
        Camera m_Cam;
        IMoveable m_Mover;
        Transform m_Player;
        bool DRAWDESIRED,DRAWDESTLAST, DRAWDESTTHIS;

        void InitCamera()
        {
            m_Cam = Camera.main;
            m_Player = GameObject.FindGameObjectWithTag("Player").transform;
            SetTarget(m_Player);
            InitSpeed();
            InitPosition();
            InitCollisionHandler();
            InitDebugLines();
        }

        void InitPosition()
        {
            m_vTargetPos = m_Target.position + vTargetPosOffset;
            m_vDestBeforeLast = transform.position;
            m_vLastDest = m_vTargetPos + Quaternion.Euler(0, -180, 0) * Vector3.back * fDistFromTarget;
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
            DRAWDESTLAST = bDrawDestinationLastFrame;
            DRAWDESTTHIS = bDrawDestinationThisFrame;
        }

        void InitCollisionHandler()
        {
            m_vAdjustedClipPoints = new Vector3[5];
            m_vDesiredClipPoints = new Vector3[5];
            RefreshCameraClipPoints(m_vDest, transform.rotation, ref m_vDesiredClipPoints);
            gameObject.GetComponent<SphereCollider>().radius = fRadius;
        }

        //void OnPlayerJumpStart()
        //{
        //    m_InputController.PlayerJumpEnd += OnPlayerJumpEnd;
        //    fDistanceFromTarget += fJumpZoom;
        //}

        //void OnPlayerJumpEnd()
        //{
        //    fDistanceFromTarget -= fJumpZoom;
        //    m_InputController.PlayerJumpEnd -= OnPlayerJumpEnd;
        //}

        void DrawDebugLines()
        {
            if (DRAWDESIRED)
            {
                for (int i = 0; i < 5; i++)
                {
                    Debug.DrawLine(m_vTargetPos, m_vDesiredClipPoints[i], Color.green);
                }
            }

            if (DRAWDESTTHIS)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(m_vDest, 0.1f);
            }
            if (DRAWDESTLAST)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(m_vDestBeforeLast, 0.1f);
            }
        }
    }
}

