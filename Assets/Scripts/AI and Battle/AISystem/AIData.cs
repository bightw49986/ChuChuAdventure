using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using PathFinding;

namespace AISystem
{
    [RequireComponent(typeof(NpcFSM))]
    [RequireComponent(typeof(AStarAgent))]
    public partial class AIData : MonoBehaviour
    {
        /// <summary>
        /// 角色的有限狀態機
        /// </summary>
        public NpcFSM m_FSM;

        [Header("Range Settings")]
        /// <summary>
        /// 是否畫出最大視野範圍
        /// </summary>
        public bool DrawCautionRange;
        /// <summary>
        /// 正面視野範圍，在角色前方FOV扇形範圍內出現的話，會被角色察覺
        /// </summary>
        public float fFaceCautionRange = 15f;
        /// <summary>
        /// 正面視野範圍的平方
        /// </summary>
        [HideInInspector] public float fSqrFaceCautionRange;
        /// <summary>
        /// 視野範圍，進入這個距離，則一定會被察覺
        /// </summary>
        public float fBackCautionRange = 7f;
        /// <summary>
        /// 視野範圍的平方
        /// </summary>
        [HideInInspector] public float fSqrBackCautionRange;
        /// <summary>
        /// 角色的視線開角
        /// </summary>
        public float FOV = 90f;
        /// <summary>
        /// 是否畫出追逐範圍
        /// </summary>
        public bool DrawChaseRange;
        /// <summary>
        /// 追逐範圍
        /// </summary>
        public float fChaseRange = 10f;
        /// <summary>
        /// 追逐範圍的平方
        /// </summary>
        [HideInInspector] public float fSqrChaseRange;
        /// <summary>
        /// 是否畫出撲擊範圍
        /// </summary>
        public bool DrawJumpAtkRange;
        /// <summary>
        /// 撲擊範圍
        /// </summary>
        public float fJumpAtkRange = 4f;
        /// <summary>
        /// 撲擊範圍的平方
        /// </summary>
        [HideInInspector] public float fSqrJumpAtkRange;
        /// <summary>
        /// 是否畫出攻擊範圍
        /// </summary>
        public bool DrawAtkRange;
        /// <summary>
        /// 攻擊範圍
        /// </summary>
        public float fAtkRange = 1.5f;
        /// <summary>
        /// 攻擊範圍的平方
        /// </summary>
        [HideInInspector] public float fSqrAtkRange;


        [Header("Moving Info")]

        public DestinationState Destination;
        [SerializeField] bool DrawDestination;
        [SerializeField] bool DrawPath;


        public enum DestinationState { None = 0, Player = 1, Patrol = 2, BackToIdle = 3 }
        /// <summary>
        /// 角色的高度
        /// </summary>
        [SerializeField] float fHeight;
        /// <summary>
        /// 角色的探針長度
        /// </summary>
        [SerializeField] float fPorbe = 3f;
        /// <summary>
        /// 角色的寬度
        /// </summary>
        [SerializeField] float fWidth;
        Vector3 vCenter;
        Vector3 vCPoint1 =Vector3.zero;
        Vector3 vCPoint2 = Vector3.zero;
        RaycastHit nearestHit;

        /// <summary>
        /// 要當作障礙物的Layer
        /// </summary>
        [SerializeField] LayerMask CollisionLayer;

        RaycastHit m_LastHit;

        /// <summary>
        /// 移動速度倍率
        /// </summary>
        [Range(0,2)] public float fMoveSpeed = 1f;

        /// <summary>
        /// 最大速度
        /// </summary>
        [SerializeField] float fMaxSpeed = 10f;

        /// <summary>
        /// 最大速度的平方
        /// </summary>
        float fSqrMaxSpped;

        /// <summary>
        /// 最大角速度
        /// </summary>
        [SerializeField] float fMaxSteerSpeed = 3f;

        /// <summary>
        /// 最大角速度的平方
        /// </summary>
        float fSqrMaxSteerSpeed;

        /// <summary>
        /// 旋轉的權重
        /// </summary>
        float fTurnWeight;

        /// <summary>
        /// 旋轉的權重平方，會是一個較平滑的曲線
        /// </summary>
        float fSqrTurnWeight;

        /// <summary>
        /// 做直接旋轉時的最大角度
        /// </summary>
        [SerializeField] float fMaxTurnDegree = 30f;

