using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AISystem
{
    public class AIData : MonoBehaviour
    {
        public NpcFSM m_FSM;

        [Header("Range Settings")]
        public bool DrawCautionRange;
        public float fFaceCautionRange = 15f;
        [HideInInspector]public float fSqrFaceCautionRange;
        public float fBackCaurionRange = 7f;
        [HideInInspector] public float fSqrBackCaurionRange;
        public float FOV = 90f;

        public bool DrawChaseRange;
        public float fChaseRange = 10f;
        [HideInInspector] public float fSqrChaseRange;

        public bool DrawJumpAtkRange;
        public float fJumpAtkRange = 4f;
        [HideInInspector] public float fSqrJumpAtkRange;

        public bool DrawAtkRange;
        public float fAtkRange = 1.5f;
        [HideInInspector] public float fSqrAtkRange;

        [Header("Moving Info")]
        Vector3 vNextPos;
        float fBoundRadius;
        Vector3 vCenter;
        public LayerMask IgnoreLayer;
        float fTurnningWeight;

        [Header("Target Info")]
        public Player m_Player;
        public bool PlayerInSight;
        public bool IsInBattle;
        Transform Target;
        Vector3 vTargetPos;
        Vector3 vTargetPosLastFrame;
        Vector3 vTargetForward;
        Vector3 vDirectionToTarget;
        public float fTargetDis;
        public float fSqrTaDis;
        public float fTaDirDotForward;
        public float fAbsDot;

        [Header("PathFinding Info")]
        List<Waypoint> Path;
        public bool Patrolling;
        float fRestTime;
        Waypoint previousWP;
        Waypoint nextWP;

        [Header("Battle Info")]
        public int NextMoveID;
        public bool AtkReady = true;
        public bool JumpAtkReady = true;
        public float fAtkOffset = 0.5f;
        public float fAttackFrequency = 3f;
        public float fJumpAttackFrequency = 7f;

        void Awake()
        {

        }

        void Start()
        {
            m_FSM = GetComponent<NpcFSM>();
            InitStats();
            m_Player = GameObject.FindWithTag("Player").GetComponent<Player>();
            Target = m_Player.transform;
        }

        void Update()
        {
            UpdateTargetInfo();
        }

        void LateUpdate()
        {
            if (Target != null)
            vTargetPosLastFrame = vTargetPos;
        }

        void UpdateTargetInfo()
        {
            if (Target != null)
            {
                vTargetPos = Target.position;
                vTargetForward = Target.forward;
                vDirectionToTarget = vTargetPos - transform.position;
                fSqrTaDis = Vector3.SqrMagnitude(vDirectionToTarget);
                fTargetDis = Mathf.Sqrt(fSqrTaDis);
                fTaDirDotForward = Vector3.Dot(transform.forward, vDirectionToTarget.normalized);
                fAbsDot = Mathf.Abs(fTaDirDotForward);
                fTurnningWeight = (1 - fAbsDot) * (1 - fAbsDot);
            }
        }

        void InitStats()
        {
            vTargetPos = vTargetForward = vDirectionToTarget = vTargetPosLastFrame = Vector3.zero;
            fSqrFaceCautionRange = fFaceCautionRange * fFaceCautionRange;
            fSqrBackCaurionRange = fBackCaurionRange * fBackCaurionRange;
            fSqrChaseRange = fChaseRange * fChaseRange;
            fSqrJumpAtkRange = fJumpAtkRange * fJumpAtkRange;
            fSqrAtkRange = fAtkRange * fAtkRange;
        }

        void OnDrawGizmosSelected()
        {
            if(DrawCautionRange)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, fFaceCautionRange);
            }
            if(DrawChaseRange)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, fChaseRange);
            }
            if(DrawJumpAtkRange)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, fJumpAtkRange);
            }
            if(DrawAtkRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, fAtkRange);
            }
        }

        public void TurnToTarget()
        {
            m_FSM.m_Animator.SetLayerWeight(1,fTurnningWeight);
            m_FSM.m_Animator.SetFloat("fTurn", fTaDirDotForward);
            Vector3 steering = AIMethod2D.SeekTarget(transform.position, vTargetPos, m_FSM.m_Animator.velocity,5* Time.deltaTime);
            Vector3 desired = Vector3.ProjectOnPlane(m_FSM.m_Animator.velocity + steering,Vector3.up) .normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(desired),fAbsDot);
        }

        public void StopTurnToTarget()
        {
            m_FSM.m_Animator.SetLayerWeight(1, 0);
        }

        public bool SomethingShowUp()
        {
            return AIMethod2D.CheckinSightFan(transform, vTargetPos, fFaceCautionRange, FOV);
        }

        public bool TargetInChaseRange()
        {
            return fSqrTaDis <= fSqrChaseRange;
        }

        public bool TargetInJumpAtkRange()
        {
            return fSqrTaDis <= fSqrJumpAtkRange;
        }

        public bool TargetInAtkRange()
        {
            return fSqrTaDis <= fSqrAtkRange;
        }

        public bool NotFacingTarget()
        {
            return fTaDirDotForward != 1f;
        }

        public bool TargetStillInAtkRange()
        {
            return AIMethod2D.CheckinSightFan(transform, vTargetPos, fAtkRange + fAtkOffset, 180f);
        }

        public bool TargetOnRightSide()
        {
            return fTaDirDotForward > 0;
        }

        public void EnterBattle()
        {
            PlayerInSight = true;
            IsInBattle = true;
        }

        public void LookAtTarget()
        {
            transform.LookAt(vTargetPos,Vector3.up);
        }
        public void ChaseTarget()
        {

        }

        public void Attack()
        {
            StartCoroutine(AtkCD(fAttackFrequency));
        }

        IEnumerator AtkCD(float fCD)
        {
            AtkReady = false;
            yield return new WaitForSeconds(fCD);
            AtkReady = true;
        }

        public void JumpAttack()
        {
            StartCoroutine(JumpAtkCD(fJumpAttackFrequency));
        }

        IEnumerator JumpAtkCD(float fCD)
        {
            JumpAtkReady = false;
            yield return new WaitForSeconds(fCD);
            JumpAtkReady = true;

        }
    }
}


