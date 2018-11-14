using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem;
using BattleSystem;
using FSM.StatesLibrary;


namespace FSM
{
    #region Base FSM System

    /// <summary>
    /// 狀態機基底類別
    /// </summary>
    public abstract class FSMSystem : MonoBehaviour
    {
        /// <summary>
        /// 這個狀態機是否正在進行狀態轉換
        /// </summary>
        protected internal bool bTranfering;

        /// <summary>
        /// 這個狀態機的初始狀態
        /// </summary>
        /// <value>初始狀態</value>
        protected internal Enum InitialStateID;

        /// <summary>
        /// 這個狀態機現在的狀態
        /// </summary>
        protected internal FSMState CurrentState { get { return _currentState; } set { _currentState = value; stateTime = 0f; } }
        FSMState _currentState;

        /// <summary>
        /// 進入Transition前的狀態
        /// </summary>
        /// <value>原本的狀態</value>
        protected internal FSMState OriginState { get; set; }

        /// <summary>
        /// 這個狀態機可切換的一般狀態
        /// </summary>
        protected internal Dictionary<Enum, FSMState> validStates;

        /// <summary>
        /// 這個狀態機可切換的全域狀態，你應該把所有的AnyState可進入的狀態加入這個Dictionary
        /// </summary>
        protected Dictionary<Enum, FSMState> globalTransitions;

        /// <summary>
        /// 當前狀態執行RunState的次數
        /// </summary>
        /// <value>RunState的次數</value>
        protected internal float stateTime;

        /// <summary>
        /// 當前正在進行的Transition，用來做StopCoroutine
        /// </summary>
        protected internal Coroutine m_currentTransition;

        /// <summary>
        /// 現在的StateID，會秀在Inspecter裡面
        /// </summary>
        [SerializeField] protected internal string CurrentStateID;

        protected internal Func<FSMState,IEnumerator> TransitionHandler;

        protected virtual void Awake()
        {
            InitFSM();
        }

        protected void OnDestroy()
        {
            UnInitFSM();
        }

        protected virtual void Update()
        {
            CurrentStateID = CurrentState.StateID.ToString();
            RunState();
        }

        /// <summary>
        /// 初始化狀態機
        /// </summary>
        protected virtual void InitFSM()
        {
            validStates = new Dictionary<Enum, FSMState>();
            globalTransitions = new Dictionary<Enum, FSMState>();
            SetTransitionHandler();
            CurrentState = InitValidStates();
            CurrentState.OnStateEnter();
        }

        protected virtual void SetTransitionHandler()
        {
            TransitionHandler = TransferTo;
        }

        /// <summary>
        /// 指定這個狀態機能執行的狀態，並設定初始狀態
        /// </summary>
        /// <returns>The valid states.</returns>
        protected abstract FSMState InitValidStates();

        /// <summary>
        /// 反初始化狀態機釋放資源
        /// </summary>
        protected virtual void UnInitFSM()
        {
            validStates.Clear();
            globalTransitions.Clear();
        }

        /// <summary>
        /// 將此狀態機能轉換的狀態加入validStates
        /// </summary>
        /// <param name="state">要加入的狀態</param>
        protected void AddState(FSMState state)
        {
            if (validStates != null && validStates.ContainsKey(state.StateID) == false)
            {
                validStates.Add(state.StateID, state);
            }
        }

        /// <summary>
        /// 傳入狀態ID查詢狀態並將其刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="stateID">要刪除的狀態ID</param>
        protected void DeleteState(Enum stateID)
        {
            if (validStates == null) return;
            var stateToRemove = from s in validStates.Keys where s == stateID select s;
            validStates.Remove((Enum)stateToRemove);
        }

        /// <summary>
        /// 執行狀態當下要做的事
        /// </summary>
        protected void RunState()
        {
            if (CurrentState != null)
            {
                CheckGlobalConditions();
                if (bTranfering == false)
                {
                    stateTime += 1;
                    CurrentState.OnStateRunning();
                }
            }
        }

