using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class Arianna : MonoBehaviour {
    public Animator m_Anim;
    public Player m_Player;
    bool m_bBlockInput = false; 
    //取得Components
    void Start()
    {
        if(m_Anim ==null || m_Player == null)
        {
            Debug.LogError("Components lost !");
        }
    }

    void Update () {
        Debug.Log(AnyState());
        Debug.Log(m_Player.m_currentState);
        //Check pre-enter
        AuthorizeInput(m_Player.m_currentState);
        //If Input not be blocked, then keep doing current state.

        //Check AnyState       
        if (AnyState() == false)
        {
            //Do
            Do(m_Player.m_currentState);
        }
    }

    bool AnyState()
    {
       
        if (m_Player.stats["HP"] <= 0)
        {
            WannaChangeState("Dead");
            return true;
        }
        else if(m_Player.stats["Toughness"] <= 0)
        {
            WannaChangeState("Down");
            return true;
        }
        else if (m_Player.stats["Toughness"] <= 6)
        {
            WannaChangeState("FrontBeHit");
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            WannaChangeState("Dash");
            return true;
        }
        else return false;
    }

    //判定此時能不能偵測輸入
    void AuthorizeInput(string _CurrentState)
    {
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("JumpStart") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1)
        {
            m_bBlockInput = true;
        }
        else
        {
            m_bBlockInput = false;
        }
    }

    //Switch the state
    void WannaChangeState(string _CurrentState)
    {
        if(m_bBlockInput == true)
        {
            return;
        }
        else
        {
            //Switch the state
            m_Anim.SetBool(m_Player.m_currentState, false);
            m_Player.m_currentState = _CurrentState;
            m_Anim.SetBool(_CurrentState, true);
            Debug.Log(m_Player.m_currentState);
            //State's true/false
        }
    }

    void Do(string _CurrentState)
    {
 
        //m_Anim.SetBool("Move", true);
        //主管：我說你可以走，你才可以走
        if (_CurrentState == "Idle")
        {
            if (Mathf.Abs(CrossPlatformInputManager.GetAxis("Vertical")) >= 0.1 ||
                Mathf.Abs(CrossPlatformInputManager.GetAxis("Horizontal")) >= 0.1)
            {
                WannaChangeState("Walk");
            }
            else if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                WannaChangeState("JumpStart");
            }
            
        }
        else if (_CurrentState == "Walk")
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)|| Mathf.Abs(CrossPlatformInputManager.GetAxis("Vertical")) >= 7 ||
                Mathf.Abs(CrossPlatformInputManager.GetAxis("Horizontal")) >= 7)
            {
                WannaChangeState("Run");
            }
            else if ((Mathf.Abs(CrossPlatformInputManager.GetAxis("Vertical")) < 0.1 &&
                Mathf.Abs(CrossPlatformInputManager.GetAxis("Horizontal")) < 0.1))
            {
                WannaChangeState("Idle");
            }
            else if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                WannaChangeState("JumpStart");
            }
        }
        else if (_CurrentState == "Run")
        {
            if ( Input.GetKey(KeyCode.LeftShift) == false)
            {
                WannaChangeState("Walk");
            }
            else if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                WannaChangeState("RunJump");
            }
            else if (CrossPlatformInputManager.GetButtonDown("Fire2"))
            {
                WannaChangeState("JumpAttack");
            }
        }
        else if (_CurrentState == "JumpStart")
        {

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("JumpStart") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1)
            {
                WannaChangeState("JumpEnd");
            }
            //On air
        }
        else if (_CurrentState == "JumpEnd")
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                if(Mathf.Abs(CrossPlatformInputManager.GetAxis("Vertical")) >= 0.1 ||
                   Mathf.Abs(CrossPlatformInputManager.GetAxis("Horizontal")) >= 0.1)
                {
                    WannaChangeState("Walk");
                }
                else
                {
                    WannaChangeState("Idle");
                }            
            }          
        }
        else if (_CurrentState == "RunJump")
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("RunJump") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                WannaChangeState("Run");
            }
            //On air
        }
        else if (_CurrentState == "JumpAttack")
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                WannaChangeState("Walk");
            }
        }
        else if (_CurrentState == "LeftGetHit")
        {

        }
        else if (_CurrentState == "FrontGetHit")
        {

        }
        else if (_CurrentState == "RightGetHit")
        {

        }
        else if (_CurrentState == "Dead")
        {

        }
        else if (_CurrentState == "Down")
        {

        }
        else if (_CurrentState == "Up")
        {

        }
        else if (_CurrentState == "Dash")
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >=0.9)
            {
                WannaChangeState("Idle");
            }
        }
        else if (_CurrentState == "R1")
        {

        }
        else if (_CurrentState == "R1R1")
        {

        }
        else if (_CurrentState == "R1R1R1")
        {

        }
        else if (_CurrentState == "R1R1R1R1")
        {

        }
        else if (_CurrentState == "R1R1R1R1R1")
        {

        }
        else if (_CurrentState == "R1R2")
        {

        }
        else if (_CurrentState == "R1R1R2")
        {

        }
        else if (_CurrentState == "R2R2R2R2")
        {

        }
        else if (_CurrentState == "R2")
        {

        }
        else if (_CurrentState == "R2R2")
        {

        }
        else if (_CurrentState == "R2R2R2")
        {

        }       
    }

   
}
