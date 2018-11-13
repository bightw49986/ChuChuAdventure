using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem;
using FSM;
using BattleSystem;
using System;

namespace FSM
{
    /// <summary>
    /// 使用狀態機的物種
    /// </summary>
    public enum Species { Goblin = 1 }

    /// <summary>
    /// Npc的狀態行為
    /// </summary>
    public enum Npc { Freezed = -2 , Died =  -1 , Idle = 0, Patrol = 1, Chase  = 2, Confront = 3, Attack = 4 , Approach = 5 }



    namespace StatesLibrary
    {
        #region StatesLib
        public static class StatesLib
        {
            public static class BasicNpc
            {
                public static readonly List<Enum> AnyStates = new List<Enum> { Npc.Freezed, Npc.Died };

                public static readonly List<Enum> States = new List<Enum>
                { Npc.Idle, Npc.Patrol, Npc.Chase, Npc.Confront, Npc.Attack, Npc.Approach };

                public static readonly Dictionary<Enum, string> Triggers = new Dictionary<Enum, string>
                {
                    { Npc.Died,"Died" }, { Npc.Freezed, "Freezed"},{ Npc.Idle, "Idle0" }, { Npc.Patrol, "Patrol"}, { Npc.Chase, "Chase"},{ Npc.Approach, "Approach"}, { Npc.Attack, "Attack0" }, {Npc.Confront, "Confront"}
                };

                public static readonly Dictionary<Enum, Dictionary<int, string>> SubStatesTriggers = new Dictionary<Enum, Dictionary<int, string>>
                {
                    { Npc.Idle, new Dictionary<int, string> { { 0, "Idle0" }, { 1, "Idle1" }, { 2, "Idle2" }, { 3, "Caution" }, { 4, "Ambush" }, { 5, "StandUp"} } },
                    { Npc.Attack, new Dictionary<int, string> { { 0, "Attack0" }, { 1, "Attack1" }, { 2, "Attack2" }, { 3, "JumpAttack" } } },
                    { Npc.Confront, new Dictionary<int, string> { { 0, "Confront"}, { 1, "CloseIn" }, { 2, "Backward" }, { 3, "StrafeLeft" }, { 4, "StrafeRight" } } }
                };
            }
        }
        #endregion

        #region Basic Npc States (Order by ID)

        #region Died
        public class Died : CharacterFSMState
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Died;
                }
            }

            public Died(FSMSystem fsm) : base(fsm) { }

            internal override void CheckConditions()
            {

            }

            internal override void OnStateEnter()
            {

            }

            internal override void OnStateExit()
            {

            }

            internal override void OnStateRunning()
            {

            }
        }
        #endregion

        #region Freezed
        public class Freezed : CharacterFSMState
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Freezed;
                }
            }

            public Freezed(FSMSystem FSM) : base(FSM) { }

            internal override void CheckConditions()
            {

            }

            internal override void OnStateEnter()
            {

            }

            internal override void OnStateExit()
            {

            }

            internal override void OnStateRunning()
            {

            }
        }
        #endregion

        #region Idle
        public class Idle : FSMSubMachine
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Idle;
                }
            }

            public Idle(FSMSystem fsm) : base(fsm) { }

            protected override void AssignSubStatesTriggers()
            {
                SubStatesTriggers = StatesLib.BasicNpc.SubStatesTriggers[Npc.Idle];
            }

            internal override void CheckConditions(int stage)
            {

            }

            internal override void OnStateEnter()
            {
                base.OnStateEnter();
            }

            internal override void OnStateExit()
            {
                
            }

            internal override void OnStateRunning(int stage)
            {

            }
        }
        #endregion

        #region Patrol
        public class Patrol : CharacterFSMState
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Patrol;
                }
            }

            public Patrol(FSMSystem fsm) : base(fsm) { }

            internal override void CheckConditions()
            {

            }

            internal override void OnStateEnter()
            {

            }

            internal override void OnStateExit()
            {

            }

            internal override void OnStateRunning()
            {

            }
        }
        #endregion

        #region Chase
        public class Chase : CharacterFSMState
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Chase;
                }
            }

            public Chase(FSMSystem fsm) : base(fsm) { }



            internal override void CheckConditions()
            {

            }

            internal override void OnStateEnter()
            {

            }

            internal override void OnStateExit()
            {

            }

            internal override void OnStateRunning()
            {

            }
        }
        #endregion

        #region Attack
        public class Attack : FSMSubMachine
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Attack;
                }
            }

            public Attack(FSMSystem fsm) : base(fsm) { }

            protected override void AssignSubStatesTriggers()
            {
                SubStatesTriggers = StatesLib.BasicNpc.SubStatesTriggers[Npc.Attack];
            }

            internal override void CheckConditions(int stage)
            {

            }

            internal override void OnStateEnter()
            {

            }

            internal override void OnStateExit()
            {

            }

            internal override void OnStateRunning()
            {

            }

            internal override void OnStateRunning(int stage)
            {

            }
        }
        #endregion

        #region Confront
        public class Confront : FSMSubMachine
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Confront;
                }
            }

            public Confront(FSMSystem fsm) : base(fsm) { }

            protected override void AssignSubStatesTriggers()
            {
               
            }

            internal override void CheckConditions(int stage)
            {

            }

            internal override void OnStateEnter()
            {

            }

            internal override void OnStateExit()
            {

            }

            internal override void OnStateRunning()
            {

            }

            internal override void OnStateRunning(int stage)
            {

            }
        }
        #endregion

        #region Approach
        public class Approach : CharacterFSMState
        {
            public override Enum StateID
            {
                get
                {
                    return Npc.Approach;
                }
            }

            public Approach(FSMSystem fsm) : base(fsm) { }

            internal override void CheckConditions()
            {

            }

            internal override void OnStateEnter()
            {

            }

            internal override void OnStateExit()
            {

            }

            internal override void OnStateRunning()
            {

            }
        }
        #endregion
       
        #endregion
    }


}
