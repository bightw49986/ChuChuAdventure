using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem;
using ResourcesManagement;

public sealed class PlayerFSMGenerater : MonoBehaviour, FSMGenerater
{
    /// <summary>
    /// 狀態機system該有的ref參考init
    /// </summary>
    /// <param name="state"></param>
    void InitState(StateSystem state)
    {
        state.FSM = this;
        state.inputMotionController = inputMotionController;
        state.player = player;
    }
    /// <summary>
    /// 取得components並宣告容器
    /// </summary>
    void Awake()
    {
        //取得Animator
        attackRange = GetComponent<SphereCollider>();
        AnimPlayer = GetComponent<Animator>();
        player = GetComponent<Player>();
        heatDistortion = transform.Find("HeatDistortion").gameObject;
        spearEffect = transform.FindDeepChild("SpearEffect").gameObject;
        inputMotionController = GetComponent<InputMotionController>();
        SubscribeStateLibrary = new Dictionary<string, StateSystem>();//狀態機dictionary
    }
    /// <summary>
    /// 註冊Anystate事件、加入狀態、給部分資料初值
    /// </summary>
    void Start()
    {
        SubscribePlayerEvents();
        AddState();
        SCurrentState = "Idle";
        BAllowTransit = true;      
    }

    /// <summary>
    /// 訂閱死亡、擊倒、硬直、單純受擊的事件，若收到通知則trigger bools
    /// </summary>
    void SubscribePlayerEvents()
    {
        player.AttackSuccess += OnAttackSuccess;
        player.Died += OnPlayerDied;
        player.KOed += OnPlayerKOed;
        player.Freezed += OnPlayerFreezed;
        player.Hit += OnPlayerHit;//() => { playerHit = true; };
    }
    /// <summary>
    /// 被打之後掉血的邏輯
    /// </summary>
    /// <returns></returns>
    IEnumerator HPRecovery()
    {
        player.bHPRecoverable = true;
        yield return new WaitForSeconds(player.TimeBeforeLostHPRecovery);
        player.CurrentMaxHPRecoverable -= player.LostHPRecoverySpeed;
        yield return new WaitUntil(()=>player.CurrentMaxHPRecoverable <= player.Hp);
        player.CurrentMaxHPRecoverable = player.Hp;
        player.bHPRecoverable = false;
    }
    /// <summary>
    /// 成功擊中則觸發回血與戰鬥評價
    /// </summary>
    void OnAttackSuccess(DefendBox defendBox, AttackBox attackBox)
    {
        //打到敵人後按照攻擊傷害 % 數回血
        if (player.bHPRecoverable)
        {
            player.Hp += player.AtkValues[attackBox] * player.AttackRecoverHPRate;
            if (player.Hp > player.CurrentMaxHPRecoverable) player.Hp = player.CurrentMaxHPRecoverable;
            print("回血" + player.AtkValues[attackBox] * player.AttackRecoverHPRate + "滴");
        }

        //打到敵人後開始計算增加多少評價分數，UI由Player訂閱事件(?)
        m_fTempProcess = 0f;
        if (m_bBePunished)player.ComboRate = 0f;//懲罰期間不會加評價
        player.EscapeCombatTime = 5f;//重製脫戰時間
        StartCoroutine("EscapeCombat");//開算脫戰
        if (CombatStyle.ContainsKey(attackBox))//開關過後會是同一個吧?
        {
            m_fCombatStyleValue = CombatStyle[attackBox];
            m_fCombatStyleValue -= 0.33f;
            if (m_fCombatStyleValue < 1.0f) m_fCombatStyleValue = 1.0f;
        }
        else
        {
            CombatStyle.Add(attackBox, 2.0f);
            m_fCombatStyleValue = 2.0f;
        }
        m_fTempProcess = player.RankDifficulty * m_fCombatStyleValue * player.ComboRate;//這次攻擊增加的評分
        player.CombatProcess += m_fTempProcess;
        if (player.CombatProcess > 1)//超過1就升階
        {
            CombatStyle.Clear();//重製招式倍率
            player.CombatProcess -= 1;
            player.CombatRank += 1;
        }
        //print("增加評價" + m_fTempProcess);
        //print("目前評價" + player.CombatRank + "，進度條" + player.CombatProcess);

    }

