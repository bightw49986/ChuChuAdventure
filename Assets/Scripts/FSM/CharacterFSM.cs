using System;
using System.Collections;
using UnityEngine;
using AISystem;
using BattleSystem;


namespace FSM
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BattleData))]
    [RequireComponent(typeof(AIData))]
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

        [Header("BattleConditions")]
        /// <summary>
        /// 是否註冊受傷事件
        /// </summary>
        [Tooltip("是否註冊受傷事件")]public bool CanBeHit = true;
        /// <summary>
        /// 是否註冊KO事件
        /// </summary>
        [Tooltip("是否註冊KO事件")] public bool CanBeKOed;
        /// <summary>
        /// 是否註冊死亡事件
        /// </summary>
        [Tooltip("是否註冊死亡事件")] public bool CanBeDead = true;

        [Header("RootMotion")]
        /// <summary>
        /// 是否Apply root motion
        /// </summary>
        [Tooltip("是否Apply root motion.")] public bool ApplyRootMotion = true;

        protected internal bool m_isDead, m_isFreezed, m_isKOed;



        /// <summary>
        /// 初始化狀態機
        /// </summary>
        protected override void InitFSM()
        {
            m_AIData = GetComponent<AIData>();
            if (m_AIData)
            {
                m_AIData.AIDataInitialized += base.InitFSM;
            }
            else
            {
                Debug.LogError("少一個Component: " + " AIData");
            }
            m_BattleData = GetComponent<BattleData>();
            if(m_BattleData)
            {
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
            }
            else
            {
                Debug.LogError("少一個Component: " + " BattleData");
            }
            m_Animator = GetComponent<Animator>();
            if(m_Animator)
            {
                m_Animator.applyRootMotion = ApplyRootMotion;
            }
            else
            {
                Debug.LogError("少一個Component: " + " Animator");
            }

            base.InitFSM();
        }

        /// <summary>
        /// 反初始化狀態機
        /// </summary>
        protected override void UnInitFSM()
        {
            if (m_AIData)
                m_AIData = null;
            if(m_Animator)
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
            if (m_BattleData)
                m_BattleData = null;

            base.UnInitFSM();
        }

        /// <summary>
        /// 設定負責Transition的delegate
        /// </summary>
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
            Debug.LogError("進到父類別的了！");
            bTranfering = true;
            if (m_Animator.IsInTransition(0) == true)
                yield return new WaitUntil(() => (m_Animator.IsInTransition(0)) == false);

            CurrentState.OnStateExit();
            if (targetState.IsSubMachine == false)
            {
                if (String.IsNullOrEmpty(targetState.Trigger) == false)
                {
                    m_Animator.SetTrigger(targetState.Trigger);
                    yield return new WaitUntil(() => (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState.Trigger) && m_Animator.IsInTransition(0)) == false);
                }
            }
            else if (targetState.GetType() == typeof(FSMSubMachine))
            {
                FSMSubMachine nextState = (FSMSubMachine)targetState;
                if (String.IsNullOrEmpty(nextState.SubStatesTriggers[0]) == false)
                {
                    m_Animator.SetTrigger(nextState.SubStatesTriggers[0]);
                    yield return new WaitUntil(() => (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(nextState.SubStatesTriggers[0]) && m_Animator.IsInTransition(0)) == false);
                }
            }
            else
            {
                Debug.LogError("輸入了一個不正確的目標狀態");
                yield break;
            }
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
            Debug.LogError("進到父類別的了！");
            bTranfering = true;
            CurrentState.OnStateExit();
            if (String.IsNullOrEmpty(targetState.SubStatesTriggers[subStateID]) == false)
            {
                m_Animator.SetTrigger(targetState.SubStatesTriggers[subStateID]);
            }
            else
            {
                Debug.LogError("輸入了一個不正確的目標狀態");
                yield break;
            }
            yield return new WaitUntil(() => (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState.SubStatesTriggers[subStateID]) && m_Animator.IsInTransition(0)) == false);
            targetState.SubState = subStateID;
            CurrentState = targetState;
            CurrentState.OnStateEnter();
            bTranfering = false;
        }

        protected internal void ResetTriggers()
        {
            m_Animator.ResetTrigger("Idle0");
            m_Animator.ResetTrigger("Idle1");
            m_Animator.ResetTrigger("Idle2");
            m_Animator.ResetTrigger("Ambush");
            m_Animator.ResetTrigger("StandUp");
            m_Animator.ResetTrigger("Patrol");
            m_Animator.ResetTrigger("Caution");
            m_Animator.ResetTrigger("Chase");
            m_Animator.ResetTrigger("Approach");
            m_Animator.ResetTrigger("Attack0");
            m_Animator.ResetTrigger("Attack1");
            m_Animator.ResetTrigger("Attack2");
            m_Animator.ResetTrigger("JumpAttack");
            m_Animator.ResetTrigger("Confront");
            m_Animator.ResetTrigger("CloseIn");
            m_Animator.ResetTrigger("Backward");
            m_Animator.ResetTrigger("StrafeLeft");
            m_Animator.ResetTrigger("StrafeRight");
        }

        /// <summary>
        /// 開始切換到子狀態機
        /// </summary>
        /// <param name="stateID">該子狀態機的ID</param>
        /// <param name="subStateID">目標子狀態的階段</param>
        protected internal abstract void PerformTransition(Enum stateID, int subStateID);

        /// <summary>
        /// 角色被打到時觸發的事件
        /// </summary>
        protected abstract void OnCharacterHit();

        /// <summary>
        /// 角色強韌度歸0觸發的事件
        /// </summary>
        /// <param name="damagedPart">Damaged part.</param>
        protected abstract void OnCharacterFreezed(DefendBox damagedPart);

        /// <summary>
        /// 角色倒地觸發的事件
        /// </summary>
        protected abstract void OnCharacterKOed();

        /// <summary>
        /// 角色死亡時觸發的事件
        /// </summary>
        protected abstract void OnCharacterDied();

    }
}
