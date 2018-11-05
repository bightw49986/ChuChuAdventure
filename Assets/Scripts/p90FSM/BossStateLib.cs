using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //targetinsight but not in all 3 attacks' range
        //also not too far way
        if (true)
        {
            FSM.SNextState = "Walk";
        }
        //Attack1.2.3
        //in attack range 分別偵測, use if/else set the priority
        //如果優先度三隻不一樣就調順序
        if (true)
        {
            FSM.SNextState = "Attack1";
            FSM.SNextState = "Attack2";
            FSM.SNextState = "Attack3";
        }
        //Jump
        //If too far away then jump for a fixed distance toward/backward
        if (true)
        {
            FSM.SNextState = "Jump";
        }
        //Watch
        if (bossStats.BCanTransform)
        {
            FSM.SNextState = "Watch";
        }
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
        if(FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
    }
}
public class BossTransform : StateSystem
{
    protected internal override void Transit()
    {
        bossStats.SwitchBoss();
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Transform";
        }
    }
}

