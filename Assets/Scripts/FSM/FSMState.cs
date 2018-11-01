using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public abstract class IFSMState
    {
        public int StateID { get; protected set; }
        public string StateName { get; protected set; }
        protected Dictionary<IFSMState, Func<IEnumerator>> transitions;
        internal IFSMSystem m_FSM;

        protected IFSMState(IFSMSystem FSM)
        {
            m_FSM = FSM;
        }

        protected abstract void AddTransition(IFSMState targetState, Func<IEnumerator> transition);

        protected abstract void DeleteTransition(IFSMState targetState);

        protected abstract void DeleteTransition(Func<IEnumerator> transition);

        protected abstract void StartTransition(IFSMState targetState);

        protected abstract void StartTransition(Func<IEnumerator> transition);

        internal abstract void CheckConditions();

        internal virtual void OnStateEnter() { }

        internal virtual void OnStateRunning() { }

        internal virtual void OnStateExit() { }

    }

    /// <summary>
    /// 所有狀態的基底類別，狀態機的每一個狀態都應該繼承此類別
    /// </summary>
    public class FSMState : IFSMState
    {
        /// <summary>
        /// 初始化狀態，將狀態機的參考傳給狀態並儲存
        /// </summary>
        /// <param name="FSM">使用這個狀態的狀態機</param>
        protected FSMState(IFSMSystem FSM) : base(FSM)
        {
            transitions = new Dictionary<IFSMState, Func<IEnumerator>>();
        }

        /// <summary>
        /// 為這個狀態添加一個Transition，該Transition必須是狀態機目前可切換的狀態Transition
        /// </summary>
        /// <param name="targetState">目標的狀態</param>
        /// <param name="transition">Transition的Function</param>
        protected override void AddTransition(IFSMState targetState, Func<IEnumerator> transition)
        {
            if (m_FSM.validStates.Contains(targetState) == true)
            {
                if (transitions.ContainsKey(targetState) == false)
                {
                    transitions.Add(targetState, transition);
                }
            }
        }

        /// <summary>
        /// 刪除這個狀態的某個Transition，小心使用這個方法並記得在CheckCondition當中對每個可切換的Transition做防呆，否則會導致null reference
        /// </summary>
        /// <param name="targetState">要刪除的目標狀態</param>
        protected sealed override void DeleteTransition(IFSMState targetState)
        {
            var transitionToRemove = from s in transitions where s.Key == targetState select s.Key;
            transitions.Remove((IFSMState)transitionToRemove);

        }

        /// <summary>
        /// 刪除這個狀態的某個Transition，小心使用這個方法並記得在CheckCondition當中對每個可切換的Transition做防呆，否則會導致null reference
        /// </summary>
        /// <param name="transition">要刪除的Transition</param>
        protected sealed override void DeleteTransition(Func<IEnumerator> transition)
        {
            var transitionToRemove = from s in transitions where s.Value == transition select s.Key;
            transitions.Remove((IFSMState)transitionToRemove);

        }

        /// <summary>
        /// 檢查轉換狀態的條件，你應該總是在這裡呼叫StartTransition()，並且在判斷每個Transition之前對其做防呆，檢查它是否存在transitions當中
        /// </summary>
        internal override void CheckConditions()
        {

        }

        /// <summary>
        /// 呼叫這個狀態的OnStateExit()並開始執行一段Transition
        /// </summary>
        /// <param name="targetState">目標狀態</param>
        protected sealed override void StartTransition(IFSMState targetState)
        {
            OnStateExit();
            m_FSM.dTransitionToPerform = transitions[targetState];
            m_FSM.PerformTransition(m_FSM.dTransitionToPerform);
        }

        /// <summary>
        /// 呼叫這個狀態的OnStateExit()並開始執行一段Transition
        /// </summary>
        /// <param name="transition">要執行的Transition</param>
        protected sealed override void StartTransition(Func<IEnumerator> transition)
        {
            OnStateExit();
            m_FSM.dTransitionToPerform = transition;
            m_FSM.PerformTransition(m_FSM.dTransitionToPerform);
        }

    }
}


