using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public abstract class StateSystem //所有狀態都來繼承它！ //FSM.SRecordInput = null改用NextState就好？
{
    public FSMGenerater FSM;
    protected internal InputMotionController inputMotionController ;
    protected internal Player player;
    protected internal BossStats bossStats;
    //protected internal BossFSMGenerater bossFSMGenerater = new BossFSMGenerater();
    //public StateSystem(FSMGenerater fsm)
    //{
    //    FSM = fsm;
    //}
    protected internal abstract void Transit();
    //從其他狀態轉換到這個狀態的期間的規範
    protected internal abstract void Do();
    //這裡寫執行狀態時該偵測的function或是改變的數值
    protected internal abstract void Check();
    //這裡偵測是否收到有效輸入按鍵或由被動觸發，而改變state
    protected internal abstract void Enter();
    //剛進來時定義好這個動作允不允許移動or攻擊or無敵狀態or跳躍(這部分單純看該動作給不給跳，跟ground check無關)
    protected internal abstract void Leave();
    //離開的時候要做的，通常是清空Do當中計算中的數值
}



