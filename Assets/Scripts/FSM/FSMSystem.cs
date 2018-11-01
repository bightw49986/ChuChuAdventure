using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// 用來規範狀態機應該有的函式的虛擬類別，繼承自MonoBehaviour，所以之後你的狀態機只需繼承FSMSystem_Base即可
    /// </summary>
    public abstract class IFSMSystem : MonoBehaviour
    {
        /// <summary>
        /// 這個狀態機是否正在進行狀態轉換
        /// </summary>
        protected internal bool bTranfering;

        /// <summary>
        /// 這個狀態機現在的狀態
        /// </summary>
        protected internal IFSMState CurrentState { get { return _currentState; } set { _currentState = value; stateTime = 0f; } }
        private IFSMState _currentState;

        /// <summary>
        /// 進入全域狀態前的狀態
        /// </summary>
        /// <value>原本的狀態</value>
        protected internal IFSMState OriginState { get; set; }

        /// <summary>
        /// 這個狀態機可切換的狀態清單
        /// </summary>
        protected internal List<IFSMState> validStates;

        /// <summary>
        /// 這個狀態機可切換的全域狀態，你應該把所有的AnyState可進入的狀態加入這個Dictionary
        /// </summary>
        protected Dictionary<IFSMState, Func<IEnumerator>> globalTransitions;

        /// <summary>
        /// 狀態切換的delegate，當轉換條件發生時，應該將轉換的執行方式指派給這個delegate
        /// </summary>
        internal Func<IEnumerator> dTransitionToPerform;

        /// <summary>
        /// 狀態機需要的資料
        /// </summary>
        protected internal object data;

        /// <summary>
        /// 當前狀態執行RunState的次數，若狀態是在Update執行，乘上Time.deltaTime就等於執行的秒數
        /// </summary>
        /// <value>RunState的次數</value>
        protected internal float stateTime;

        /// <summary>
        /// 將此狀態機能轉換的狀態加入validStates
        /// </summary>
        /// <param name="states">要加入的狀態</param>
        protected abstract void AddStates(params IFSMState[] states);

        /// <summary>
        /// 將傳入的狀態從此狀態機能轉換的狀態中刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="state">要刪除的狀態</param>
        protected abstract void DeleteState(IFSMState state);

        /// <summary>
        /// 用傳入的狀態ID查詢狀態並將其刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="stateID">要刪除的狀態ID</param>
        protected abstract void DeleteState(int stateID);

        /// <summary>
        /// 用傳入的狀態名將狀態刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="stateName">要刪除的狀態名稱</param>
        protected abstract void DeleteState(string stateName);

        /// <summary>
        /// 執行狀態當下要做的事
        /// </summary>
        protected abstract void RunState();

        /// <summary>
        /// 開始轉換狀態，呼叫這個方法前，必須確定你已指派新的dTransitionToPerform
        /// </summary>
        /// <param name="transitionToPerform">目標狀態的delegate</param>
        internal abstract void PerformTransition(Func<IEnumerator> transitionToPerform);

        /// <summary>
        /// 新增全域狀態
        /// </summary>
        /// <param name="globalState">要新增的狀態</param>
        /// <param name="transition">該狀態的Transition</param>
        protected abstract void AddGlobalTransition(IFSMState globalState, Func<IEnumerator> transition);

        /// <summary>
        /// 檢查全域狀態的條件，你應該總是在這裡更改dTransitionToPerform並呼叫PerformGlobalTransition()，並且決定各個全域狀態之間的優先順序
        /// </summary>
        protected abstract void CheckGlobalTransitionConditions();

        /// <summary>
        /// 開始切換到全域狀態
        /// </summary>
        /// <param name="globalTransitionToPerform">全域狀態的delegate</param>
        protected abstract void PerformGlobalTransition(Func<IEnumerator> globalTransitionToPerform);

        /// <summary>
        /// 重置狀態機的狀態時間
        /// </summary>
        protected internal abstract void ReSetStateTimer();
    }


    ///<summary>
    /// 狀態機的基底類別，當使用狀態機繼承這個類別
    /// </summary>
        public class FSMSystem : IFSMSystem
    {
        protected virtual void OnEnable()
        {
            validStates = new List<IFSMState>();
            globalTransitions = new Dictionary<IFSMState, Func<IEnumerator>>();
        }

        protected virtual void OnDisable()
        {
            validStates.Clear();
            globalTransitions.Clear();
        }

        /// <summary>
        /// 將此狀態機能轉換的狀態加入validStates
        /// </summary>
        /// <param name="states">要加入的狀態</param>
        protected sealed override void AddStates(params IFSMState[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (validStates.Contains(states[i]) == false)
                {
                    validStates.Add(states[i]);
                }
            }
        }

        /// <summary>
        /// 將傳入的狀態從此狀態機能轉換的狀態中刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="state">要刪除的狀態</param>
        protected sealed override void DeleteState(IFSMState state)
        {
            var stateToRemove = from s in validStates where s == state select s;
            validStates.Remove((IFSMState)stateToRemove);
        }

        /// <summary>
        /// 用傳入的狀態ID查詢狀態並將其刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="stateID">要刪除的狀態ID</param>
        protected sealed override void DeleteState(int stateID)
        {
            var stateToRemove = from s in validStates where s.StateID == stateID select s;
            validStates.Remove((IFSMState)stateToRemove);
        }

        /// <summary>
        /// 用傳入的狀態名將狀態刪除，全域狀態不會被刪除
        /// </summary>
        /// <param name="stateName">要刪除的狀態名稱</param>
        protected sealed override void DeleteState(string stateName)
        {
            var stateToRemove = from s in validStates where s.StateName == stateName select s;
            validStates.Remove((IFSMState)stateToRemove);
        }

        /// <summary>
        /// 執行狀態當下要做的事
        /// </summary>
        protected sealed override void RunState()
        {
            if (bTranfering == false)
            {
                stateTime += 1;
                print("狀態: " + CurrentState.StateName + "已執行了: " + stateTime * Time.deltaTime + "秒");
                CheckGlobalTransitionConditions();
                CurrentState.OnStateRunning();
            }
        }


        internal sealed override void PerformTransition(Func<IEnumerator> transitionToPerform)
        {
            StartCoroutine(transitionToPerform());
        }

        /// <summary>
        /// 新增全域狀態
        /// </summary>
        /// <param name="globalState">要新增的狀態</param>
        /// <param name="transition">該狀態的Transition</param>
        protected sealed override void AddGlobalTransition(IFSMState globalState, Func<IEnumerator> transition)
        {
            if (globalTransitions.ContainsKey(globalState) == false)
            {
                globalTransitions.Add(globalState, transition);
            }
        }

        /// <summary>
        /// 檢查全域狀態的條件，你應該總是在這裡更改dTransitionToPerform並呼叫PerformGlobalTransition()，並且決定各個全域狀態之間的優先順序
        /// </summary>
        protected override void CheckGlobalTransitionConditions()
        {

        }

        /// <summary>
        /// 開始切換到全域狀態
        /// </summary>
        /// <param name="globalTransitionToPerform">全域狀態的delegate</param>
        protected sealed override void PerformGlobalTransition(Func<IEnumerator> globalTransitionToPerform)
        {
            StartCoroutine(globalTransitionToPerform());
        }

        protected internal sealed override void ReSetStateTimer()
        {
            stateTime = 0f;
        }
    }
}
