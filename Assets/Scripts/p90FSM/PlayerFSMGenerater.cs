using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFSMGenerater : MonoBehaviour, FSMGenerater {
    
    public static Animator AnimPlayer;//動畫播放機  
    private Dictionary<string, StateSystem> SubscribeStateLibrary = new Dictionary<string, StateSystem>();//狀態機dictionary
    private bool bAllowTransit;//允許狀態轉換與否
    private string sCurrentState;//現在的state(Key)
    [SerializeField]private string sState;//暫時Show在Inspector用的
    public static string sNextState;//準備上場的state(Key)，當轉換結束後改回null  

    void Start()
    {
        //取得Animator
        AnimPlayer = GetComponent<Animator>();
        //開始新增StateLibrary中的狀態
        AddState();
        sCurrentState = "Idle";
        bAllowTransit = true;
    }
    void Update()
    {
        sState = sCurrentState;
        AnyState();
        if (bAllowTransit && sNextState != null) StartCoroutine(Transit(sNextState));
        DoCurrentState(SubscribeStateLibrary[sCurrentState]);//擋的事交給coroutine的bools切換
    }

    public IEnumerator Transit(string _state)
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
    //protected override IEnumerator f(Vector3 to)
    //{
    //  var baseVal = base.f(to);
    //    while (true)
    //    {
    //        yield return baseVal.Current;
    //        if (baseVal.MoveNext() == False) break;
    //    }
    //}
    /// <summary>
    /// 每個動作的前多少時間阻斷「所有」輸入，之所以要與Transit分開主要因為攻擊動作的預輸入;一段可記錄預輸入的時間
    /// </summary>
    /// <returns></returns>
    public IEnumerator InputController()
    {
        Player.bBlockInput = true;
        yield return new WaitForSeconds(Player.fBlockTime);//等待當個狀態的阻斷輸入時間
        Player.bBlockInput = false;
        Player.bPreEnter = true;
        yield return new WaitForSeconds(Player.fRecordTime);//等待當個狀態的預輸入紀錄時間
        Player.bPreEnter = false;
        bAllowTransit = true;
    }
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
        //else if (sCurrentState=="JumpLoop" && InputMotionController.bGrounded == false && InputMotionController.m_bJumpEnd == true)
        //{
        //    sNextState = "JumpEnd";
        //    return true;
        //}
        else return false;
    }
    public void AddState()
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
}