    /// <summary>
    /// 死亡觸發的事件(停止掉血)
    /// </summary>
    void OnPlayerDied()
    {
        StopCoroutine("HPRecovery");
        playerHit = true;
    }

    /// <summary>
    /// 玩家被打倒的事件(停止加分、未能補血時開始可補血)
    /// </summary>
    void OnPlayerKOed()
    {
        StartCoroutine("KOPunishment");
        if (player.bHPRecoverable == false) StartCoroutine("HPRecovery");
        playerHit = true;
    }

    /// <summary>
    /// 玩家被打(但沒被打倒或死亡時？)發生的事件(可開始補血)
    /// </summary>
    void OnPlayerHit()
    {
        if (player.bHPRecoverable == false) StartCoroutine("HPRecovery");
        playerHit = true;
    }

    /// <summary>
    /// 玩家被打出硬直的事件(Combo被斷、可補血、判斷受擊盒位置做出不同方向的受擊動作)
    /// </summary>
    /// <param name="damagedPart"></param>
    void OnPlayerFreezed(DefendBox damagedPart)
    {
        player.ComboRate = 1.0f;
        if (player.bHPRecoverable == false) StartCoroutine("HPRecovery");
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

    

    void CheckBeforeAttack()
    {
        if (NearEnemyList.Count > 0)
        {
            //m_target = null;
            foreach (var target in NearEnemyList)//每次都有近來
            {
                float m_fDot = Vector3.Dot((target.transform.position - transform.position).normalized, transform.forward.normalized);
                if (m_fDot > fPlayerRotateLimit)
                {
                    fPlayerRotateLimit = m_fDot;
                    m_target = target;
                    player.enemyToPlayer = target.transform.position - transform.position;
                }
            }
            if(m_target!=null)
            {
                print("Rotate to " + fPlayerRotateLimit);
                player.enemyToPlayer = Vector3.ProjectOnPlane(player.enemyToPlayer, Vector3.up);
                inputMotionController.m_qTargetRotation = Quaternion.LookRotation(player.enemyToPlayer, Vector3.up);
                m_target = null;
            }         
        }
    }

    /// <summary>
    /// 狀態機每frame流程
    /// </summary>
    void Update()
    {
        //if (attackanyonesuccessfully)
        //{
        //    NewtonThirdLaw();
        //}

        sState = SCurrentState;
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

        if (Input.GetMouseButtonUp(2))
        {
            if (player.Power >= player.MaxPower)
            {
                print("釋放無雙！");
                StartCoroutine("Madness");
            }
            else print("無雙槽未充好，無法施放！");
        }
    }
    /// <summary>
    /// 被擊倒的懲罰
    /// </summary>
    /// <returns></returns>
    IEnumerator KOPunishment()
    {
        m_bBePunished = true;
        yield return new WaitForSeconds(player.BeKOedBlockTime);
        m_bBePunished = false;
    }
    /// <summary>
    /// 脫戰
    /// </summary>
    /// <returns></returns>
    IEnumerator EscapeCombat()
    {
        player.bInCombat = true;
        while (player.EscapeCombatTime <= 0)
        {
            yield return new WaitForSeconds(1);
            player.EscapeCombatTime--;
            player.ComboRate-=0.33f;
            if (player.ComboRate <= 1.0f) player.ComboRate = 1.0f;
        }
        player.bInCombat = false;
    }

    /// <summary>
    /// 使用無雙
    /// </summary>
    /// <returns></returns>
    IEnumerator Madness()//叫怪物加無雙值前都要確定bool
    {
        player.bCanIncreasePower = false;
        heatDistortion.SetActive(true);
        player.Power = 0f;//考慮可用lerp扣減，或是無雙時間內等速扣減。可跟回血掉落系統等等統一方式呈現
        player.AttackRecoverHPRate = 1.5f;
        AnimPlayer.SetFloat("MadnessSpeed", player.MadnessAttackSpeedRate);
        AnimPlayer.SetFloat("MadnessMove", player.MadnessMoveSpeedRate);
        inputMotionController.fFullSpeed *= player.MadnessMoveSpeedRate;
        player.CurrentMaxHPRecoverable = player.MaxHp;
        yield return new WaitForSeconds(player.MadnessTime);
        AnimPlayer.SetFloat("MadnessSpeed",1f);
        AnimPlayer.SetFloat("MadnessMove",1f);
        player.AttackRecoverHPRate = 1f;
        inputMotionController.fFullSpeed = 6f;
        heatDistortion.SetActive(false);
        player.CurrentMaxHPRecoverable = player.Hp;
        player.bCanIncreasePower = true;
    }

    /// <summary>
    /// 此禎重置後關掉trigger bools
    /// </summary>
    void LateUpdate()
    {
        //print(player.bAdjustTurn);
        playerHit = playerFreezed_Middle = playerFreezed_Left = playerFreezed_Right = playerKOed = playerDied = false;
    }
    /// <summary>
    /// 狀態轉換邏輯
    /// </summary>
    /// <param name="_state"></param>
    /// <returns></returns>
    public IEnumerator Transit(string _state)
    {
        if (SubscribeStateLibrary.ContainsKey(SNextState))//防呆！如果沒有註冊該狀態跳Log
            {
                BAllowTransit = false;//新的狀態即重新計算可轉換的時機
                LeaveCurrentState(SubscribeStateLibrary[SCurrentState]);//執行Leave
                SCurrentState = SNextState;//目前狀態->新的狀態
                AnimPlayer.SetTrigger(SCurrentState);//(Animator)把原本進這狀態的transition關上
                SNextState = null;//把排隊的位置清掉                
                //AnimPlayer.SetBool(SCurrentState, true);//(Animator)打開前往下一個狀態的transition
                EnterCurrentState(SubscribeStateLibrary[SCurrentState]);//開始轉換後呼叫Enter該狀態改變規範          
                yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == true);//(Animator)等，直到動作切換開始
                yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == false);//(Animator)等，直到動作切換結束                                                 
                BAllowTransit = animatorInfo.IsTag("notAttack") ? true : false;//如果此動作沒設有預輸入事件，狀態轉換結束後即可轉換到其他狀態
                if(BAllowTransit) AllowTransitCurrentState(SubscribeStateLibrary[SCurrentState]);//執行AllowTransit
        }
        else print("An accidental state be called !");//防呆！如果沒有註冊該狀態跳Log
    }
        

    /// <summary>
    /// 每禎都會跑的執行與偵測轉換function
    /// </summary>
    /// <param name="state"></param>
    public void DoCurrentState(StateSystem state)
    {
        state.Check();
        state.Do();
    }
    /// <summary>
    /// 離開上一狀態時呼叫
    /// </summary>
    /// <param name="state"></param>
    public void LeaveCurrentState(StateSystem state)
    {
        state.Leave();
    }
    /// <summary>
    /// 狀態Transition結束後時呼叫
    /// </summary>
    /// <param name="state"></param>
    public void EnterCurrentState(StateSystem state)
    {
        state.Enter();
    }
    /// <summary>
    /// 開放轉換時呼叫
    /// </summary>
    /// <param name="state"></param>
    public void AllowTransitCurrentState(StateSystem state)
    {
        state.AllowTransit();
    }

    /// <summary>
    /// 全域狀態偵測，若有切到狀態回傳true
    /// </summary>
    /// <returns></returns>
    /// <summary>
    ///Add string/statesystem 的dictionary，並且一一init
    /// </summary>
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

    //void AddNewtonEffect()
    //{
    //    NewtonPlayer.Add("R1R1",-0.2f);
    //    NewtonEnemy.Add("R1R1",0.5f);
    //    NewtonPlayer.Add("R1R1R1", 1.5f);
    //    NewtonEnemy.Add("R1R1R1",-2f);
    //    NewtonPlayer.Add("R1R1R1R1", 0f);
    //    NewtonEnemy.Add("R1R1R1R1", 0.5f);
    //    NewtonPlayer.Add("R1R1R1R1R1", -1f);
    //    NewtonEnemy.Add("R1R1R1R1R1", 2f);
    //    NewtonPlayer.Add("R1R2", -0.5f);
    //    NewtonEnemy.Add("R1R2", 0.5f);
    //    NewtonPlayer.Add("R1R1R2", 0f);
    //    NewtonEnemy.Add("R1R1R2", -1f);
    //    NewtonPlayer.Add("R2R2", 0f);
    //    NewtonEnemy.Add("R2R2", 0.5f);
    //    NewtonPlayer.Add("R2R2R2", -0.1f);
    //    NewtonEnemy.Add("R2R2R2", 0.2f);
    //    NewtonPlayer.Add("R2R2R2R2", 2f);
    //    NewtonEnemy.Add("R2R2R2R2", 3f);
    //}

    /// <summary>
    /// 動畫事件呼叫的方法
    /// </summary>
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
        //if (player.bInputDash && inputMotionController.m_fDInput >0)
        //{
        //    SNextState = "Dash";
        //    return true;
        //}
        if(inputMotionController.m_fJInput>0.9f && player.bInputJump)
        {
            SNextState = "JumpStart";
            return true;
        }
        return false;
    }

    

    #region AnimationEvents
    void StartRecord()
    {
        BAllowTransit = false;
        player.bPreEnter = true;
        player.bInputDash = false;
        WeaponTrail_distort.SetActive(true);
        WeaponTrail_long.SetActive(true);
        WeaponTrail_short.SetActive(true);
    }
    void CloseRecord()
    {
        //player.bAdjustTurn = false;
        BAllowTransit = true;
        player.bPreEnter = false;
        WeaponTrail_distort.SetActive(false);
        WeaponTrail_long.SetActive(false);
        WeaponTrail_short.SetActive(false);
        AllowTransitCurrentState(SubscribeStateLibrary[SCurrentState]);//執行AllowTransit
    }
    /// <summary>
    /// 
    /// </summary>
    void SwitchOnAdjustTurn()
    {
        CheckBeforeAttack();
    }

    /// <summary>
    /// 攻擊盒的Transform依照攻擊動作略有不同，順序乃依照Hierachy中之順序，由動畫事件傳入int編號產生
    /// </summary>
    /// <param name="boxIndex"></param>
    void SwitchOnAtkBox(int boxIndex)
    {        
        player.EnableAttackBox(boxIndex);
        if (boxIndex == 0) spearEffect.SetActive(true);
    }
    void SwitchOffAtkBox(int boxIndex)
    {
        player.DisableAttackBox(boxIndex);
        if (boxIndex == 0) spearEffect.SetActive(false);

    }

    #endregion



    //void NewtonThirdLaw()
    //{

    //        fPlayerMove = NewtonPlayer[SCurrentState];
    //    inputMotionController.m_velocity.y += fPlayerMove;
    //    fEnemyMove = NewtonEnemy[SCurrentState];
    //    inputMotionController.m_velocity.y += fPlayerMove;

    //}

    //public Dictionary<string, float> NewtonPlayer = new Dictionary<string, float>();
    //public Dictionary<string, float> NewtonEnemy = new Dictionary<string, float>();
    //public float fPlayerMove { get; private set; }
    //public float fEnemyMove { get; private set; }
    [SerializeField] GameObject WeaponTrail_distort;
    [SerializeField] GameObject WeaponTrail_long;
    [SerializeField] GameObject WeaponTrail_short;
    float fPlayerRotateLimit = 0f;
    GameObject m_target;
    Collider attackRange;
    public HashSet<GameObject> NearEnemyList = new HashSet<GameObject>();
    public Animator AnimPlayer { get; set; }
    public Dictionary<string, StateSystem> SubscribeStateLibrary { get; set; }
    public bool BAllowTransit { get; set; }
    public string SCurrentState { get { return sState; } set { sState = value; } }
    public string SNextState { get; set; }

    public ObjectPool ObjectPool
    {
        get
        {
            throw new System.NotImplementedException();
        }

        set
        {
            throw new System.NotImplementedException();
        }
    }

    AnimatorStateInfo animatorInfo;
    Player player;
    GameObject heatDistortion;
    GameObject spearEffect;
    InputMotionController inputMotionController;
    float m_fTempProcess;
    float m_fCombatStyleValue;
    Dictionary<AttackBox, float> CombatStyle = new Dictionary<AttackBox, float>();
    bool m_bBePunished = false;
    bool playerHit, playerFreezed_Middle, playerFreezed_Left, playerFreezed_Right, playerKOed, playerDied;
    [SerializeField] string sState;//暫時Show在Inspector用的
}

