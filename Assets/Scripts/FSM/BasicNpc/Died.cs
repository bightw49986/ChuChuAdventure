using System;

namespace FSM
{



    namespace StatesLibrary
    {
        public partial class BasicNpc
        {
            /// <summary>
            /// 中斷動作並死掉的行為
            /// </summary>
            public class Died : NpcFSMState
            {
                public override Enum StateID
                {
                    get
                    {
                        return Npc.Died;
                    }
                }

                public Died(NpcFSM fsm) : base(fsm) { }

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
                    base.OnStateRunning();
                }

            }
        }
    }
}
