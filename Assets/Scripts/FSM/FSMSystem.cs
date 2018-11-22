using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        /// 是否印出現在的狀態
        /// </summary>
        [SerializeField] protected internal bool bLogCurrentState;

        /// <summary>
        /// 是否印狀態切換的Log
        /// </summary>
        [SerializeField] protected internal bool bLogTransition;

        /// <summary>
        /// 現在的StateID，會秀在Inspecter裡面
        /// </summary>
        [SerializeField] protected internal string CurrentStateID;

        /// <summary>
        /// 這個狀態機現在的狀態
        /// </summary>
        protected internal FSMState CurrentState { get { return _currentState; } set { _currentState = value; stateTime = 0f; } }
        FSMState _currentState;

        /// <summary>
        /// 負責Transition的delegate
        /// </summary>
        protected internal Func<FSMState, IEnumerator> TransitionHandler;

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
        /// 初始化狀態機，並設定TransitionHandler
        /// </summary>
        protected virtual void Awake()
        {
            InitFSM();
            SetTransitionHandler();
        }

        /// <summary>
        /// 結束狀態機
        /// </summary>
        protected void OnDestroy()
        {
            UnInitFSM();
        }

        /// <summary>
        /// 更新狀態機，執行狀態Update
        /// </summary>
        protected void Update()
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
            CurrentState = InitValidStates();
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        /// <summary>
        /// 設定負責Transition的delegate
        /// </summary>
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
            if (CurrentState == null) return;
            if (bTranfering == false)
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
        IEnumerator TransferTo(FSMState targetState)
        {
            if (bLogTransition)
                Debug.LogWarning("1，進到父類的了");
            bTranfering = true;
            CurrentState.OnStateExit();
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
            yield return null;
        }

        /// <summary>
        /// 執行狀態切換
        /// </summary>
        /// <param name="stateID">目標狀態的StateID</param>
        internal void PerformTransition(Enum stateID)
        {
            if (validStates == null) return;
            if (validStates.ContainsKey(stateID))
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
        protected internal IEnumerator PerformGlobalTransition(Enum anyStateID)
        {
            if (globalTransitions != null && globalTransitions.ContainsKey(anyStateID))
            {
                if (bLogTransition)
                    Debug.Log("有進AnyState");
                yield return StartCoroutine(TransitionHandler(globalTransitions[anyStateID]));
            }
            else
            {
                Debug.LogError(gameObject.name + "全域狀態切換失敗！原因：要切換的AnyState：" + anyStateID + " 不在此狀態機可切換的全域狀態內");
            }
        }
    }

    #endregion

    #region NPC FSM

    public abstract class NpcFSM : CharacterFSM
    {
        [SerializeField] protected internal  Npc InitialStateID = Npc.Idle;

        public enum Style { Normal = 0, Sinister = 1, Cautious = 2 }
        public enum StartPose { Stand = 0, Crouch = 1 }


        [Header("Personality")]
        [SerializeField] Style _style;
        [SerializeField] Species _species;
        [SerializeField] StartPose _pose;

        /// <summary>
        /// 這個Npc的行為風格
        /// </summary>
        public Style BehaviorStyle { get { return _style; } set { _style = value; } }

        /// <summary>
        /// 這個Npc的種類
        /// </summary>
        /// <value>物種名稱</value>
        public Species Species { get { return _species; } set { _species = value; } }

        /// <summary>
        /// 這個Npc的初始狀態姿勢，會決定Animator初始化後前往的第一個狀態
        /// </summary>
        /// <value>初始姿勢</value>
        public StartPose Pose { get { return _pose; } set { _pose = value; } }


        protected override void SetTransitionHandler()
        {
            TransitionHandler = (arg1) => TransferTo((NpcFSMState)arg1);
            SubMachineTransitionHandler = (arg1, arg2) => TransferToSub((NpcSubMachine)arg1, arg2);
        }

        IEnumerator TransferTo(NpcFSMState targetState)
        {
            if (bLogTransition)
                Debug.Log(CurrentState.StateID + " -> " + targetState.StateID);
            bTranfering = true;
            CurrentState.OnStateExit();
            if (targetState.IsSubMachine== false)
            {
                if (String.IsNullOrEmpty(targetState.Trigger) == false)
                {
                    m_Animator.SetTrigger(targetState.Trigger);
                    yield return new WaitUntil(() => (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState.Trigger) && m_Animator.IsInTransition(0)) == false);
                }
            }
            else if (targetState.IsSubMachine == true)
            {
                NpcSubMachine nextState = (NpcSubMachine)targetState;
                if (String.IsNullOrEmpty(nextState.SubStatesTriggers[0]) == false)
                {
                    m_Animator.SetTrigger(nextState.SubStatesTriggers[0]);
                    yield return new WaitUntil(() => (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(nextState.SubStatesTriggers[0]) && m_Animator.IsInTransition(0)) == false);
                }
            }
            else
            {
                Debug.LogWarning("輸入了一個不正確的目標狀態");
                yield break;
            }
          
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            if (bLogTransition)
                Debug.Log(CurrentState.StateID + " Enter");
            bTranfering = false;
        }

        IEnumerator TransferToSub(NpcSubMachine targetState,int subStateID)
        {
            if (bLogTransition)
                Debug.Log(CurrentState.StateID + " -> " + targetState.StateID + " : " + subStateID);
            bTranfering = true;
            CurrentState.OnStateExit();
            if (String.IsNullOrEmpty(targetState.SubStatesTriggers[subStateID]) == false)
            {
                m_Animator.SetTrigger(targetState.SubStatesTriggers[subStateID]);
            }
            else
            {
                Debug.LogWarning("輸入了一個不正確的目標狀態");
                yield break;
            }

            if (bLogTransition)
            {
                Debug.Log(targetState.SubStatesTriggers[subStateID] + "設定完成，等animator");
            }
            yield return new WaitUntil(() => (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState.SubStatesTriggers[subStateID]) && m_Animator.IsInTransition(0)) == false);
            Debug.Log(targetState.SubStatesTriggers[subStateID] + "的animator有等到");
            targetState.SubState = subStateID;
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
            if (bLogTransition)
                Debug.Log(CurrentState.StateID + " 開始Update");
        }

        protected internal override void PerformTransition(Enum stateID, int subStateID)
        {
            if (validStates == null) { return; }
            if (validStates.ContainsKey(stateID))
            {
                m_currentTransition = StartCoroutine(SubMachineTransitionHandler(validStates[stateID], subStateID));
            }
            else
            {
                Debug.LogError(gameObject.name + "狀態切換失敗！原因：要切換的狀態：" + stateID + " 不在此狀態機可切換的狀態清單內(是否忘記增刪過或遺漏條件判定？)");
            }
        }

        protected override FSMState InitValidStates()
        {
            return QueryValidStates(Species);
        }

        FSMState QueryValidStates(Species species)
        {
            BasicNpc.Died died;
            BasicNpc.Freezed freezed;
            BasicNpc.Idle idle;
            BasicNpc.Patrol patrol;
            BasicNpc.Chase chase;
            BasicNpc.Confront confront;
            BasicNpc.Attack attack;
            BasicNpc.Approach approach;

            switch (species)
            {
                case Species.Goblin: //todo 改成不要寫死
                    {
                        AddState(idle = new BasicNpc.Idle(this, Pose)); 
                        AddState(patrol = new BasicNpc.Patrol(this)); 
                        AddState(chase = new BasicNpc.Chase(this)); 
                        AddState(confront = new BasicNpc.Confront(this)); 
                        AddState(attack = new BasicNpc.Attack(this)); 
                        AddState(approach = new BasicNpc.Approach(this)); 
                        AddGlobalTransition(died = new BasicNpc.Died(this)); 
                        AddGlobalTransition(freezed = new BasicNpc.Freezed(this)); 
                        idle.RegisterTransitions();
                        patrol.RegisterTransitions();
                        chase.RegisterTransitions();
                        confront.RegisterTransitions();
                        attack.RegisterTransitions();
                        approach.RegisterTransitions();
                        died.RegisterTransitions();
                        freezed.RegisterTransitions();
                        break;
                    }
            }

            return validStates[InitialStateID];
        }
    }

    #endregion

}
