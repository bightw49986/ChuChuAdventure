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

        /// <summary>
        /// 角色的中心點位置(高度的一半)
        /// </summary>
        Vector3 m_vCenter;

        /// <summary>
        /// 探針的遠點位置
        /// </summary>
        Vector3 m_vProbeEnd;

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
        /// 與玩家之間是否有被障礙物遮蔽
        /// </summary>
        public bool PlayerInSight;
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
        public Waypoint previousWP;
        /// <summary>
        /// 下一個巡邏點
        /// </summary>
        public Waypoint nextWP;
        /// <summary>
        /// 進入戰鬥前的最後位置
        /// </summary>
        public Vector3 vLastPosBeforeBattle;

        [Header("A* Info")]
        /// <summary>
        /// A star 代理人
        /// </summary>
        AStarAgent aStarAgent;
        /// <summary>
        /// 目標位置
        /// </summary>
        Vector3 m_vDestination;
        /// <summary>
        /// 上一幀的目標位置
        /// </summary>
        Vector3 m_vDestLastFrame;
        /// <summary>
        /// 到目標的方向
        /// </summary>
        Vector3 vDirectionToDest;
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

        }

        /// <summary>
        /// 初始化AI Data
        /// </summary>
        void InitAIData()
        {
            m_FSM = GetComponent<NpcFSM>(); if (!m_FSM) Debug.LogError("沒有FSM");
            aStarAgent = GetComponent<AStarAgent>(); if (!aStarAgent) Debug.LogError("沒有a star agent");
            InitAIStats();
            InitPlayerData();
            OnAIDataInitialized();
        }

        /// <summary>
        /// 初始化玩家的資料
        /// </summary>
        void InitPlayerData()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            player.Died += () => { PlayerIsDead = true; };
            UpdatePlayerInfo();
            UpdateProbe();
        }

        /// <summary>
        /// 當AI Data初始完觸發的事件
        /// </summary>
        public event Action AIDataInitialized;

        /// <summary>
        /// 當AI Data初始完觸發事件
        /// </summary>
        protected virtual void OnAIDataInitialized()
        {
            if (AIDataInitialized != null) AIDataInitialized();
        }

        void Update()
        {
            UpdatePlayerInfo(); //更新玩家的資料
            UpdateProbe(); //更新自己的探針資訊
        }

        void LateUpdate()
        {
            m_vDestLastFrame = m_vDestination; //把這一幀的目標位置紀錄起來
        }

        /// <summary>
        /// 更新玩家資訊
        /// </summary>
        void UpdatePlayerInfo()
        {
            m_vPlayerPos = player.Position; //找玩家位置
            vDirectionToPlayer = m_vPlayerPos - aStarAgent.Position; //算玩家的方向
            fSqrPlayerDis = Vector3.SqrMagnitude(vDirectionToPlayer); //算玩家的距離平方
            fPlayerDis = Mathf.Sqrt(fSqrPlayerDis); //算玩家的距離
        }

        /// <summary>
        /// 更新目的地資訊
        /// </summary>
        void UpdateDestinationInfo()
        {
            vDirectionToDest = m_vDestination - aStarAgent.Position; //算目的地方向
            m_fSqrDistToDest = Vector3.SqrMagnitude(vDirectionToDest); //算目的距離平方
            m_fDistToDest = Mathf.Sqrt(m_fSqrDistToDest); //算目的地的距離
            m_fDestDot = Mathf.Clamp01(Vector3.Dot(transform.forward, vDirectionToDest.normalized)); //算目的地方向相對於角色的偏離方向
            m_fAbsDestDot = Mathf.Abs(m_fDestDot); //算偏離程度
            fTurnWeight = 1 - m_fAbsDestDot; //根據偏離程度決定轉向動畫權重
            fSqrTurnWeight = Mathf.Clamp01(fTurnWeight * fTurnWeight); //把權重平滑化
            m_FSM.m_Animator.SetFloat("fTurn", fSqrTurnWeight); //Set給animator
        }

        /// <summary>
        /// 更新探針資訊
        /// </summary>
        void UpdateProbe()
        {
            m_vCenter = new Vector3(0, fHeight * 0.5f, 0); //算角色的中心點位置
            m_vCenter += transform.position; 
            m_vProbeEnd = new Vector3(0, fHeight * 0.5f, fPorbe); //算探針的最遠位置
            m_vProbeEnd += transform.position;
        }

        /// <summary>
        /// 初始化數值
        /// </summary>
        void InitAIStats()
        {
            InitRangeStats();
            InitMoveStats();
        }

        /// <summary>
        /// 初始化範圍數值，把該乘的乘一乘
        /// </summary>
        void InitRangeStats()
        {
            m_vDestination = vDirectionToDest = m_vDestLastFrame = Vector3.zero;
            fSqrFaceCautionRange = fFaceCautionRange * fFaceCautionRange;
            fSqrBackCautionRange = fBackCautionRange * fBackCautionRange;
            fSqrChaseRange = fChaseRange * fChaseRange;
            fSqrJumpAtkRange = fJumpAtkRange * fJumpAtkRange;
            fSqrAtkRange = fAtkRange * fAtkRange;
        }

        /// <summary>
        /// 初始化移動數據，把該乘的乘一乘
        /// </summary>
        void InitMoveStats()
        {
            UpdateProbe();
            fMaxSpeed *= Time.deltaTime;
            fSqrMaxSpped = fMaxSpeed * fMaxSpeed;
            fMaxSteerSpeed *= Time.deltaTime;
            fSqrMaxSteerSpeed = fMaxSteerSpeed * fMaxSteerSpeed;
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

        /// <summary>
        /// 朝玩家移動
        /// </summary>
        public void MoveToPlayer()
        {
            SteerToTarget();
            UpdateDestinationInfo();
        }



        /// <summary>
        /// 轉向至目標
        /// </summary>
        void SteerToTarget()
        {

            if (m_fDestDot > fWidth)
            {

            }



            Quaternion qDesireRotation = Quaternion.identity;
            if (transform.forward != vDirectionToDest)
            {
                qDesireRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vDirectionToDest), fMaxTurnDegree * Time.deltaTime);
            }
            transform.position = m_FSM.m_Animator.rootPosition;
            transform.rotation = qDesireRotation;
        }

        /// <summary>
        /// 迴避障礙物(不優)
        /// </summary>
        /// <returns>The obstacles.</returns>
        /// <param name="vDest">V destination.</param>
        /// <param name="velocity">Velocity.</param>
        Vector3 AvoidObstacles(Vector3 vDest, Vector3 velocity)
        {
            RaycastHit[] hits = Physics.CapsuleCastAll(m_vCenter, m_vProbeEnd, fWidth, vDest,CollisionLayer);
            if (hits != null)
            {
                float fMinSqrDist=10000f;
                float fSqrDist;
                float fDot;
                Vector3 direction = Vector3.zero;
                Vector3 probeToObstacle = Vector3.zero;
                RaycastHit nearestHit = new RaycastHit();
                for (int i = 0; i < hits.Length; i++)
                {
                    direction = hits[i].point - transform.position;
                    direction.y = 0;
                    fSqrDist = Vector3.SqrMagnitude(direction);
                    fDot = Mathf.Clamp(Vector3.Dot(transform.forward, direction.normalized), -1, 1);
                    if (fSqrDist < fMinSqrDist)
                    {
                       fMinSqrDist= fSqrDist;
                        nearestHit = hits[i];
                    }
                }
                if (fMinSqrDist != 10000f)
                {
                    m_LastHit = nearestHit;
                    return AIMethod.Flee(transform.position, m_LastHit.point, velocity, fMaxSteerSpeed);
                }
            }
            return Vector3.zero;
        }

        public void StopTurnning()
        {
            m_FSM.m_Animator.SetLayerWeight(1, 0);
        }

        public void StartTurnning()
        {
            m_FSM.m_Animator.SetLayerWeight(1, 1);
        }

        public void LookAtTargetDirectly()
        {
            float fAngle = AIMethod.FindAngle(transform.position, m_vDestination, transform.up);
            fAngle = Mathf.Clamp(fAngle, -fMaxTurnDegree, fMaxTurnDegree);
            Quaternion qDesireRotation = Quaternion.AngleAxis(fAngle, transform.up) * transform.rotation;
            transform.rotation = qDesireRotation;
        }


        public void EnterBattle()
        {
            IsInBattle = true;
            vLastPosBeforeBattle = transform.position;
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
            StartCoroutine(AtkCD(fAttackFrequency));
        }

        IEnumerator JumpAtkCD(float fCD)
        {
            JumpAtkReady = false;
            yield return new WaitForSeconds(fCD);
            JumpAtkReady = true;

        }
    }
}


