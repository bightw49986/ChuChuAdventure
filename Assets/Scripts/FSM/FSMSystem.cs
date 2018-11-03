using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem;
using BattleSystem;

namespace FSM
{
    /// <summary>
    /// 用來規範狀態機應該有的函式的虛擬類別，繼承自MonoBehaviour，所以之後你的狀態機只需繼承FSMSystem即可
    /// </summary>
    public abstract class FSMSystem : MonoBehaviour
    {
        /// <summary>
        /// 這個狀態機是否正在進行狀態轉換
        /// </summary>
        protected internal bool bTranfering;

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
        /// 這個狀態機可切換的狀態清單
        /// </summary>
        protected internal Dictionary<Enum,FSMState> validStates;

        /// <summary>
        /// 這個狀態機可切換的全域狀態，你應該把所有的AnyState可進入的狀態加入這個Dictionary
        /// </summary>
        protected Dictionary<Enum,FSMGlobalState > globalTransitions;

        /// <summary>
        /// 狀態機需要的資料
        /// </summary>
        protected internal AIData m_AIData;
        protected internal BattleData m_BattleData;
        protected internal Animator m_Animator;

        /// <summary>
        /// 當前狀態執行RunState的次數，若狀態是在Update執行，乘上Time.deltaTime就等於執行的秒數
        /// </summary>
        /// <value>RunState的次數</value>
        protected internal float stateTime;

        protected abstract void OnEnable();

        protected abstract void OnDisable();

        /// <summary>
        /// 初始化狀態機，在OnEnable呼叫
        /// </summary>
        protected void InitFSM()
        {
            m_AIData = GetComponent<AIData>();
            m_Animator = GetComponent<Animator>();
            

            validStates = new Dictionary<Enum, FSMState>();
            globalTransitions = new Dictionary<Enum, FSMGlobalState>();
        }

        /// <summary>
        /// 反初始化狀態機釋放資源，在OnDisable中呼叫
        /// </summary>
        protected void UnInitFSM()
        {
            validStates.Clear();
            globalTransitions.Clear();
        }

        /// <summary>
        /// 將此狀態機能轉換的狀態加入validStates
        /// </summary>
        /// /// <param name="stateID">要加入的狀態ID</param>
        /// <param name="state">要加入的狀態</param>
        protected void AddState(Enum stateID, FSMState state)
        {
            if (validStates.ContainsValue(state) == false)
            {
                validStates.Add(stateID,state);
            }
        }

        /// <summary>
        /// 傳入狀態ID查詢狀態並將其刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="stateID">要刪除的狀態ID</param>
        protected void DeleteState(Enum stateID)
        {
            var stateToRemove = from s in validStates.Keys where s == stateID select s;
            validStates.Remove((Enum)stateToRemove);
        }

        /// <summary>
        /// 執行狀態當下要做的事
        /// </summary>
        protected void RunState()
        {
            if (bTranfering == false)
            {
                stateTime += 1;
                print("狀態: " + CurrentState.StateID + "已執行了: " + stateTime * Time.deltaTime + "秒");
                CheckGlobalTransitionConditions();
                CurrentState.OnStateRunning();
            }
        }

        /// <summary>
        /// 轉換狀態的coroutine
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        /// <param name="sTriggerName">要Set的Animator Trigger</param>
        protected IEnumerator TransitionToPerform(FSMState targetState, string sTriggerName)
        {
            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            if (String.IsNullOrEmpty(sTriggerName)==false)
            {
                m_Animator.SetTrigger(sTriggerName);
                yield return new WaitUntil(() => (m_Animator.IsInTransition(0))==true);
            }
            yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == false);
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        /// <summary>
        /// 執行狀態切換
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        /// /// <param name="sTriggerName">該狀態對應的TriggerName</param>
        internal void PerformTransition(FSMState targetState, string sTriggerName)
        {
            if(validStates.ContainsValue(targetState)||globalTransitions.ContainsValue((FSMGlobalState)targetState))
                StartCoroutine(TransitionToPerform(targetState, sTriggerName));
        }

        /// <summary>
        /// 新增全域狀態
        /// </summary>
        /// <param name="stateID">狀態的ID</param>
        /// <param name="globalState">要新增的狀態</param>
        protected void AddGlobalTransition(Enum stateID, FSMGlobalState globalState)
        {
            if (globalTransitions.ContainsValue(globalState) == false)
            {
                globalTransitions.Add(stateID,globalState);
            }
        }

        /// <summary>
        /// 檢查全域狀態的條件
        /// </summary>
        protected abstract void CheckGlobalTransitionConditions();

        /// <summary>
        /// 開始切換到全域狀態
        /// </summary>
        /// <param name="targetGlobalStateID">全域狀態的ID</param>
        protected internal void PerformGlobalTransition(Enum targetGlobalStateID)
        {
            if (globalTransitions.ContainsKey(targetGlobalStateID))
            {
                var globalTransitionToPerform = globalTransitions[targetGlobalStateID];
                StartCoroutine(TransitionToPerform(globalTransitionToPerform, globalTransitionToPerform.TriggerName));
            }
                
        }
    }
}
