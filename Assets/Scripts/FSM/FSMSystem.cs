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
        protected internal Enum InitialState { get { return _initialState; } set { _initialState = value; } }
        [SerializeField] Enum _initialState;

        /// <summary>
        /// 這個狀態機現在的狀態
        /// </summary>
        protected internal FSMState CurrentState { get { return _currentState; } set { _currentState = value; stateTime = 0f; } }
        private FSMState _currentState;

        /// <summary>
        /// 進入Transition前的狀態
        /// </summary>
        /// <value>原本的狀態</value>
        protected internal FSMState OriginState { get; set; }

        /// <summary>
        /// 這個狀態機可切換的一般狀態
        /// </summary>
        protected internal Dictionary<Enum,FSMState> validStates;

        /// <summary>
        /// 這個狀態機可切換的全域狀態，你應該把所有的AnyState可進入的狀態加入這個Dictionary
        /// </summary>
        protected Dictionary<Enum,FSMState > globalTransitions;

        /// <summary>
        /// 當前狀態執行RunState的次數
        /// </summary>
        /// <value>RunState的次數</value>
        protected internal float stateTime;

        protected void Awake()
        {
            InitFSM();
        }

        protected void OnDestroy()
        {
            UnInitFSM();
        }

        /// <summary>
        /// 初始化狀態機
        /// </summary>
        protected virtual void InitFSM()
        {
            validStates = new Dictionary<Enum, FSMState>();
            globalTransitions = new Dictionary<Enum, FSMState>();
            CurrentState = InitValidStates(InitialState);
        }

        /// <summary>
        /// 指定這個狀態機能執行的狀態，
        /// </summary>
        /// <returns>The valid states.</returns>
        protected abstract FSMState InitValidStates(Enum initialState);

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
            if (validStates!= null && validStates.ContainsKey(state.StateID) == false)
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
            if (CurrentState!=null && bTranfering == false)
            {
                stateTime += 1;
                CheckGlobalConditions();
                CurrentState.OnStateRunning();
            }
        }

        /// <summary>
        /// 轉換狀態的coroutine
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        protected virtual IEnumerator TransferTo(FSMState targetState)
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
                StartCoroutine(TransferTo(validStates[stateID]));
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
                StartCoroutine(TransferTo(globalTransitions[anyStateID]));
            }
            else
            {
                Debug.LogError(gameObject.name + "全域狀態切換失敗！原因：要切換的AnyState：" + anyStateID + " 不在此狀態機可切換的全域狀態內");
            }
        }
    }

    public abstract class CharacterFSM : FSMSystem
    {
        /// <summary>
        /// 角色狀態機需要的資料
        /// </summary>
        protected internal AIData m_AIData;
        protected internal BattleData m_BattleData;
        protected internal Animator m_Animator;

        protected override void InitFSM()
        {
            m_AIData = GetComponent<AIData>();
            m_BattleData = GetComponent<BattleData>();
            m_Animator = GetComponent<Animator>();
            m_BattleData.Freezed += OnCharacterFreezed;
            m_BattleData.Died += OnCharacterDied;
            base.InitFSM();
        }

        protected override void UnInitFSM()
        {
            m_AIData = null;
            m_Animator = null;
            m_BattleData.Freezed -= OnCharacterFreezed;
            m_BattleData.Died -= OnCharacterDied;
            m_BattleData = null;
            base.UnInitFSM();
        }

        /// <summary>
        /// 轉換狀態的coroutine
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        protected override IEnumerator TransferTo(FSMState targetState)
        {
            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            CharacterFSMState nextState = (CharacterFSMState)targetState;
            if (String.IsNullOrEmpty(nextState.Trigger) == false)
            {
                m_Animator.SetTrigger(nextState.Trigger);
                yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == true);
            }
            yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == false);
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        protected abstract void OnCharacterFreezed(DefendBox damagedPart);

        protected abstract void OnCharacterDied();


    }

    public abstract class NpcFSM : CharacterFSM
    {
        public enum NpcAgent { Normal = 0, Sinister = 1, Cautious = 2 }
        public enum NpcStartingPose { Stand = 0 , Crouch = 1 }

        /// <summary>
        /// 這個Npc的行為風格
        /// </summary>
        public NpcAgent Agent { get { return _agent; } set { _agent = value; } }
        [SerializeField] NpcAgent _agent;

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
        public NpcStartingPose StartingPose { get { return _startingPose; } set { _startingPose = value; } }
        [SerializeField]NpcStartingPose _startingPose;

        protected override FSMState InitValidStates(Enum initialState)
        {
            foreach (var s in StatesLib.BasicNpc.States)
            {
                if ((Npc)s == Npc.Idle) AddState(new Idle(this));
            }
            return validStates[initialState];
        }
    }
}
