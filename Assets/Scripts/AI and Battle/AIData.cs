using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AISystem
{
    public class AIData : MonoBehaviour
    {
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
        public float AttackFrequency;


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


