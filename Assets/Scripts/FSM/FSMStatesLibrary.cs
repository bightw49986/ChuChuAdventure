using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem;
using FSM;
using BattleSystem;
using System;

namespace FSM
{

    public enum BasicFSMStates {Idle = 0,Patrol = 1, Chase  = 2, Confront = 3, Attack = 4,Die =5 }

    public abstract class BasicIdle : FSMState
    {

        float fIdleDuration;

        protected BasicIdle(FSMSystem fsm) : base(fsm) { }

        public override Enum StateID { get; protected set; }

        internal override void CheckConditions()
        {
            if (m_FSM.stateTime >= fIdleDuration)
            {
                StartTransition(m_FSM.validStates[BasicFSMStates.Patrol]);
            }

            //if (DetectPlayerInRange(m_FSM.m_AIData.Target))
            //{
            //    StartTransition(m_FSM.validStates[BasicFSMStates.Chase]);
            //}
        }

        internal override void OnStateEnter()
        {
            StateID = BasicFSMStates.Idle;
        }

        internal override void OnStateExit()
        {

        }

        internal override void OnStateRunning()
        {

        }

        //protected virtual bool DetectPlayerInRange(Transform target)
        //{
        //    float fDistance = Vector3.Distance(m_FSM.transform.position, target.position);

        //    //return fDistance < m_FSM.m_AIData.ChaseRadius;
        //}
    }

    public class BasicPatrol : FSMState
    {
        public BasicPatrol(FSMSystem fsm) : base(fsm) { }

        public override Enum StateID { get; protected set; }

        internal override void CheckConditions()
        {

        }

        internal override void OnStateEnter()
        {
            StateID = BasicFSMStates.Patrol;
        }

        internal override void OnStateExit()
        {

        }

        internal override void OnStateRunning()
        {

        }
    }

    public class BasicChase : FSMState
    {
        public BasicChase(FSMSystem fsm) : base(fsm) { }

        public override Enum StateID { get; protected set; }

        internal override void CheckConditions()
        {

        }

        internal override void OnStateEnter()
        {
            StateID = BasicFSMStates.Chase;
        }

        internal override void OnStateExit()
        {

        }

        internal override void OnStateRunning()
        {

        }
    }


    public class BasicConfront : FSMState
    {
        public BasicConfront(FSMSystem fsm) : base(fsm) { }

        public override Enum StateID { get; protected set; }

        internal override void CheckConditions()
        {

        }

        internal override void OnStateEnter()
        {
            StateID = BasicFSMStates.Confront;
        }

        internal override void OnStateExit()
        {

        }

        internal override void OnStateRunning()
        {

        }
    }

    public class BasicAttack : FSMState
    {
        public BasicAttack(FSMSystem fsm) : base(fsm) { }

        public override Enum StateID { get; protected set; }

        internal override void CheckConditions()
        {

        }

        internal override void OnStateEnter()
        {
            StateID = BasicFSMStates.Attack;
        }

        internal override void OnStateExit()
        {

        }

        internal override void OnStateRunning()
        {

        }
    }

    public class BasicDie : FSMGlobalState
    {
        public BasicDie(FSMSystem fsm,string sTriggerName) : base(fsm,sTriggerName) { }

        public override Enum StateID { get; protected set; }

        internal override void CheckConditions()
        {

        }

        internal override void OnStateEnter()
        {
            StateID = BasicFSMStates.Die;
        }

        internal override void OnStateExit()
        {

        }

        internal override void OnStateRunning()
        {

        }
    }
}
