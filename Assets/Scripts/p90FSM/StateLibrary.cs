using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public abstract class StateSystem//所有狀態都來繼承它！ //FSMGenerater_1022.sRecordInput = null改用NextState就好？
{
    public abstract void Transit();
    //從其他狀態轉換到這個狀態的期間的規範
    public abstract void Do();
    //這裡寫執行狀態時該偵測的function或是改變的數值
    public abstract void Check();
    //這裡偵測是否收到有效輸入按鍵或由被動觸發，而改變state
    public abstract void Enter();
    //剛進來時定義好這個動作允不允許移動or攻擊or無敵狀態or跳躍(這部分單純看該動作給不給跳，跟ground check無關)
    public abstract void Leave();
    //離開的時候要做的，通常是清空Do當中計算中的數值
}

public class ChuChuIdleToRun : StateSystem
{
    //時間由其他狀態回Idle各自給定
    public override void Transit()
    {
        Player.fBlockTime = 0f;
        Player.fRecordTime = 0.6f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {
        //離開Transit會改的bools再到這改
        Player.bCanMove = true;
        Player.bCanAttack = true;
        Player.bCanJump = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {
        FSMGenerater_1022.AnimPlayer.SetFloat("Move", InputMotionController.m_fMoveSpeed);
    }
    public override void Check()
    {
        if (InputMotionController.m_fJInput ==1 )
        {
            FSMGenerater_1022.sNextState = "JumpStart";
        }
        else if (InputMotionController.m_fLAInput == 1)
        {
            FSMGenerater_1022.sNextState = "R1";
        }
        else if (InputMotionController.m_fHAInput == 1)
        {
            FSMGenerater_1022.sNextState = "R2";
        }
    }
}
public class ChuChuJumpStart : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 0;
        Player.fRecordTime = 0.1f;
        Player.bCanMove = true;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
        Player.bHarmless = false;//考慮給一下
    }
    public override void Leave()
    {

    }
    public override void Do()
    {
        
    }
    public override void Check()
    {
        if (InputMotionController.m_fLAInput == 1)
        {
            FSMGenerater_1022.sNextState = "R1";
        }
        else if (InputMotionController.m_fHAInput == 1)
        {
            FSMGenerater_1022.sNextState = "R2";
        }
        else if (FSMGenerater_1022.sNextState == null &&
            FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f)
        {
            FSMGenerater_1022.sNextState = "JumpLoop";
        }
    }
}
public class ChuChuJumpLoop : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 0;
        Player.fRecordTime = 0f;
        Player.bCanMove = true;
        Player.bCanAttack = true;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {

    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (InputMotionController.m_fLAInput == 1)
        {
            FSMGenerater_1022.sNextState = "R1";
        }
        else if (InputMotionController.m_fHAInput == 1)
        {
            FSMGenerater_1022.sNextState = "R2";
        }
        //else if (FSMGenerater_1022.sNextState == null &&
        //    InputMotionController.bGrounded)
        //{
        //    FSMGenerater_1022.sNextState = "JumpEnd";
        //}
    }
}
public class ChuChuJumpEnd : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 0;
        Player.fRecordTime = 0.1f;
        Player.bCanMove = false;
        Player.bCanAttack = true;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {

    }
    public override void Leave()
    {
        
    }
    public override void Do()
    {

    }
    public override void Check()
    {
        //if (Player.m_fLAInput == 1)
        //{
        //    FSMGenerater_1022.sNextState = "R1";
        //}
        //else if (Player.m_fHAInput == 1)
        //{
        //    FSMGenerater_1022.sNextState = "R2";
        //}
        //else if (InputMotionController.bGrounded)
        //{
        //    FSMGenerater_1022.sNextState = "Idle";
        //}
    }
}
public class ChuChuLeftGetHit : StateSystem
{
    public override void Transit()
    {
       
    }
    public override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("LeftGetHit") &&
            FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuFrontGetHit : StateSystem
{
    public override void Transit()
    {

    }
    public override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("FrontGetHit") &&
            FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuRightGetHit : StateSystem
{
    public override void Transit()
    {

    }
    public override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("RightGetHit") &&
            FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuDead : StateSystem//若玩家處於無敵狀態，則npc不會進行攻擊
{
    public override void Transit()
    {

    }
    public override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {

    }
}
public class ChuChuDown : StateSystem
{
    public override void Transit()
    {

    }
    public override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Down") &&
           FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSMGenerater_1022.sNextState = "Up";
        }
    }
}
public class ChuChuUp : StateSystem
{
    public override void Transit()
    {

    }
    public override void Enter()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Up") &&
         FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuDash : StateSystem
{
    public override void Transit()
    {
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = true;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    public override void Enter()
    {

    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Dash") &&
         FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            if (InputMotionController.bGrounded)
            {
                FSMGenerater_1022.sNextState = "Idle";
            }
            else
            {
                FSMGenerater_1022.sNextState = "JumpEnd";
            }
            
        }
    }
}
public class ChuChuR1 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 0.3f;
        Player.fRecordTime = 0.5f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R1";
            }
            else if (InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R2";
            }
        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }             
    }
}
public class ChuChuR1R1 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {
  
    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R1R1";
            }
            else if (InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R1R2";
            }
        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R1 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R1R1R1";
            }
            else if (InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R1R1R2";
            }
        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R1R1 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1|| InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R1R1R1R1";
            }
            
        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R1R1R1 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = true;
    }
    public override void Enter()
    {

    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("R1R1R1R1R1") &&
            FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R2 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R1R1R2";
            }

        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR1R1R2 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R2R2R2R2";
            }

        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR2R2R2R2 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {

    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR2 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 0.3f;
        Player.fRecordTime = 0.5f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R2R2";
            }

        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR2R2 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R2R2R2";
            }

        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}
public class ChuChuR2R2R2 : StateSystem
{
    public override void Transit()
    {
        Player.fBlockTime = 1f;
        Player.fRecordTime = 2f;
        Player.bCanMove = false;
        Player.bCanAttack = false;
        Player.bHarmless = false;
        Player.bCanJump = false;
        Player.bFuckTheGravity = false;
    }
    public override void Enter()
    {
        Player.bCanAttack = true;
    }
    public override void Leave()
    {

    }
    public override void Do()
    {

    }
    public override void Check()
    {
        if (Player.bPreEnter)
        {
            if (InputMotionController.m_fLAInput == 1 || InputMotionController.m_fHAInput == 1)
            {
                FSMGenerater_1022.sNextState = "R2R2R2R2";
            }

        }
        if (FSMGenerater_1022.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            FSMGenerater_1022.sNextState = "Idle";
        }
    }
}

