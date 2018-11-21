using System.Collections;
using System.Collections.Generic;
using AISystem;
using FSM;
using BattleSystem;
using System;

namespace FSM
{
    /// <summary>
    /// 使用狀態機的物種
    /// </summary>
    public enum Species { Goblin = 0 }

    /// <summary>
    /// Npc的狀態行為
    /// </summary>
    public enum Npc { Freezed = -2, Died = -1, Idle = 0, Patrol = 1, Chase = 2, Confront = 3, Attack = 4, Approach = 5 }

    namespace StatesLibrary
    {
        public static class StatesLib
        {
            public static class BasicNpc
            {
                //public static readonly List<Npc> AnyStates = new List<Npc> { Npc.Freezed, Npc.Died };

                //public static readonly Dictionary<Species, List<Npc>> States = new Dictionary<Species, List<Npc>>
                //{
                //    {Species.Goblin,new List<Npc>{ Npc.Idle, Npc.Patrol, Npc.Chase, Npc.Confront, Npc.Attack, Npc.Approach }}
                //};


                public static readonly Dictionary<Enum, string> Triggers = new Dictionary<Enum, string>
                {
                    { Npc.Died,"Died" }, { Npc.Freezed, "Freezed"},{ Npc.Idle, "Idle0" }, { Npc.Patrol, "Patrol"}, { Npc.Chase, "Chase"},{ Npc.Approach, "Approach"}, { Npc.Attack, "Attack0" }, {Npc.Confront, "Confront"}
                };

                public static readonly Dictionary<Enum, List<Enum>> Transitions = new Dictionary<Enum, List<Enum>>
                {
                    {Npc.Died,new List<Enum>()},
                    {Npc.Freezed,new List<Enum>{Npc.Idle, Npc.Patrol, Npc.Chase, Npc.Confront, Npc.Attack, Npc.Approach}},
                    {Npc.Idle,new List<Enum>{ Npc.Idle,Npc.Approach,Npc.Patrol,Npc.Chase,Npc.Confront,Npc.Attack}},
                    {Npc.Patrol,new List<Enum>{Npc.Idle,Npc.Confront,Npc.Chase}},
                    {Npc.Approach,new List<Enum>{Npc.Idle,Npc.Chase,Npc.Attack, Npc.Confront}},
                    {Npc.Chase,new List<Enum>{Npc.Idle,Npc.Approach,Npc.Attack, Npc.Confront}},
                    {Npc.Attack,new List<Enum>{Npc.Idle,Npc.Confront,Npc.Approach, Npc.Confront}},
                    {Npc.Confront,new List<Enum>{Npc.Idle,Npc.Chase,Npc.Attack,Npc.Approach,Npc.Confront}}
                };

                public static readonly Dictionary<Enum, Dictionary<int, string>> SubStatesTriggers = new Dictionary<Enum, Dictionary<int, string>>
                {
                    { Npc.Idle, new Dictionary<int, string> { { 0, "Idle0" }, { 1, "Idle1" }, { 2, "Idle2" }, { 3, "Caution" }, { 4, "Ambush" }, { 5, "StandUp"} } },
                    { Npc.Attack, new Dictionary<int, string> { { 0, "Attack0" }, { 1, "Attack1" }, { 2, "Attack2" }, { 3, "JumpAttack" } } },
                    { Npc.Confront, new Dictionary<int, string> { { 0, "Confront"}, { 1, "CloseIn" }, { 2, "Backward" }, { 3, "StrafeLeft" }, { 4, "StrafeRight" } } }
                };
            }
        }
    }
}
