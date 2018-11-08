﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem;

public sealed class PlayerFSMGenerater : MonoBehaviour, FSMGenerater
{

    void InitState(StateSystem state)
    {
        state.FSM = this;
        state.inputMotionController = inputMotionController;
        state.player = player;
    }
    void Awake()
    {
        //取得Animator
        AnimPlayer = GetComponent<Animator>();
        player = GetComponent<Player>();
        inputMotionController = GetComponent<InputMotionController>();
        SubscribeStateLibrary = new Dictionary<string, StateSystem>();//狀態機dictionary
    }
    void Start()
    {
        SubscribePlayerEvents();
        //開始新增StateLibrary中的狀態
        AddState();
        SCurrentState = "Idle";
        BAllowTransit = true;
    }

    void SubscribePlayerEvents()
    {
        player.Died += () => { playerDied = true; };
        player.KOed += () => { playerKOed = true; };
        player.Freezed += OnPlayerFreezed;
        player.Hit += () => { playerHit = true; };
    }

    void OnPlayerFreezed(DefendBox damagedPart)
    {
        DefendBox_Player playerPart = (DefendBox_Player)damagedPart;
        switch (playerPart.DamagePart)
        {
            case EDamagePart.Middle:
                {
                    playerFreezed_Middle = true;
                    break;
                }

            case EDamagePart.Left:
                {
                    playerFreezed_Left = true;
                    break;
                }
            case EDamagePart.Right:
                {
                    playerFreezed_Right = true;
                    break;
                }
            default:
                return;
        }
    }

        void Update()
        {
            //if (player.bPreEnter == true)
            //{
            //    BAllowTransit = false;
            //}
            animatorInfo = AnimPlayer.GetCurrentAnimatorStateInfo(0);
            if (AnyState())
            {
                StartCoroutine(Transit(SNextState));
            }
            else if (BAllowTransit && SNextState != null)
            {
                StartCoroutine(Transit(SNextState));
            }
            DoCurrentState(SubscribeStateLibrary[SCurrentState]);//擋的事交給coroutine的bools切換
        }

        void LateUpdate()
        {
            playerHit = playerFreezed_Middle = playerFreezed_Left = playerFreezed_Right =  playerKOed = playerDied = false;
        }

