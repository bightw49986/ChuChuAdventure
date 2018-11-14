using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StatesLibrary;

namespace FSM
{
    #region Base FSM State
    /// <summary>
    /// Base class of finite state machine state.(If associates with animator, use CharacterFSMState instead).
    /// </summary>
    public abstract class FSMState
    {
        /// <summary>
        /// The ID of this state.
        /// </summary>
        /// <value>ID.</value>
        public virtual Enum StateID { get { return _stateID; } set { _stateID = value; } }
        private Enum _stateID;

        /// <summary>
        /// The Collections of states in which this state can transfer to.
        /// </summary>
        protected Dictionary<Enum, FSMState> transitions;


        /// <summary>
        /// Return if this state is a sub - machine state.
        /// </summary>
        public readonly bool IsSubMachine;

        /// <summary>
        /// The finite state machine which handles this state.
        /// </summary>
        protected internal FSMSystem m_FSM { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FSM.FSMState"/> class.
        /// </summary>
        /// <param name="FSM">The finite state machine that handles this state(Most case use "this" keyword).</param>
        protected FSMState(FSMSystem FSM)
        {
            m_FSM = FSM;
            transitions = new Dictionary<Enum, FSMState>();
        }

        protected virtual void RegisterFSM(FSMSystem FSM)
        {
            if (m_FSM != null)
            {
                m_FSM = FSM;
            }
        }

        /// <summary>
        /// Add a state to this state's transition.
        /// </summary>
        /// <param name="targetState">State to add.</param>
        protected void AddTransition(FSMState targetState)
        {
            if (m_FSM.validStates.ContainsKey(targetState.StateID) == true)
            {
                if (transitions.ContainsKey(targetState.StateID) == false)
                {
                    transitions.Add(targetState.StateID, targetState);
                }
            }
        }

        /// <summary>
        /// Remove a state from the collection of transitions.
        /// </summary>
        /// <param name="targetStateID">The ID of the state to remove.</param>
        protected void DeleteTransition(Enum targetStateID)
        {
            var transitionToRemove = from s in transitions where s.Key == targetStateID select s.Key;
            transitions.Remove((Enum)transitionToRemove);
        }

        /// <summary>
        /// Starts a transition to target state.
        /// </summary>
        /// <param name="targetStateID">The ID of targetstate</param>
        protected virtual void StartTransition(Enum targetStateID)
        {
            if (transitions.ContainsKey(targetStateID))
                m_FSM.PerformTransition(targetStateID);
        }

        /// <summary>
        /// Execute when state machine enter this state. 
        /// </summary>
        internal abstract void OnStateEnter();

        /// <summary>
        /// Execute what this state should do. Pick a method in "Update", "FixedUpdate", or other callback function to call this method.
        /// </summary>
        internal abstract void OnStateRunning();

        /// <summary>
        /// Execute when state machine starts to transfer to another state.
        /// </summary>
        internal abstract void OnStateExit();

        /// <summary>
        /// Check if the conditions for transitions been satisfied then change states.
        /// </summary>
        internal abstract void CheckConditions();

    }
    #endregion

    #region AnimatorFSM State
    /// <summary>
    /// Finite state machine state which associates with Unity's animator.
    /// </summary>
    public abstract class CharacterFSMState : FSMState
    {
        protected internal new CharacterFSM m_FSM { get; set; }

        /// <summary>
        /// The trigger to activate this state in animator.
        /// </summary>
        public string Trigger { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FSM.CharacterFSMState"/> class.
        /// </summary>
        /// <param name="FSM">The finite state machine that handles this state(Most case use "this" keyword).</param>
        protected CharacterFSMState(CharacterFSM FSM) : base(FSM)
        {
            m_FSM = (CharacterFSM)base.m_FSM;
            Trigger = StatesLib.BasicNpc.Triggers[StateID];
        }

        protected abstract void OnAnimatorMove();

    }

    /// <summary>
    /// A finite state machine state which repersents a group of animator states that shares a same FSM behavior.
    /// </summary>
    public abstract class FSMSubMachine : CharacterFSMState
    {
        /// <summary>
        /// The stage this submachine is currently at.
        /// </summary>
        /// <value>Repersents the different animator states in Unity animator.</value>
        public int SubState { get; protected set; }

        /// <summary>
        /// Triggers for each stage.
        /// </summary>
        public Dictionary<int, string> SubStatesTriggers;

        protected FSMSubMachine(CharacterFSM FSM) : base(FSM)
        {
            SubState = 0;
            SubStatesTriggers = new Dictionary<int, string>();
        }

