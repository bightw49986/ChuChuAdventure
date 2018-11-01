using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFSMGenerater : MonoBehaviour, FSMGenerater
{
    protected internal Animator AnimPlayer;//動畫播放機  
    protected internal Dictionary<string, StateSystem> SubscribeStateLibrary = new Dictionary<string, StateSystem>();//狀態機dictionary
    protected internal bool bAllowTransit;//允許狀態轉換與否
    protected internal string sCurrentState;//現在的state(Key)
    protected internal string sNextState;//準備上場的state(Key)，當轉換結束後改回null  

    public virtual void Start()
    {
        //取得Animator
        AnimPlayer = GetComponent<Animator>();
        //開始新增StateLibrary中的狀態
        AddState();
        sCurrentState = "Idle";
        bAllowTransit = true;
    }
    public virtual void Update()
    {
        AnyState();
        if (bAllowTransit && sNextState != null) StartCoroutine(Transit(sNextState));
        DoCurrentState(SubscribeStateLibrary[sCurrentState]);//擋的事交給coroutine的bools切換
    }

    public virtual IEnumerator Transit(string _state)
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
        }
        else Debug.Log("An accidental state be called !");//防呆！如果沒有註冊該狀態跳Log
        yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == false);//(Animator)等，直到動作切換結束
        EnterCurrentState(SubscribeStateLibrary[sCurrentState]);//轉換結束後呼叫Enter該狀態改變規範
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
