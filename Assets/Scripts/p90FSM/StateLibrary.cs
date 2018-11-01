using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public abstract class StateSystem //所有狀態都來繼承它！ //PlayerFSMGenerater.sRecordInput = null改用NextState就好？
{
    public FSMGenerater FSM;
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

public class ChuChuIdleToRun : StateSystem
{
    //ChuChuIdleToRun(FSMGenerater fsm) : base(fsm)
    //{
        
    //}
    //時間由其他狀態回Idle各自給定
    protected internal override void Transit()
    {
        Player.fBlockTime = 0.3f;
        Player.fRecordTime = 0f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        //離開Transit會改的bools再到這改
        Player.bCanMove = true;
        Player.bCanAttack = true;
        Player.bCanJump = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {//這樣到底有沒有init一份啊？
        PlayerFSMGenerater.AnimPlayer.SetFloat("Move", InputMotionController.m_fMoveSpeed);
    }
    protected internal override void Check()
    {
        if (InputMotionController.m_fJInput ==1 )
        {
            PlayerFSMGenerater.sNextState = "JumpStart";
        }
        else if (InputMotionController.m_fLAInput == 1)
        {
            PlayerFSMGenerater.sNextState = "R1";
        }
        else if (InputMotionController.m_fHAInput == 1)
        {
            PlayerFSMGenerater.sNextState = "R2";
        }
    }
}
public class ChuChuJumpStart : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 0;
        Player.fRecordTime = 0.1f;
        Player.bCanMove = true;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
        Player.bHarmless = false;//考慮給一下
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {
        
    }
    protected internal override void Check()
    {
        if (InputMotionController.m_fLAInput == 1)
        {
            PlayerFSMGenerater.sNextState = "R1";
        }
        else if (InputMotionController.m_fHAInput == 1)
        {
            PlayerFSMGenerater.sNextState = "R2";
        }
        else if (PlayerFSMGenerater.sNextState == null &&
            PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f)
        {
            PlayerFSMGenerater.sNextState = "JumpLoop";
        }
    }
}
public class ChuChuJumpLoop : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 0;
        Player.fRecordTime = 0f;
        Player.bCanMove = true;
        Player.bCanAttack = true;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
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
        if (InputMotionController.m_fLAInput == 1)
        {
            PlayerFSMGenerater.sNextState = "R1";
        }
        else if (InputMotionController.m_fHAInput == 1)
        {
            PlayerFSMGenerater.sNextState = "R2";
        }
        else if (PlayerFSMGenerater.sNextState == null &&
            InputMotionController.m_bJumpEnd == true)
        {
            PlayerFSMGenerater.sNextState = "JumpEnd";
        }
    }
}
public class ChuChuJumpEnd : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 0;
        Player.fRecordTime = 0.1f;
        Player.bCanMove = false;
        Player.bCanAttack = true;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
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
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuLeftGetHit : StateSystem
{
    protected internal override void Transit()
    {
       
    }
    protected internal override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("LeftGetHit") &&
            PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuFrontGetHit : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("FrontGetHit") &&
            PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuRightGetHit : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("RightGetHit") &&
            PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuDead : StateSystem//若玩家處於無敵狀態，則npc不會進行攻擊
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
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
public class ChuChuDown : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Down") &&
           PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            PlayerFSMGenerater.sNextState = "Up";
        }
    }
}
public class ChuChuUp : StateSystem
{
    protected internal override void Transit()
    {

    }
    protected internal override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Up") &&
         PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuDash : StateSystem
{
    protected internal override void Transit()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
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
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Dash") &&
         PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            if (InputMotionController.bGrounded)
            {
                PlayerFSMGenerater.sNextState = "Idle";
            }
            else
            {
                PlayerFSMGenerater.sNextState = "JumpEnd";
            }
            
        }
    }
}
public class ChuChuR1 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 0.3f;
        Player.fRecordTime = 0.5f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R1";
            }
            else if (InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R2";
            }
        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }             
    }
}
public class ChuChuR1R1 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {
  
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R1R1";
            }
            else if (InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R1R2";
            }
        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R1 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R1R1R1";
            }
            else if (InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R1R1R2";
            }
        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R1R1 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1|| InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R1R1R1R1";
            }
            
        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R1R1R1 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
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
        
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("R1R1R1R1R1") &&
            PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R2 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R1R1R2";
            }

        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R2 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R2R2R2R2";
            }

        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR2R2R2R2 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
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
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR2 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 0.3f;
        Player.fRecordTime = 0.5f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R2R2";
            }

        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR2R2 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R2R2R2";
            }

        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}
public class ChuChuR2R2R2 : StateSystem
{
    protected internal override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        Player.bCanAttack = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                PlayerFSMGenerater.sNextState = "R2R2R2R2";
            }

        }
        if (PlayerFSMGenerater.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            PlayerFSMGenerater.sNextState = "Idle";
        }
    }
}

