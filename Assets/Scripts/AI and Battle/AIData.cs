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
            FetchPlayerData();
            OnAIDataInitialized();
        }

        /// <summary>
        /// 初始化玩家的資料
        /// </summary>
        void FetchPlayerData()
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
        /// 更新膠囊探針資訊
        /// </summary>
        void UpdateProbe()
        {
            vCenter = transform.position + Vector3.up * fHeight * 0.5f;
            vCPoint1 = vCenter + Vector3.up * 0.4f;
            vCPoint2 = vCenter + Vector3.down * 0.4f;
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
            if (CheckIfBlocked(player.transform.position)==false)//檢查與玩家之間有無障礙物
            {
                m_vDestination = player.transform.position;
            }
            else
            {
                Vector3 vTemp = Vector3.zero;
                List<Vector3> path = aStarAgent.GetPath(aStarAgent, player);
                print(path.Count);
                for (int i =path.Count -1 ; i > 0; i--)
                {
                    if(CheckIfBlocked(path[i])==true)
                    {
                        vTemp = path[i];
                        break;
                    }
                }
                if (vTemp != Vector3.zero)
                {
                    m_vDestination = vTemp;
                }
                else
                {
                    m_vDestination = path[0];
                }
            }
            UpdateDestinationInfo();
            SteerToDestination();
            //無的話，把Destination設成玩家的位置
            //有的話，用Astar算出Destination
            //檢查新的Destination路上有無障礙物
            //有的話，算出最短沒有障礙物的點，把Destination設成那個點
            //轉向Desination
        }


        bool CheckIfBlocked(Vector3 vTargetPos)
        {
            vTargetPos.y = 0f;
            Vector3 vStart = transform.position;
            vStart.y = 0f;
            Vector3 vDir = vTargetPos - vStart;
            float fDis = Vector3.Magnitude(vDir);
            RaycastHit[] hits;
            hits = Physics.CapsuleCastAll(vCPoint1, vCPoint2, fWidth, vDir, fDis);
            if (hits == null) return false;
            float fMin =100f;
            foreach (var hit in hits)
            {
                if(hit.distance < fMin)
                {
                    fMin = hit.distance;
                    nearestHit = hit;
                }
            }
            return true;
        }


        /// <summary>
        /// 轉向至目標
        /// </summary>
        void SteerToDestination()
        {
            Quaternion qDesireRotation = Quaternion.identity;
            if (transform.forward != vDirectionToDest)
            {
                qDesireRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vDirectionToDest), fMaxTurnDegree * Time.deltaTime);
            }
            transform.position = m_FSM.m_Animator.rootPosition;
            transform.rotation = qDesireRotation;
        }

        /// <summary>
        /// 不許播腳步旋轉動畫
        /// </summary>
        public void StopTurnning()
        {
            m_FSM.m_Animator.SetLayerWeight(1, 0);
        }

        /// <summary>
        /// 允許播腳部旋轉動畫
        /// </summary>
        public void StartTurnning()
        {
            m_FSM.m_Animator.SetLayerWeight(1, 1);
        }

        /// <summary>
        /// 瞬間轉向player的位置，角度受到m_fMaxTurnDegree限制
        /// </summary>
        public void LookAtPlayerDirectly()
        {
            float fAngle = AIMethod.FindAngle(aStarAgent.Position, m_vPlayerPos, transform.up);
            fAngle = Mathf.Clamp(fAngle, -fMaxTurnDegree, fMaxTurnDegree);
            Quaternion qDesireRotation = Quaternion.AngleAxis(fAngle, transform.up) * transform.rotation;
            transform.rotation = qDesireRotation;
        }

        /// <summary>
        /// 進入戰鬥，紀錄進入戰鬥前最後的位置
        /// </summary>
        public void EnterBattle()
        {
            IsInBattle = true;
            vLastPosBeforeBattle = aStarAgent.Position;
        }

        /// <summary>
        /// 攻擊，攻擊進入冷卻時間
        /// </summary>
        public void Attack()
        {
            StartCoroutine(AtkCD(fAttackFrequency));
        }

        /// <summary>
        /// 計算攻擊冷卻時間
        /// </summary>
        /// <returns>The cd.</returns>
        /// <param name="fCD">F cd.</param>
        IEnumerator AtkCD(float fCD)
        {
            AtkReady = false;
            yield return new WaitForSeconds(fCD);
            AtkReady = true;
        }

        /// <summary>
        /// 跳躍攻擊，攻擊跟跳躍攻擊進冷卻時間
        /// </summary>
        public void JumpAttack()
        {
            StartCoroutine(JumpAtkCD(fJumpAttackFrequency));
            StartCoroutine(AtkCD(fAttackFrequency));
        }

        /// <summary>
        /// 計算跳躍攻擊冷卻時間
        /// </summary>
        /// <returns>The atk cd.</returns>
        /// <param name="fCD">F cd.</param>
        IEnumerator JumpAtkCD(float fCD)
        {
            JumpAtkReady = false;
            yield return new WaitForSeconds(fCD);
            JumpAtkReady = true;

        }
    }
}


