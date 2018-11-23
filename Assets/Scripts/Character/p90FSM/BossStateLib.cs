using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//只有Idle.Walk等會一直loop的動作才需要用動畫事件呼叫AllowTransit
public class BossIdle : StateSystem
{
    protected internal override void AllowTransit()
    {
        //bossStats.CheckTargetDistance();
        ////Debug.Log("StandBy " + bossStats.Attack1StandBy + " Attack1 " + bossStats.CanUseAttack1);
        ////Debug.Log("StandBy " + bossStats.Attack2StandBy + " Attack2 " + bossStats.CanUseAttack2);
        ////Debug.Log("StandBy " + bossStats.Attack3StandBy + " Attack3 " + bossStats.CanUseAttack3);

        ////Watch
        //if (bossStats.BCanTransform) FSM.SNextState = "Watch";
        ////Attack1.2.3 use switch case
        ////in attack range 分別偵測, use if/else set the priority
        ////如果優先度三隻不一樣就調順序
        //else if (bossStats.CanUseAttack1 && bossStats.Attack1StandBy) FSM.SNextState = "Attack1";
        //else if (bossStats.CanUseAttack2 && bossStats.Attack2StandBy) FSM.SNextState = "Attack2";
        //else if (bossStats.CanUseAttack3 && bossStats.Attack3StandBy) FSM.SNextState = "Attack3";
        //else if (bossStats.Dot > 0 && bossStats.m_fDistanceToPlayer > bossStats.m_DistanceLimit) FSM.SNextState = "Jump";
        //else FSM.SNextState = "Walk";
        ////else if (bossStats.Dot < 0 && bossStats.m_fDistanceToPlayer < bossStats.m_DistanceLimit) FSM.SNextState = "Walk";
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
        if (Input.GetKeyDown(KeyCode.F1)) FSM.SNextState = "Watch";
        if (Input.GetKeyDown(KeyCode.F2)) FSM.SNextState = "Attack1";
        if (Input.GetKeyDown(KeyCode.F3)) FSM.SNextState = "Attack2";
        if (Input.GetKeyDown(KeyCode.F4)) FSM.SNextState = "Attack3";
        if (Input.GetKeyDown(KeyCode.F5)) FSM.SNextState = "Jump";

        //除了GetHit，其他動作都要有allow transit動畫事件
    }
}
public class BossWalk : StateSystem
{
    protected internal override void AllowTransit()
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
        bossStats.Rotate();
        bossStats.CheckTargetDistance();
    }
    protected internal override void Check()
    {
        if(bossStats.Dot > 0.9) FSM.SNextState = "Idle";
    }
}
public class BossJump : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        bossStats.InitJumpData();
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {
        Debug.Log(bossStats.isJumping);
        if (bossStats.isJumping) bossStats.Jump();
    }
    protected internal override void Check()
    {

    }
}
public class BossAttack1 : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        bossStats.StartRunningCD(FSM.SCurrentState);       
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
public class BossAttack2 : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        bossStats.StartRunningCD(FSM.SCurrentState);
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
public class BossAttack3 : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        bossStats.StartRunningCD(FSM.SCurrentState);
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
public class BossTransform : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        
    }
    protected internal override void Leave()
    {
        bossStats.SwitchBoss();
    }
    protected internal override void Do()
    {
        //變身特效(現身)
    }
    protected internal override void Check()
    {

    }
}
public class BossGetHit1 : StateSystem
{
    protected internal override void AllowTransit()
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
public class BossGetHit2 : StateSystem
{
    protected internal override void AllowTransit()
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
public class BossDeath : StateSystem
{
    protected internal override void AllowTransit()
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
    protected internal override void AllowTransit()
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
        //變身特效(醞釀)
    }
    protected internal override void Check()
    {
        //Transform
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState) && FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Transform";
        }
    }
}