        /// <summary>
        /// 轉換狀態的coroutine
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        IEnumerator TransferTo(FSMState targetState)
        {
            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            yield return null;
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        /// <summary>
        /// 執行狀態切換
        /// </summary>
        /// <param name="stateID">目標狀態的StateID</param>
        internal void PerformTransition(Enum stateID)
        {
            if (validStates != null && validStates.ContainsKey(stateID))
            {
                m_currentTransition = StartCoroutine(TransitionHandler(validStates[stateID]));
            }
            else
            {
                Debug.LogError(gameObject.name + "狀態切換失敗！原因：要切換的狀態：" + stateID + " 不在此狀態機可切換的狀態清單內(是否忘記增刪過或遺漏條件判定？)");
            }
        }

        /// <summary>
        /// 新增全域狀態
        /// </summary>
        /// <param name="anyState">要新增的狀態</param>
        protected void AddGlobalTransition(FSMState anyState)
        {
            if (globalTransitions != null && globalTransitions.ContainsKey(anyState.StateID) == false)
            {
                globalTransitions.Add(anyState.StateID, anyState);
            }
        }

        /// <summary>
        /// 檢查全域狀態的條件
        /// </summary>
        protected abstract void CheckGlobalConditions();

        /// <summary>
        /// 開始切換到全域狀態
        /// </summary>
        /// <param name="anyStateID">全域狀態的ID</param>
        protected internal void PerformGlobalTransition(Enum anyStateID)
        {
            if (globalTransitions != null && globalTransitions.ContainsKey(anyStateID))
            {
                StopCoroutine(m_currentTransition);
                m_currentTransition = StartCoroutine(TransitionHandler(globalTransitions[anyStateID]));
            }
            else
            {
                Debug.LogError(gameObject.name + "全域狀態切換失敗！原因：要切換的AnyState：" + anyStateID + " 不在此狀態機可切換的全域狀態內");
            }
        }
    }

    #endregion

    #region Animator FSM

    public abstract class CharacterFSM : FSMSystem
    {
        /// <summary>
        /// 角色狀態機需要的資料
        /// </summary>
        protected internal AIData m_AIData;
        protected internal BattleData m_BattleData;
        protected internal Animator m_Animator;

        protected internal Func<FSMState, int, IEnumerator> SubMachineTransitionHandler;

        public bool CanBeHit;
        public bool CanBeKOed;
        public bool CanBeDead;

        protected bool m_isDead, m_isFreezed, m_isKOed;

        protected virtual void OnAnimatorMove()
        {
            m_Animator.ApplyBuiltinRootMotion();
            if (m_Animator==null || CurrentState == null) return;
            CharacterFSMState currentState = (CharacterFSMState)CurrentState;
            currentState.OnAnimatorMove();
        }

        protected override void InitFSM()
        {
            m_AIData = GetComponent<AIData>();
            m_BattleData = GetComponent<BattleData>();
            m_Animator = GetComponent<Animator>();

            if (CanBeDead)
            {
                m_BattleData.Died += OnCharacterDied;
                if (CanBeHit)
                {
                    m_BattleData.Hit += OnCharacterHit;
                    m_BattleData.Freezed += OnCharacterFreezed;
                    if (CanBeKOed)
                        m_BattleData.KOed += OnCharacterKOed;
                }
            }
            base.InitFSM();
        }

        protected override void UnInitFSM()
        {
            m_AIData = null;
            m_Animator = null;
            if (CanBeDead)
            {
                m_BattleData.Died -= OnCharacterDied;
                if (CanBeHit)
                {
                    m_BattleData.Hit -= OnCharacterHit;
                    m_BattleData.Freezed -= OnCharacterFreezed;
                    if (CanBeKOed)
                        m_BattleData.KOed -= OnCharacterKOed;
                }
            }
            m_BattleData = null;
            base.UnInitFSM();
        }

        protected override void SetTransitionHandler()
        {
            TransitionHandler = (arg1) => TransferTo((CharacterFSMState)arg1);
            SubMachineTransitionHandler = (arg1, arg2) => TransferTo((FSMSubMachine)arg1, arg2);
        }

