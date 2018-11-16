using System;
using System.Collections;
using UnityEngine;
using AISystem;
using BattleSystem;


namespace FSM
{
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


        Func<FSMState, int, IEnumerator> _subMachineTransitionHandler;

        public bool CanBeHit;
        public bool CanBeKOed;
        public bool CanBeDead;

        protected internal bool m_isDead, m_isFreezed, m_isKOed;

        protected virtual void OnAnimatorMove()
        {
            m_Animator.ApplyBuiltinRootMotion();
            if (m_Animator == null || CurrentState == null) return;
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
            Debug.LogWarning("進到父類別的了！");
            if (TargetStateID == targetState.StateID) yield break;
            TargetStateID = targetState.StateID;
            bTranfering = true;
            OriginState = CurrentState;
            CurrentState.OnStateExit();
            if (targetState.IsSubMachine == false)
            {
                if (String.IsNullOrEmpty(targetState.Trigger) == false)
                {
                    if (m_Animator.IsInTransition(0))
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
            Debug.LogWarning("進到父類別的了！");
            if (TargetStateID == targetState.StateID) yield break;
            TargetStateID = targetState.StateID;
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
        protected internal abstract void PerformTransition(Enum stateID, int subStateID);
        //{
        //    if (validStates != null && validStates.ContainsKey(stateID))
        //    {
        //        if (validStates[stateID].GetType() == typeof(FSMSubMachine))
        //        {
        //            FSMSubMachine nextState = (FSMSubMachine)validStates[stateID];
        //            m_currentTransition = StartCoroutine(SubMachineTransitionHandler(nextState, subStateID));
        //        }
        //        else
        //        {
        //            FSMState nextState = validStates[stateID];
        //            m_currentTransition = StartCoroutine(TransitionHandler(nextState));
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError(gameObject.name + "狀態切換失敗！原因：要切換的狀態：" + stateID + " 不在此狀態機可切換的狀態清單內(是否忘記增刪過或遺漏條件判定？)");
        //    }
        //}


        protected abstract void OnCharacterHit();

        protected abstract void OnCharacterFreezed(DefendBox damagedPart);

        protected abstract void OnCharacterKOed();

        protected abstract void OnCharacterDied();

    }

    #endregion

}
