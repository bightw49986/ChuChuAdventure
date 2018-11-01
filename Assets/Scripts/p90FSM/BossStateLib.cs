using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public abstract class StateSystem //所有狀態都來繼承它！ //PlayerFSMGenerater.sRecordInput = null改用NextState就好？
//{
//    //protected internal PlayerFSMGenerater playerFSMGenerater = new PlayerFSMGenerater();
//    //protected internal Player m_player = new Player();
//    protected internal abstract void Transit();
//    //從其他狀態轉換到這個狀態的期間的規範
//    protected internal abstract void Do();
//    //這裡寫執行狀態時該偵測的function或是改變的數值
//    protected internal abstract void Check();
//    //這裡偵測是否收到有效輸入按鍵或由被動觸發，而改變state
//    protected internal abstract void Enter();
//    //剛進來時定義好這個動作允不允許移動or攻擊or無敵狀態or跳躍(這部分單純看該動作給不給跳，跟ground check無關)
//    protected internal abstract void Leave();
//    //離開的時候要做的，通常是清空Do當中計算中的數值
//}
public class BossIdle : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //if target go away but not too far:walk(chase)
        //if target go away :jump(Chase)
        //times up for transformation(coroutine)
        //target in front & can attack :attack3(back to Idle)
        //target within a small range & can attack :attack2
        //target on the right side & can attack:sttack1
        //(anystate)be attacked & toughness<0.5 :random GetHit1or2
        //(antstate)be killed : death

        //Chase(Walk)
        //Attack1.2.3
        //Jump
        //Watch
    }
}
public class BossWalk : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossJump : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossAttack1 : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossAttack2 : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossAttack3 : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossTransform : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {
        
    }
    IEnumerator Transforming()
    {
        
        yield return new WaitForSeconds(BossStats.fIceOrFireTime);
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossGetHit1 : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossGetHit2 : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Idle
    }
}
public class BossDeath : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {

    }
}
public class BossWatch : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        //Transform
    }
}

