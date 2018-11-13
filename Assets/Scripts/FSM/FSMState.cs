using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM.StatesLibrary;

namespace FSM
{
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
        internal FSMSystem m_FSM;



        /// <summary>
        /// Initializes a new instance of the <see cref="T:FSM.FSMState"/> class.
        /// </summary>
        /// <param name="FSM">The finite state machine that handles this state(Most case use "this" keyword).</param>
        protected FSMState(FSMSystem FSM)
        {
            m_FSM = FSM;
            transitions = new Dictionary<Enum, FSMState>();
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
                    transitions.Add(targetState.StateID,targetState);
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

    /// <summary>
    /// Finite state machine state which associates with Unity's animator.
    /// </summary>
    public abstract class CharacterFSMState : FSMState
    {
        /// <summary>
        /// The trigger to activate this state in animator.
        /// </summary>
        public string Trigger { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FSM.CharacterFSMState"/> class.
        /// </summary>
        /// <param name="FSM">The finite state machine that handles this state(Most case use "this" keyword).</param>
        protected CharacterFSMState(FSMSystem FSM) : base(FSM)
        {
            Trigger = StatesLib.BasicNpc.Triggers[StateID];
        }
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

        protected FSMSubMachine(FSMSystem FSM) : base(FSM)
        {
            SubState = 0;
            SubStatesTriggers = new Dictionary<int, string>();
        }

        protected abstract void AssignSubStatesTriggers();

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
}


