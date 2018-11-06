using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuChuIdleToRun : StateSystem
{
    float fMoveDamp = 0;
    //時間由其他狀態回Idle各自給定
    protected internal override void Transit()
    {
        //player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = false;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        //離開Transit會改的bools再到這改
        player.bCanMove = true;
        player.bCanAttack = true;
        player.bCanJump = true;
        fMoveDamp = 0;
    }
    protected internal override void Leave()
    {
        player.bCanMove = false;
    }
    protected internal override void Do()
    {//這樣到底有沒有init一份啊？
        if(player.bCanMove == false)
        {
            FSM.AnimPlayer.SetFloat("Move", 0);
        }
        else
        {
            if (inputMotionController.m_fMoveInput > inputMotionController.m_fDeadZone)
            {
                fMoveDamp = Mathf.Lerp(fMoveDamp, inputMotionController.m_fMoveSpeed, 0.1f);
            }
           
            else { fMoveDamp = Mathf.Lerp(fMoveDamp, 0, 0.2f); }
            
            FSM.AnimPlayer.SetFloat("Move", fMoveDamp);
        }
        
    }
    protected internal override void Check()
    {
        if (inputMotionController.m_fJInput == 1)
        {
            FSM.SNextState = "JumpStart";
        }
        else if (inputMotionController.m_fLAInput == 1)
        {
            FSM.SNextState = "R1";
        }
        else if (inputMotionController.m_fHAInput == 1)
        {
            FSM.SNextState = "R2";
        }
    }
}
public class ChuChuJumpStart : StateSystem
{
    protected internal override void Transit()
    {
        player.bCanMove = true;
        player.bCanAttack = false;
        player.bHarmless = true;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {
        player.bCanAttack = true;
        player.bHarmless = false;//考慮給一下
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (inputMotionController.m_fLAInput == 1)
        {
            FSM.SNextState = "R1";
        }
        else if (inputMotionController.m_fHAInput == 1)
        {
            FSM.SNextState = "R2";
        }
        else if (FSM.SNextState == null &&
            FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f)
        {
            FSM.SNextState = "JumpLoop";
        }
    }
}
public class ChuChuJumpLoop : StateSystem
{
    protected internal override void Transit()
    {
        player.bCanMove = true;
        player.bCanAttack = true;
        player.bHarmless = false;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
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
        if (inputMotionController.m_fLAInput == 1)
        {
            FSM.SNextState = "R1";
        }
        else if (inputMotionController.m_fHAInput == 1)
        {
            FSM.SNextState = "R2";
        }
        else if (FSM.SNextState == null &&
            inputMotionController.m_bJumpEnd == true)
        {
            FSM.SNextState = "JumpEnd";
        }
    }
}
public class ChuChuJumpEnd : StateSystem
{
    protected internal override void Transit()
    {
        player.bCanMove = false;
        player.bCanAttack = true;
        player.bHarmless = false;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Enter()
    {

    }
    protected internal override void Leave()
    {
        player.bCanMove = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
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
        player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = false;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("LeftGetHit") &&
            FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSM.SNextState = "Idle";
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
        player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = false;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("FrontGetHit") &&
            FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSM.SNextState = "Idle";
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
        player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = false;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("RightGetHit") &&
            FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSM.SNextState = "Idle";
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
        player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = true;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
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
        player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = true;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Down") &&
           FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSM.SNextState = "Up";
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
        player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = true;
        player.bCanJump = false;
        player.bFuckTheGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Up") &&
         FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            FSM.SNextState = "Idle";
        }
    }
}
public class ChuChuDash : StateSystem
{
    protected internal override void Transit()
    {
        player.bCanMove = false;
        player.bCanAttack = false;
        player.bHarmless = true;
        player.bCanJump = false;
        player.bFuckTheGravity = true;
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
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Dash") &&
         FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            if (inputMotionController.m_bJumpEnd == true)
            {
                FSM.SNextState = "Idle";
            }
            else
            {
                FSM.SNextState = "JumpEnd";
            }

        }
    }
}
public class ChuChuR1 : StateSystem
{

    protected internal override void Transit()
    {
        //player.bCanMove = false;
        //player.bCanAttack = true;
        //player.bHarmless = false;
        //player.bCanJump = false;
        //player.bFuckTheGravity = true;
    }
    protected internal override void Enter()
    {
        player.bCanMove = false;
        player.bCanAttack = true;
        player.bHarmless = false;
        player.bCanJump = false;
        player.bFuckTheGravity = true;
    }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1)
                {
                    FSM.SNextState = "R1R1";
                }
                else if (inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R1R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }
        
    }
}
    public class ChuChuR1R1 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = true;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1)
                {
                    FSM.SNextState = "R1R1R1";
                }
                else if (inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R1R1R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR1R1R1 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = true;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1)
                {
                    FSM.SNextState = "R1R1R1R1";
                }
                else if (inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R1R1R1R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR1R1R1R1 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = true;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1 || inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R1R1R1R1R1";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR1R1R1R1R1 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = true;
        }
        protected internal override void Enter()
        {

        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
               if (inputMotionController.m_fMoveInput > 0.7f)
               {
                    FSM.SNextState = "Idle";
               }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR1R2 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = false;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1 || inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R1R1R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR1R1R2 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = false;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1 || inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R2R2R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR2R2R2R2 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = false;
        }
        protected internal override void Enter()
        {

        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR2 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = false;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1 || inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR2R2 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = false;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1 || inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R2R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}
    public class ChuChuR2R2R2 : StateSystem
    {
        protected internal override void Transit()
        {
            player.bCanMove = false;
            player.bCanAttack = false;
            player.bHarmless = false;
            player.bCanJump = false;
            player.bFuckTheGravity = false;
        }
        protected internal override void Enter()
        {
            player.bCanAttack = true;
        }
    protected internal override void Leave()
    {
        player.bPreEnter = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
            {
                if (inputMotionController.m_fLAInput == 1 || inputMotionController.m_fHAInput == 1)
                {
                    FSM.SNextState = "R2R2R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}

