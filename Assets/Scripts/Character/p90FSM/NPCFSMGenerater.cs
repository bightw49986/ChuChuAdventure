using ResourcesManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFSMGenerater : MonoBehaviour, FSMGenerater
{

    public virtual void InitState(StateSystem state)
    {
        state.FSM = this;
    }

    public virtual void Awake()
    {
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
        //取得Animator
        AnimPlayer = GetComponent<Animator>();
        SubscribeStateLibrary = new Dictionary<string, StateSystem>();//狀態機dictionary
        
    }
    public virtual void Start()
    {
        //objectPool = GameObject.FindGameObjectWithTag("Main").GetComponent<ObjectPool>();
        if (AnimPlayer == null) Debug.LogError("Doesn't attach boss' animator!!");

        //開始新增StateLibrary中的狀態
        AddState();
        SCurrentState = "Idle";
        BAllowTransit = true;


    }
    public virtual void Update()
    {
        AnyState();
        
        animatorInfo = AnimPlayer.GetCurrentAnimatorStateInfo(0);
        if (animatorInfo.IsName("Idle")) SCurrentState = "Idle";
        sState = SCurrentState;
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

    public virtual IEnumerator Transit(string _state)
    {
        if (SubscribeStateLibrary.ContainsKey(SNextState))//防呆！如果沒有註冊該狀態跳Log
        {
            BAllowTransit = false;//新的狀態即重新計算可轉換的時機
            LeaveCurrentState(SubscribeStateLibrary[SCurrentState]);//執行Leave
            SCurrentState = SNextState;//目前狀態->新的狀態
            AnimPlayer.SetTrigger(SCurrentState);//(Animator)把原本進這狀態的transition關上
            SNextState = null;//把排隊的位置清掉            
            EnterCurrentState(SubscribeStateLibrary[SCurrentState]);//轉換結束後呼叫Enter該狀態改變規範
            yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == true);//(Animator)等，直到動作切換結束
            yield return new WaitUntil(() => AnimPlayer.IsInTransition(0) == false);//(Animator)等，直到動作切換結束
            BAllowTransit = animatorInfo.IsTag("LoopState") ? false : true; //由動畫事件控制可以傳的時候吧
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
    public virtual void AllowTransitCurrentState(StateSystem state)
    {
        state.AllowTransit();
    }
    public virtual bool AnyState()
    {
        return false;
    }
    public virtual void AddState()
    {

    }
    public virtual void Jump()
    {

    }
    public virtual void AllowLoopMotionTransit()
    {
        BAllowTransit = true;
        AllowTransitCurrentState(SubscribeStateLibrary[SCurrentState]);
    }
    public Animator AnimPlayer { get; set; }
    public Dictionary<string, StateSystem> SubscribeStateLibrary { get; set; }
    public bool BAllowTransit { get; set; }
    public string SCurrentState { get; set; }
    public string SNextState { get; set; }

    public ObjectPool ObjectPool { get { return main.ObjectPool; }}


    //ObjectPool objectPool;
    AnimatorStateInfo animatorInfo;
    Main main;
    [SerializeField] private string sState;//暫時Show在Inspector用的
}
