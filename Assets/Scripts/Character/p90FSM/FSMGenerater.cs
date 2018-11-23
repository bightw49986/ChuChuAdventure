using ResourcesManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public interface FSMGenerater {
    Animator AnimPlayer { get; set; }//動畫播放機  
    ObjectPool ObjectPool { get;}//物件池
    Dictionary<string, StateSystem> SubscribeStateLibrary { get; set; }
    bool BAllowTransit { get; set; }//允許狀態轉換與否
    string SCurrentState { get; set; }//現在的state(Key)
    string SNextState { get; set; }//準備上場的state(Key)，當轉換結束後改回null  

    /// <summary>
    /// 轉換狀態
    /// </summary>
    /// <param name="_state"></param>
    /// <returns></returns>
    IEnumerator Transit(string _state);

    /// <summary>
    /// 每一frame都要做狀態該做的事&檢查切換狀態的條件
    /// </summary>
    /// <param name="state"></param>
    void DoCurrentState(StateSystem state);
    /// <summary>
    /// 狀態離開的時候該做的
    /// </summary>
    /// <param name="state"></param>
    void LeaveCurrentState(StateSystem state);
    /// <summary>
    /// 狀態轉換結束後進入時呼叫
    /// </summary>
    /// <param name="state"></param>
    void EnterCurrentState(StateSystem state);
    ///// <summary>
    ///// 可以切換時呼叫
    ///// </summary>
    ///// <param name="state"></param>
    void AllowTransitCurrentState(StateSystem state);

    /// <summary>
    /// 擊退、死亡等Anystate在這裡判斷，若有狀態觸發到就回傳True
    /// </summary>
    /// <returns></returns>
    bool AnyState();
    /// <summary>
    ///  把想要註冊的狀態一個個加進來～
    /// </summary>
    void AddState();
}
