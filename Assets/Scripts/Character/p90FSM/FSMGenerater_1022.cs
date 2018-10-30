using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Animator))]
public class FSMGenerater_1022 : MonoBehaviour {

	void Start () {
        //取得Animator
        AnimPlayer = GetComponent<Animator>();
        //開始新增StateLibrary中的狀態
        AddState();       
        sCurrentState = "Idle";
    }
	
    //取得輸入量->檢查AnyState->如有新狀態待切換，則開始Coroutine->執行現在(轉換後)狀態的Do+Check方法。
	void Update () {
        sState = sCurrentState;
        //Debug.LogError("bAllowTransit" + bAllowTransit);
        //Debug.LogError("Player.bFuckTheGravity" + Player.bFuckTheGravity);
        AnyState();
        if (bAllowTransit&& sNextState!=null) StartCoroutine(Transit(sNextState));
        DoCurrentState(SubscribeStateLibrary[sCurrentState]);//擋的事交給coroutine的bools切換
    }
    /// <summary>
    /// 轉換狀態
    /// </summary>
    /// <param name="_state"></param>
    /// <returns></returns>
    IEnumerator Transit(string _state)
    {
        if (SubscribeStateLibrary.ContainsKey(sNextState))//防呆！如果沒有註冊該狀態跳Log
        {
            AnimPlayer.SetBool(sCurrentState, false);//(Animator)把原本進這狀態的transition關上
            LeaveCurrentState(SubscribeStateLibrary[sCurrentState]);//執行Leave
            sCurrentState = sNextState;//目前狀態->新的狀態
            sNextState = null;//把排隊的位置清掉
            bAllowTransit = false;//新的狀態即重新計算可轉換的時機
            AnimPlayer.SetBool(sCurrentState, true);//(Animator)打開前往下一個狀態的transition
            TransitCurrentState(SubscribeStateLibrary[sCurrentState]);//執行Transit
            StartCoroutine(InputController());//從轉換開始當下已開始跑下一狀態的動作，因此剛轉換呼叫為準
        }
        else Debug.Log("An accidental state be called !");//防呆！如果沒有註冊該狀態跳Log
        yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == false);//(Animator)等，直到動作切換結束
        EnterCurrentState(SubscribeStateLibrary[sCurrentState]);//轉換結束後呼叫Enter該狀態改變規範
    }
    /// <summary>
    /// 每個動作的前多少時間阻斷「所有」輸入，之所以要與Transit分開主要因為攻擊動作的預輸入;一段可記錄預輸入的時間
    /// </summary>
    /// <returns></returns>
    IEnumerator InputController()
    {
        Player.bBlockInput = true;
        yield return new WaitForSeconds(Player.fBlockTime);//等待當個狀態的阻斷輸入時間
        Player.bBlockInput = false;
        Player.bPreEnter = true;
        yield return new WaitForSeconds(Player.fRecordTime);//等待當個狀態的預輸入紀錄時間
        Player.bPreEnter = false;
        bAllowTransit = true;
    }
    
    /// <summary>
    /// 每一frame都要做狀態該做的事&檢查切換狀態的條件
    /// </summary>
    /// <param name="state"></param>
    void DoCurrentState(StateSystem state)
    {
        state.Check();
        state.Do();
    }
    /// <summary>
    /// 狀態離開的時候該做的
    /// </summary>
    /// <param name="state"></param>
    void LeaveCurrentState(StateSystem state)
    {
        state.Leave();
    }
    /// <summary>
    /// 狀態轉換結束後進入時呼叫
    /// </summary>
    /// <param name="state"></param>
    void EnterCurrentState(StateSystem state)
    {
        state.Enter();
    }
    /// <summary>
    /// 狀態第一時間切換時呼叫
    /// </summary>
    /// <param name="state"></param>
    void TransitCurrentState(StateSystem state)
    {
        state.Transit();
    }

    /// <summary>
    /// 擊退、死亡等Anystate在這裡判斷，若有狀態觸發到就回傳True
    /// </summary>
    /// <returns></returns>
    bool AnyState()
    {
        if (Player.fHP <= 0)
        {
            sNextState = "Dead";
            return true;
        }
        else if (Player.fToughness <= 0)
        {
            sNextState = "Down";
            return true;
        }
        else if (Player.fToughness <= 6)//這裡得判斷哪個方向被打惹
        {
            
            return true;
        }
        else if (Player.bCanDash && InputMotionController.m_fDInput >= 0.1f)
        {
            sNextState = "Dash";
            return true;
        }
        
        else return false;
    }

   
    /// <summary>
    ///  把想要註冊的狀態一個個加進來～
    /// </summary>
    void AddState()
    {
        SubscribeStateLibrary.Add("Idle", new ChuChuIdleToRun());
        SubscribeStateLibrary.Add("JumpStart", new ChuChuJumpStart());
        SubscribeStateLibrary.Add("JumpLoop", new ChuChuJumpLoop());
        SubscribeStateLibrary.Add("JumpEnd", new ChuChuJumpEnd());
        SubscribeStateLibrary.Add("LeftGetHit", new ChuChuLeftGetHit());
        SubscribeStateLibrary.Add("FrontGetHit", new ChuChuFrontGetHit());
        SubscribeStateLibrary.Add("RightGetHit", new ChuChuRightGetHit());
        SubscribeStateLibrary.Add("Dead", new ChuChuDead());
        SubscribeStateLibrary.Add("Down", new ChuChuDown());
        SubscribeStateLibrary.Add("Up", new ChuChuUp());
        SubscribeStateLibrary.Add("Dash", new ChuChuDash());
        SubscribeStateLibrary.Add("R1", new ChuChuR1());
        SubscribeStateLibrary.Add("R1R1", new ChuChuR1R1());
        SubscribeStateLibrary.Add("R1R1R1", new ChuChuR1R1R1());
        SubscribeStateLibrary.Add("R1R1R1R1", new ChuChuR1R1R1R1());
        SubscribeStateLibrary.Add("R1R1R1R1R1", new ChuChuR1R1R1R1R1());
        SubscribeStateLibrary.Add("R1R2", new ChuChuR1R2());
        SubscribeStateLibrary.Add("R1R1R2", new ChuChuR1R1R2());
        SubscribeStateLibrary.Add("R2R2R2R2", new ChuChuR2R2R2R2());
        SubscribeStateLibrary.Add("R2", new ChuChuR2());
        SubscribeStateLibrary.Add("R2R2", new ChuChuR2R2());
        SubscribeStateLibrary.Add("R2R2R2", new ChuChuR2R2R2());
    }

    public static Animator AnimPlayer;//動畫播放機  
    Dictionary<string, StateSystem> SubscribeStateLibrary = new Dictionary<string, StateSystem>();//狀態機dictionary
    public static bool bAllowTransit = true;
    //public static string sRecordInput = null;//預輸入暫存區 //Maybe過時！
    public static string sCurrentState ;//現在的state(Key)
    [SerializeField]
    string sState;
    public static string sNextState;//準備上場的state(Key)，當轉換結束後改回null  
}