        public IEnumerator Transit(string _state)
        {
            if (SubscribeStateLibrary.ContainsKey(SNextState))//防呆！如果沒有註冊該狀態跳Log
            {
                AnimPlayer.SetBool(SCurrentState, false);//(Animator)把原本進這狀態的transition關上
                LeaveCurrentState(SubscribeStateLibrary[SCurrentState]);//執行Leave
                SCurrentState = SNextState;//目前狀態->新的狀態
                SNextState = null;//把排隊的位置清掉
                BAllowTransit = false;//新的狀態即重新計算可轉換的時機
                AnimPlayer.SetBool(SCurrentState, true);//(Animator)打開前往下一個狀態的transition
                TransitCurrentState(SubscribeStateLibrary[SCurrentState]);//執行Transit
                player.bBlockInput = true;//Transition期間阻擋一切輸入(開)
                yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == true);//(Animator)等，直到動作切換結束
                yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == false);//(Animator)等，直到動作切換結束
                player.bBlockInput = false;//Transition期間阻擋一切輸入(關)
                EnterCurrentState(SubscribeStateLibrary[SCurrentState]);//轉換結束後呼叫Enter該狀態改變規範                                                                   //player.bPreEnter = false;
                BAllowTransit = animatorInfo.IsTag("notAttack") ? true : false;//如果此動作沒設有預輸入事件，狀態轉換結束後即可轉換到其他狀態
            }
            else print("An accidental state be called !");//防呆！如果沒有註冊該狀態跳Log
        }
        /// <summary>
        /// 每個動作的前多少時間阻斷「所有」輸入，之所以要與Transit分開主要因為攻擊動作的預輸入;一段可記錄預輸入的時間
        /// </summary>
        /// <returns></returns>
        public void DoCurrentState(StateSystem state)
        {
            state.Check();
            state.Do();
        }
        public void LeaveCurrentState(StateSystem state)
        {
            state.Leave();
        }
        public void EnterCurrentState(StateSystem state)
        {
            state.Enter();
        }
        public void TransitCurrentState(StateSystem state)
        {
            state.Transit();
        }
        public bool AnyState()
        {
            if (playerDied == true)
            {
                SNextState = "Dead";
                return true;
            }
            if (playerKOed == true)
            {
                SNextState = "Down";
                return true;
            }
            if (playerFreezed_Middle == true)
            {
                SNextState = "FrontGetHit";
                return true;
            }
            if (playerFreezed_Left ==true)
            {
                SNextState = "LeftGetHit";
                return true;
            }
            if (playerFreezed_Right == true)
            {
                SNextState = "RightGetHit";
                return true;
            }
            if (player.bCanDash && inputMotionController.m_fDInput >= 0.1f)
            {
                SNextState = "Dash";
                return true;
            }
            return false;
        }
        public void AddState()
        {
            SubscribeStateLibrary.Add("Idle", new ChuChuIdleToRun());
            InitState(SubscribeStateLibrary["Idle"]);
            SubscribeStateLibrary.Add("JumpStart", new ChuChuJumpStart());
            InitState(SubscribeStateLibrary["JumpStart"]);
            SubscribeStateLibrary.Add("JumpLoop", new ChuChuJumpLoop());
            InitState(SubscribeStateLibrary["JumpLoop"]);
            SubscribeStateLibrary.Add("JumpEnd", new ChuChuJumpEnd());
            InitState(SubscribeStateLibrary["JumpEnd"]);
            SubscribeStateLibrary.Add("LeftGetHit", new ChuChuLeftGetHit());
            InitState(SubscribeStateLibrary["LeftGetHit"]);
            SubscribeStateLibrary.Add("FrontGetHit", new ChuChuFrontGetHit());
            InitState(SubscribeStateLibrary["FrontGetHit"]);
            SubscribeStateLibrary.Add("RightGetHit", new ChuChuRightGetHit());
            InitState(SubscribeStateLibrary["RightGetHit"]);
            SubscribeStateLibrary.Add("Dead", new ChuChuDead());
            InitState(SubscribeStateLibrary["Dead"]);
            SubscribeStateLibrary.Add("Down", new ChuChuDown());
            InitState(SubscribeStateLibrary["Down"]);
            SubscribeStateLibrary.Add("Up", new ChuChuUp());
            InitState(SubscribeStateLibrary["Up"]);
            SubscribeStateLibrary.Add("Dash", new ChuChuDash());
            InitState(SubscribeStateLibrary["Dash"]);
            SubscribeStateLibrary.Add("R1", new ChuChuR1());
            InitState(SubscribeStateLibrary["R1"]);
            SubscribeStateLibrary.Add("R1R1", new ChuChuR1R1());
            InitState(SubscribeStateLibrary["R1R1"]);
            SubscribeStateLibrary.Add("R1R1R1", new ChuChuR1R1R1());
            InitState(SubscribeStateLibrary["R1R1R1"]);
            SubscribeStateLibrary.Add("R1R1R1R1", new ChuChuR1R1R1R1());
            InitState(SubscribeStateLibrary["R1R1R1R1"]);
            SubscribeStateLibrary.Add("R1R1R1R1R1", new ChuChuR1R1R1R1R1());
            InitState(SubscribeStateLibrary["R1R1R1R1R1"]);
            SubscribeStateLibrary.Add("R1R2", new ChuChuR1R2());
            InitState(SubscribeStateLibrary["R1R2"]);
            SubscribeStateLibrary.Add("R1R1R2", new ChuChuR1R1R2());
            InitState(SubscribeStateLibrary["R1R1R2"]);
            SubscribeStateLibrary.Add("R2R2R2R2", new ChuChuR2R2R2R2());
            InitState(SubscribeStateLibrary["R2R2R2R2"]);
            SubscribeStateLibrary.Add("R2", new ChuChuR2());
            InitState(SubscribeStateLibrary["R2"]);
            SubscribeStateLibrary.Add("R2R2", new ChuChuR2R2());
            InitState(SubscribeStateLibrary["R2R2"]);
            SubscribeStateLibrary.Add("R2R2R2", new ChuChuR2R2R2());
            InitState(SubscribeStateLibrary["R2R2R2"]);
        }
    
#region AnimationEvents
    void CloseRecord()
    {
        player.bPreEnter = false;
        BAllowTransit = true;
    }
    void StartRecord()
    {
        BAllowTransit = false;
        player.bPreEnter = true;
    }
    void SwitchOnAtkBox(int boxIndex)
    {
        player.EnableAttackBox(boxIndex);
    }
    void SwitchOffAtkBox(int boxIndex)
    {
        player.DisableAttackBox(boxIndex);
    }

    #endregion

    public Animator AnimPlayer { get; set; }
    public Dictionary<string, StateSystem> SubscribeStateLibrary { get; set; }
    public bool BAllowTransit { get; set; }
    public string SCurrentState { get { return sState; } set { sState = value; } }
    public string SNextState { get; set; }
    AnimatorStateInfo animatorInfo;
    Player player;
    InputMotionController inputMotionController;
    bool playerHit, playerFreezed_Middle, playerFreezed_Left, playerFreezed_Right, playerKOed, playerDied;
    [SerializeField] string sState;//暫時Show在Inspector用的
}