        internal void StartTransition(Enum targetStateID, int iSubState)
        {
            if (transitions.ContainsKey(targetStateID))
                m_FSM.PerformTransition(targetStateID, iSubState);
        }

        protected internal abstract void AssignSubStatesTriggers();

        internal override void OnStateEnter()
        {
            AssignSubStatesTriggers();
        }

        internal override void OnStateRunning()
        {
            OnStateRunning(SubState);
        }

        internal sealed override void CheckConditions()
        {
            CheckConditions(SubState);
        }

        internal abstract void OnStateRunning(int stage);

        internal abstract void CheckConditions(int stage);
    }


    #endregion

    #region Npc FSMState
    /// <summary>
    /// Finite state machine state with npc stats.
    /// </summary>
    public abstract class NpcFSMState : CharacterFSMState
    {
        protected GameObject Player;

        protected float m_fFaceCautionRange;
        protected float m_fSqrFaceCautionRange;
        protected float m_fBackCaurionRange;
        protected float m_fSqrBackCaurionRange;
        protected float m_fFOV;

        protected float m_fChaseRange;
        protected float m_fSqrChaseRange;

        protected float m_fJumpAtkRange;
        protected float m_fSqrJumpAtkRange;

        protected float m_fAtkRange;
        protected float m_fSqrAtkRange;

        protected internal new NpcFSM m_FSM { get; set; }

        protected NpcFSMState(NpcFSM FSM) : base(FSM)
        {
            m_FSM = (NpcFSM)base.m_FSM;
            GetNpcAIStats();

        }

        void GetNpcAIStats()
        {
            Player = GameObject.FindWithTag("Player");
            if (Player == null)
            {
                Debug.LogError("Can find GO with tag \" Player \"."); return;
            }
            m_fFaceCautionRange = m_FSM.m_AIData.FaceCautionRange;
            m_fBackCaurionRange = m_FSM.m_AIData.BackCaurionRange;
            m_fFOV = m_FSM.m_AIData.FOV;
            m_fChaseRange = m_FSM.m_AIData.ChaseRange;
            m_fJumpAtkRange = m_FSM.m_AIData.JumpAtkRange;
            m_fAtkRange = m_FSM.m_AIData.AtkRange;
            m_fSqrFaceCautionRange = m_fFaceCautionRange * m_fFaceCautionRange;
            m_fSqrBackCaurionRange = m_fBackCaurionRange * m_fBackCaurionRange;
            m_fSqrChaseRange = m_fChaseRange * m_fChaseRange;
            m_fSqrJumpAtkRange = m_fJumpAtkRange * m_fJumpAtkRange;
            m_fSqrAtkRange = m_fAtkRange * m_fAtkRange;
        }

        internal override void OnStateEnter()
        {
            RegisterTransitions();
        }

        protected virtual void RegisterTransitions()
        {
            foreach (var s in StatesLib.BasicNpc.Transitions[StateID])
            {
                AddTransition(m_FSM.validStates[s]);
            }
        }
    }

    public abstract class NpcSubMachine : NpcFSMState
    {
        /// <summary>
        /// The stage this submachine is currently at.
        /// </summary>
        /// <value>Repersents the different animator states in Unity animator.</value>
        public int SubState { get; protected set; }

        /// <summary>
        /// Triggers for each stage.
        /// </summary>
        public Dictionary<int, string> SubStatesTriggers;

        protected NpcSubMachine(NpcFSM FSM) : base(FSM)
        {
            SubState = 0;
            SubStatesTriggers = new Dictionary<int, string>();
        }

        protected abstract void AssignSubStatesTriggers();

        internal override void OnStateEnter()
        {
            base.OnStateEnter();
            AssignSubStatesTriggers();
        }

        internal override void OnStateRunning()
        {
            OnStateRunning(SubState);
        }

        internal sealed override void CheckConditions()
        {
            CheckConditions(SubState);
        }

        internal void StartTransition(Enum targetStateID, int iSubState)
        {
            if (transitions.ContainsKey(targetStateID))
                m_FSM.PerformTransition(targetStateID,iSubState);
        }

        internal IEnumerator TransferToSubState(int iSubStateID)
        {
            m_FSM.bTranfering = true;
            m_FSM.m_Animator.SetTrigger(SubStatesTriggers[iSubStateID]);
            yield return new WaitUntil(() => (m_FSM.m_Animator.IsInTransition(0)) == true);
            yield return new WaitUntil(() => (m_FSM.m_Animator.IsInTransition(0)) == false);
            SubState = iSubStateID;
            m_FSM.bTranfering = false;
        }

        internal abstract void OnStateRunning(int stage);

        internal abstract void CheckConditions(int stage);
    }
    #endregion
}