        /// <summary>
        /// 上一幀轉向力投影在速度方向的內積
        /// </summary>
        float fSteerLastFrame;

        [Header("Player Info")]
        /// <summary>
        /// 玩家
        /// </summary>
        public Player player;
        /// <summary>
        /// 玩家死了嗎
        /// </summary>
        public bool PlayerIsDead;
        /// <summary>
        /// 是否與玩家進入戰鬥
        /// </summary>
        public bool IsInBattle;
        /// <summary>
        /// 玩家的位置
        /// </summary>
        public Vector3 m_vPlayerPos;
        /// <summary>
        /// 與玩家的距離
        /// </summary>
        public float fPlayerDis;
        /// <summary>
        /// 與玩家的距離平方
        /// </summary>
        public float fSqrPlayerDis;
        /// <summary>
        /// 到玩家的方向
        /// </summary>
        public Vector3 vDirectionToPlayer;

        [Header("Patrol Setting")]
        /// <summary>
        /// 這個角色在非戰鬥時是否巡邏
        /// </summary>
        public bool Patrolling;
        /// <summary>
        /// 巡邏路線(照順序手動加)
        /// </summary>
        public List<Waypoint> Path;
        /// <summary>
        /// 巡邏點間的休息時間
        /// </summary>
        public float fRestTime;
        /// <summary>
        /// 上一個巡邏點
        /// </summary>
        public AStarNode previousNode;
        /// <summary>
        /// 下一個巡邏點
        /// </summary>
        public AStarNode nextNode;
        /// <summary>
        /// 進入戰鬥前的最後位置
        /// </summary>
        public AStarNode lastNodeBeforeBattle;

        [Header("A* Info")]
        /// <summary>
        /// A star 代理人
        /// </summary>
        AStarAgent aStarAgent;
        /// <summary>
        /// 目標位置
        /// </summary>
        Vector3 m_vDestination =Vector3.zero;
        /// <summary>
        /// 上一幀的目標位置
        /// </summary>
        Vector3 m_vDestLastFrame = Vector3.zero;
        /// <summary>
        /// 到目標的方向
        /// </summary>
        Vector3 vDirectionToDest = Vector3.zero;
        /// <summary>
        /// 到目標的距離
        /// </summary>
        float m_fDistToDest;
        /// <summary>
        /// 到目標的距離平方
        /// </summary>
        float m_fSqrDistToDest;
        /// <summary>
        /// 目標與面朝方向的內積
        /// </summary>
        float m_fDestDot;
        /// <summary>
        /// 目標與面朝方向的內積絕對值，用來算權重
        /// </summary>
        float m_fAbsDestDot;

        [Header("Battle Info")]
        /// <summary>
        /// 下一個行動的代號
        /// </summary>
        public int NextMoveID;
        /// <summary>
        /// 攻擊是否CD到
        /// </summary>
        public bool AtkReady = true;
        /// <summary>
        /// 撲擊是否CD到
        /// </summary>
        public bool JumpAtkReady = true;
        /// <summary>
        /// 攻擊範圍的寬容值(加越大會越笨)
        /// </summary>
        public float fAtkOffset = 0.5f;
        /// <summary>
        /// 攻擊的CD
        /// </summary>
        public float fAttackFrequency = 3f;
        /// <summary>
        /// 撲擊的CD
        /// </summary>
        public float fJumpAttackFrequency = 20f;

        void Awake()
        {
            InitAIData(); //初始化AIData
        }

        void Start()
        {
            Destination = DestinationState.None;
        }

        void Update()
        {
            UpdatePlayerInfo(); //更新玩家的資料
            UpdateProbe(); //更新自己的探針資訊
        }

        void OnAnimatorMove()
        {
            MoveToDestination();
        }

        void LateUpdate()
        {
            m_vDestLastFrame = m_vDestination; //把這一幀的目標位置紀錄起來
        }

        void OnDrawGizmosSelected()
        {
            if (DrawDestination)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(vCenter, 0.2f);
                Gizmos.DrawLine(aStarAgent.Position, m_vDestination);

                if (DrawPath &&  path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        if (i < path.Count -2)
                        {
                            Gizmos.DrawLine(path[i], path[i + 1]);
                        }
                    }
                }


                Gizmos.color = Color.black;
                Gizmos.DrawSphere(m_vDestination, 0.3f);
                Gizmos.DrawLine(vCenter, m_vDestination);
            }

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
    }
}


