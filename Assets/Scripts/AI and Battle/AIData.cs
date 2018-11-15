using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;


namespace AISystem
{
    public class AIData : MonoBehaviour
    {
        public CharacterFSM m_FSM;

        [Header("Range Settings")]
        public bool EnemyInSight;

        public bool DrawCautionRange;
        public float FaceCautionRange = 15f;
        public float BackCaurionRange = 7f;
        public float FOV = 60f;

        public bool DrawChaseRange;
        public float ChaseRange = 10f;

        public bool DrawJumpAtkRange;
        public float JumpAtkRange = 4f;

        public bool DrawAtkRange;
        public float AtkRange = 1.5f;

        public bool PlayerInSight;

        [Header("Patrol Settings")]
        public List<Waypoint> Path;
        public bool Patrolling;
        public float RestTime;
        public Waypoint PreviousWP;
        public Waypoint NextWP;

        [Header("Battle Settings")]
        public float AtkOffset = 0.5f;
        public bool AtkReady = true;
        public bool JumpAtkReady = true;

        public float AttackFrequency = 3f;
        public float JumpAttackFrequency = 7f;

        void Start()
        {
            m_FSM = GetComponent<CharacterFSM>();
        }



        public void Attack()
        {
            StartCoroutine(AtkCD(AttackFrequency));
        }

        IEnumerator AtkCD(float fCD)
        {
            AtkReady = false;
            yield return new WaitForSeconds(fCD);
            AtkReady = true;
        }

        public void JumpAttack()
        {
            StartCoroutine(JumpAtkCD(JumpAttackFrequency));
        }

        IEnumerator JumpAtkCD(float fCD)
        {
            JumpAtkReady = false;
            yield return new WaitForSeconds(fCD);
            JumpAtkReady = true;

        }

        void OnDrawGizmosSelected()
        {
            if(DrawCautionRange)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, FaceCautionRange);
            }
            if(DrawChaseRange)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, ChaseRange);
            }
            if(DrawJumpAtkRange)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, JumpAtkRange);
            }
            if(DrawAtkRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, AtkRange);
            }
        }
    }
}


