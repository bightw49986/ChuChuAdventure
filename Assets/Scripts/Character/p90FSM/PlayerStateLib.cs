using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//input的部分怎麼擋?
public class ChuChuIdleToRun : StateSystem
{
    float fMoveDamp = 0;
    //時間由其他狀態回Idle各自給定
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = true;
        player.bIgnoreGravity = false;
        fMoveDamp = 0;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {
        if (player.bInputMove == false)
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
        else if (Input.GetMouseButtonDown(0))
        {
            FSM.SNextState = "R1";
        }
        else if (Input.GetMouseButtonDown(1))
        {
            FSM.SNextState = "R2";
        }
    }
}
public class ChuChuJumpStart : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = true;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FSM.SNextState = "R1";
        }
        else if (Input.GetMouseButtonDown(1))
        {
            FSM.SNextState = "R2";
        }
        else if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            FSM.SNextState = "JumpLoop";
        }
    }
}
public class ChuChuJumpLoop : StateSystem
{
    protected internal override void AllowTransit()
    {
        
    }
    protected internal override void Enter()
    {
        player.bInputMove = true;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FSM.SNextState = "R1";
        }
        else if (Input.GetMouseButtonDown(1))
        {
            FSM.SNextState = "R2";
        }
        else if (inputMotionController.m_bJumpEnd == true)
        {
            FSM.SNextState = "JumpEnd";
        }
    }
}
public class ChuChuJumpEnd : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

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
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState) &&
            FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
    }
}
public class ChuChuFrontGetHit : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState) &&
            FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
    }
}
public class ChuChuRightGetHit : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState) &&
            FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
    }
}
public class ChuChuDead : StateSystem//若玩家處於無敵狀態，則npc不會進行攻擊
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
        player.IsHarmless = true;
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
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
        player.IsHarmless = true;
    }
    protected internal override void Leave()
    {

    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState) &&
           FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >=0.99f)
        {
            FSM.SNextState = "Up";
        }
    }
}
public class ChuChuUp : StateSystem
{
    protected internal override void AllowTransit()
    {

    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
        player.IsHarmless = true;
    }
    protected internal override void Leave()
    {
        player.IsHarmless = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState) &&
         FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            FSM.SNextState = "Idle";
        }
    }
}
//public class ChuChuDash : StateSystem
//{
//    protected internal override void AllowTransit()
//    {

//    }
//    protected internal override void Enter()
//    {
//        player.bInputMove = false;
//        player.bInputJump = false;
//        player.bInputDash = false;
//        player.bIgnoreGravity = false;
//        player.IsHarmless = true;
//    }
//    protected internal override void Leave()
//    {
//        player.IsHarmless = false;
//    }
//    protected internal override void Do()
//    {

//    }
//    protected internal override void Check()
//    {
//        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState) &&
//         FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
//        {
//            FSM.SNextState = "Idle";
//        }
//    }
//}
public class ChuChuR1 : StateSystem
{

    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
        
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = true;
    }
    protected internal override void Leave()
    {
        player.bIgnoreGravity = false;
    }
    protected internal override void Do()
    {

    }
    protected internal override void Check()
    {
        if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName(FSM.SCurrentState))
        {
            if (FSM.AnimPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    FSM.SNextState = "R1R1";
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R1R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
                }
            }
            else
            {
                if(inputMotionController.m_bJumpEnd)
                    FSM.SNextState = "Idle";
                else FSM.SNextState = "JumpLoop";
            }
        }

    }
}
public class ChuChuR1R1 : StateSystem
{
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
        
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = true;
    }
    protected internal override void Leave()
    {
        player.bIgnoreGravity = false;
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
                if (Input.GetMouseButtonDown(0))
                {
                    FSM.SNextState = "R1R1R1";
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R1R1R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
                }
            }
            else
            {
                if (inputMotionController.m_bJumpEnd)
                    FSM.SNextState = "Idle";
                else FSM.SNextState = "JumpLoop";
            }
        }

    }
}
public class ChuChuR1R1R1 : StateSystem
{
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
        
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = true;
    }
    protected internal override void Leave()
    {
        player.bIgnoreGravity = false;
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
                if (Input.GetMouseButtonDown(0))
                {
                    FSM.SNextState = "R1R1R1R1";
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R2R2R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
                }
            }
            else
            {
                if (inputMotionController.m_bJumpEnd)
                    FSM.SNextState = "Idle";
                else FSM.SNextState = "JumpLoop";
            }
        }

    }
}
public class ChuChuR1R1R1R1 : StateSystem
{
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
        
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = true;
    }
    protected internal override void Leave()
    {
        player.bIgnoreGravity = false;
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
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R1R1R1R1R1";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
                }
            }
            else
            {
                if (inputMotionController.m_bJumpEnd)
                    FSM.SNextState = "Idle";
                else FSM.SNextState = "JumpLoop";
            }
        }

    }
}
public class ChuChuR1R1R1R1R1 : StateSystem
{
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
        
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = true;
    }
    protected internal override void Leave()
    {
        player.bIgnoreGravity = false;
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
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
                }
            }
            else
            {
                if (inputMotionController.m_bJumpEnd)
                    FSM.SNextState = "Idle";
                else FSM.SNextState = "JumpLoop";
            }
        }

    }
}
public class ChuChuR1R2 : StateSystem
{
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

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
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R1R1R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
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
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

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
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R2R2R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
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
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

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
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
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
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

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
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
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
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

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
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R2R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
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
    protected internal override void AllowTransit()
    {
        player.bInputMove = true;
        player.bInputJump = true;
        player.bInputDash = true;
    }
    protected internal override void Enter()
    {
        player.bInputMove = false;
        player.bInputJump = false;
        player.bInputDash = false;
        player.bIgnoreGravity = false;
    }
    protected internal override void Leave()
    {

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
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    FSM.SNextState = "R2R2R2R2";
                }
                else if (inputMotionController.m_fMoveInput > 0.7f)
                {
                    FSM.SNextState = "Idle";
                }
                else if (inputMotionController.m_fJInput > 0.9f)
                {
                    FSM.SNextState = "JumpStart";
                }
            }
            else
            {
                FSM.SNextState = "Idle";
            }
        }

    }
}

