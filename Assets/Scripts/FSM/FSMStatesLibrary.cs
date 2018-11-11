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
    public enum Species { Goblin = 1}

    /// <summary>
    /// Npc的狀態行為
    /// </summary>
    public enum Npc { Freezed = -2 , Died =  -1 , Idle = 0, Ambush = 1 , Patrol = 2, Chase  = 3, Confront = 4, Attack = 5, Caution = 6, JumpAttack = 7}



    namespace StatesLibrary
    {
        #region StatesLib
        public static class StatesLib
        {
            public static class BasicNpc
            {
                public static readonly List<Enum> AnyStates = new List<Enum> { Npc.Freezed, Npc.Died };

                public static readonly List<Enum> States = new List<Enum>
                { Npc.Idle, Npc.Ambush, Npc.Patrol, Npc.Caution, Npc.Chase, Npc.Attack, Npc.Confront, Npc.JumpAttack };

                public static readonly Dictionary<Enum, string> Triggers = new Dictionary<Enum, string>
                {
                    { Npc.Died,"Died" }, { Npc.Freezed, "Freezed"}, { Npc.Idle , "Idle0"}, { Npc.Ambush, "Ambush"}, { Npc.Patrol, "Patrol"}, { Npc.Chase, "Chase"},
                    { Npc.Confront, "Confront"}, { Npc.Attack, "Attack0"}, {Npc.Caution, "Caution"}, { Npc.JumpAttack, "JumpAttack"}
                };

                public static readonly Dictionary<Enum, Dictionary<int, string>> SubStatesTriggers = new Dictionary<Enum, Dictionary<int, string>>
                {
                    { Npc.Idle, new Dictionary<int, string> { { 0, "Idle0" }, { 1, "Idle1" }, { 2, "Idle2" } } },
                    { Npc.Attack, new Dictionary<int, string> { { 0, "Attack0" }, { 1, "Attack1" }, { 2, "Attack2" } } },
                    { Npc.Confront, new Dictionary<int, string> { { 0, "Confront"}, { 1, "CloseIn" }, { 2, "Backward" }, { 3, "StrafeLeft" }, { 4, "StrafeRight" } } }
                };
            }
        }
        #endregion

        #region Basic Npc States

        #region Died
        public class Died : CharacterFSMState
        {
            public Died(FSMSystem fsm) : base(fsm) { StateID = Npc.Died; }


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
            public Freezed(FSMSystem FSM) : base(FSM) { StateID = Npc.Freezed; }

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
            public Idle(FSMSystem fsm) : base(fsm) { StateID = Npc.Idle; }

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

        #region Ambush
        public class Ambush : CharacterFSMState
        {
            public Ambush(FSMSystem fsm) : base(fsm) { StateID = Npc.Ambush; }

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

        #region Patrol
        public class Patrol : CharacterFSMState
        {
            public Patrol(FSMSystem fsm) : base(fsm) { StateID = Npc.Patrol; }

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
            public Chase(FSMSystem fsm) : base(fsm) { StateID = Npc.Chase; }



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

        #region Confront
        public class Confront : CharacterFSMState
        {
            public Confront(FSMSystem fsm) : base(fsm) { StateID = Npc.Confront; }



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
            public Attack(FSMSystem fsm) : base(fsm) { StateID = Npc.Attack; }

            protected override void AssignSubStatesTriggers()
            {

            }

            internal override void CheckConditions()
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

        #region Caution
        public class Caution : CharacterFSMState
        {
            public Caution(FSMSystem fsm) : base(fsm) { StateID = Npc.Caution; }



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

        #region JumpAttack
        public class JumpAttack : CharacterFSMState
        {
            public JumpAttack(FSMSystem fsm) : base(fsm) { StateID = Npc.JumpAttack; }



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
