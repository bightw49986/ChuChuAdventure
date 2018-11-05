using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFSMGenerater : MonoBehaviour, FSMGenerater
{
    public Animator AnimPlayer{ get; set; }
    public Dictionary<string, StateSystem> SubscribeStateLibrary { get ; set ; }
    public bool BAllowTransit { get ; set; }
    public string SCurrentState { get; set ; }
    public string SNextState { get ; set ; }

    public virtual void InitState(StateSystem state)
    {
        state.FSM = this;
    }

    public virtual void Awake()
    {
        //取得Animator
        AnimPlayer = GetComponent<Animator>();
        SubscribeStateLibrary = new Dictionary<string, StateSystem>();//狀態機dictionary
    }
    public virtual void Start()
    {
        if (AnimPlayer == null) Debug.LogError("Doesn't attach boss' animator!!");
        //開始新增StateLibrary中的狀態
        AddState();
        SCurrentState = "Idle";
        BAllowTransit = true;
    }
    public virtual void Update()
    {
        AnyState();
        if (BAllowTransit && SNextState != null) StartCoroutine(Transit(SNextState));
        DoCurrentState(SubscribeStateLibrary[SCurrentState]);//擋的事交給coroutine的bools切換
    }

    public virtual IEnumerator Transit(string _state)
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
            yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == false);//(Animator)等，直到動作切換結束
            EnterCurrentState(SubscribeStateLibrary[SCurrentState]);//轉換結束後呼叫Enter該狀態改變規範
            BAllowTransit = true;
        }
        else Debug.Log("An accidental state be called !");//防呆！如果沒有註冊該狀態跳Log      
    }
    public virtual void DoCurrentState(StateSystem state)
    {
        state.Check();
        state.Do();
    }
    public virtual void LeaveCurrentState(StateSystem state)
    {
        state.Leave();
    }
    public virtual void EnterCurrentState(StateSystem state)
    {
        state.Enter();
    }
    public virtual void TransitCurrentState(StateSystem state)
    {
        state.Transit();
    }
    public virtual bool AnyState()
    {
        return false;
    }
    public virtual void AddState()
    {

    }
}
