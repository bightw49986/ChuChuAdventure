using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public abstract class FSMState
    {
        public abstract Enum StateID { get;protected set; }
        protected Dictionary<FSMState, string> transitions;
        internal FSMSystem m_FSM;

        protected FSMState(FSMSystem FSM)
        {
            m_FSM = FSM;
            transitions = new Dictionary<FSMState, string>();
        }

        /// <summary>
        /// 為這個狀態添加一個Transition，該Transition必須是狀態機目前可切換的狀態Transition
        /// </summary>
        /// <param name="targetState">目標的狀態</param>
        /// <param name="sTriggerName">該狀態對應的Animator Trigger</param>
        protected void AddTransition(FSMState targetState, string sTriggerName)
        {
            if (m_FSM.validStates.ContainsValue(targetState) == true)
            {
                if (transitions.ContainsKey(targetState) == false)
                {
                    transitions.Add(targetState, sTriggerName);
                }
            }
        }

        /// <summary>
        /// 刪除這個狀態的某個Transition，小心使用這個方法並記得在CheckCondition當中對每個可切換的Transition做防呆，否則會導致null reference
        /// </summary>
        /// <param name="targetState">要刪除的目標狀態</param>
        protected void DeleteTransition(FSMState targetState)
        {
            var transitionToRemove = from s in transitions where s.Key == targetState select s.Key;
            transitions.Remove((FSMState)transitionToRemove);

        }

        /// <summary>
        /// 呼叫這個狀態的OnStateExit()並開始執行一段Transition
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        protected virtual void StartTransition(FSMState targetState)
        {
            if (transitions.ContainsKey(targetState))
            m_FSM.PerformTransition(targetState, transitions[targetState]);
        }

        internal abstract void OnStateEnter();

        internal abstract void OnStateRunning();

        internal abstract void OnStateExit();

        /// <summary>
        /// 檢查轉換狀態的條件，你應該總是在這裡呼叫StartTransition()，並且在判斷每個Transition之前對其做防呆，檢查它是否存在transitions當中
        /// </summary>
        internal abstract void CheckConditions();

    }

    public abstract class FSMGlobalState : FSMState
    {
        public string TriggerName { get; set; }

        protected FSMGlobalState(FSMSystem FSM,string sTriggerName) : base(FSM)
        {
            TriggerName = sTriggerName;
        }
    }
}