        /// <summary>
        /// 轉換狀態的coroutine
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        IEnumerator TransferTo(CharacterFSMState targetState)
        {
            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            if (targetState.IsSubMachine == false)
            {
                if (String.IsNullOrEmpty(targetState.Trigger) == false)
                {
                    m_Animator.SetTrigger(targetState.Trigger);
                    yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == true);
                }
            }
            else if (targetState.GetType() == typeof(FSMSubMachine))
            {
                FSMSubMachine nextState = (FSMSubMachine)targetState;
                if (String.IsNullOrEmpty(nextState.SubStatesTriggers[0]) == false)
                {
                    m_Animator.SetTrigger(nextState.SubStatesTriggers[0]);
                    yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == true);
                }
            }
            else
            {
                Debug.LogWarning("輸入了一個不正確的目標狀態");
            }
            yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == false);
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        /// <summary>
        /// 轉換到子狀態機的Coroutine
        /// </summary>
        /// <param name="targetState">目標子狀態機</param>
        /// <param name="subStateID">目標階段</param>
        IEnumerator TransferTo(FSMSubMachine targetState, int subStateID)
        {
            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            if (String.IsNullOrEmpty(targetState.SubStatesTriggers[subStateID]) == false)
            {
                m_Animator.SetTrigger(targetState.SubStatesTriggers[subStateID]);
                yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == true);
            }
            else
            {
                Debug.LogWarning("輸入了一個不正確的目標狀態");
            }
            yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == false);
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }


        /// <summary>
        /// 開始切換到子狀態機
        /// </summary>
        /// <param name="stateID">該子狀態機的ID</param>
        /// <param name="subStateID">目標子狀態的階段</param>
        protected internal void PerformTransition(Enum stateID, int subStateID)
        {
            if (validStates != null && validStates.ContainsKey(stateID))
            {
                if (validStates[stateID].GetType() == typeof(FSMSubMachine))
                {
                    FSMSubMachine nextState = (FSMSubMachine)validStates[stateID];
                    m_currentTransition = StartCoroutine(SubMachineTransitionHandler(nextState, subStateID));
                }
                else
                {
                    FSMState nextState = validStates[stateID];
                    m_currentTransition = StartCoroutine(TransitionHandler(nextState));
                }
            }
            else
            {
                Debug.LogError(gameObject.name + "狀態切換失敗！原因：要切換的狀態：" + stateID + " 不在此狀態機可切換的狀態清單內(是否忘記增刪過或遺漏條件判定？)");
            }
        }

        protected abstract void OnCharacterHit();

        protected abstract void OnCharacterFreezed(DefendBox damagedPart);

        protected abstract void OnCharacterKOed();

        protected abstract void OnCharacterDied();

    }

    #endregion

    #region NPC FSM

    public abstract class NpcFSM : CharacterFSM
    {
        [SerializeField] protected internal new Npc InitialStateID = Npc.Idle;

        public enum Style { Normal = 0, Sinister = 1, Cautious = 2 }
        public enum StartPose { Stand = 0, Crouch = 1 }

        /// <summary>
        /// 這個Npc的行為風格
        /// </summary>
        public Style Agent { get { return _style; } set { _style = value; } }
        [SerializeField] Style _style;

        /// <summary>
        /// 這個Npc的種類
        /// </summary>
        /// <value>物種名稱</value>
        public Species Species { get { return _species; } set { _species = value; } }
        [SerializeField] Species _species;

        /// <summary>
        /// 這個Npc的初始狀態姿勢，會決定Animator初始化後前往的第一個狀態
        /// </summary>
        /// <value>初始姿勢</value>
        public StartPose Pose { get { return _pose; } set { _pose = value; } }
        [SerializeField] StartPose _pose;

        protected override void SetTransitionHandler()
        {
            TransitionHandler = (arg1) => TransferTo((NpcFSMState)arg1);
            SubMachineTransitionHandler = (arg1, arg2) => TransferTo((NpcSubMachine)arg1, arg2);
        }

        IEnumerator TransferTo(NpcFSMState targetState)
        {

            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            if (targetState.GetType() != typeof(NpcSubMachine))
            {
                if (String.IsNullOrEmpty(targetState.Trigger) == false)
                {
                    m_Animator.SetTrigger(targetState.Trigger);
                    yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == true);
                }
            }
            else if (targetState.GetType() == typeof(NpcSubMachine))
            {
                NpcSubMachine nextState = (NpcSubMachine)targetState;
                if (String.IsNullOrEmpty(nextState.SubStatesTriggers[0]) == false)
                {
                    m_Animator.SetTrigger(nextState.SubStatesTriggers[0]);
                    yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == true);
                }
            }
            else
            {
                Debug.LogWarning("輸入了一個不正確的目標狀態");
            }
            yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == false);
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        IEnumerator TransferTo(NpcSubMachine targetState,int subStateID)
        {

            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            if (String.IsNullOrEmpty(targetState.SubStatesTriggers[subStateID]) == false)
            {
                m_Animator.SetTrigger(targetState.SubStatesTriggers[subStateID]);
                yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == true);
            }
            else
            {
                Debug.LogWarning("輸入了一個不正確的目標狀態");
            }
            yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == false);
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        protected override FSMState InitValidStates()
        {
            foreach (var s in StatesLib.BasicNpc.States)
            {
                if ((Npc)s == Npc.Idle) AddState(new BasicNpc.Idle(this, Pose));
                if ((Npc)s == Npc.Patrol) AddState(new BasicNpc.Patrol(this));
                if ((Npc)s == Npc.Chase) AddState(new BasicNpc.Chase(this));
                if ((Npc)s == Npc.Confront) AddState(new BasicNpc.Confront(this));
                if ((Npc)s == Npc.Attack) AddState(new BasicNpc.Attack(this));
                if ((Npc)s == Npc.Approach) AddState(new BasicNpc.Approach(this));
            }
            foreach (var a in StatesLib.BasicNpc.AnyStates)
            {
                if ((Npc)a == Npc.Died) AddGlobalTransition(new BasicNpc.Died(this));
                if ((Npc)a == Npc.Freezed) AddGlobalTransition(new BasicNpc.Freezed(this));
            }
            return validStates[InitialStateID];
        }
    }

    #endregion

}
