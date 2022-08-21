using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController Controller) : base(Controller) {}

    public override void OnEnter()
    {
        base.OnEnter();
        
        //Force Grounded to be false since we are about to jump
        Grounded  = false;
        Velocity += new Vector2(0, m_Controller.m_JumpForce);

        CurrentSprite = m_Controller.m_Jump;
    }

    public override PlayerState OnUpdate()
    {
        //Check if we are grounded after short grace peroid
        if (m_TimeInState > 1 && Grounded)
            return m_Controller.m_GroundedState;

        Vector2 NewVelocity = Velocity;

        //Subtract gravity
        NewVelocity -= new Vector2(0, m_Controller.m_Gravity * Time.deltaTime);

        //Ensure X Velocity stays consistant whilst jumping
        NewVelocity.x = Speed;

        if (m_Controller.CheckCollision(Velocity, out RaycastHit Hit))
        {
            //Surface normal is in the opposite direction of the velocity, bounce of it!
            if(Vector2.Dot(Velocity.normalized, Hit.normal) < 0.0f)
            {
                NewVelocity.x = -NewVelocity.x;
                Speed         = -Speed; //Invert speed as well to make sure when we land we resume in that direction
            }

            //TODO Check if you've hit your head
        }

        if (Velocity.y < 0.0f)
            return m_Controller.m_AirbourneState;

        Velocity = NewVelocity;

        return base.OnUpdate();
    }
}

public class PlayerAirbourneState : PlayerState
{
    public PlayerAirbourneState(PlayerController Controller) : base(Controller)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        CurrentSprite = m_Controller.m_Airbourne;
    }

    public override PlayerState OnUpdate()
    {
        if (Grounded)
            return m_Controller.m_GroundedState;

        //Fall
        Velocity -= new Vector2(0, m_Controller.m_Gravity * Time.deltaTime);

        return base.OnUpdate();
    }
}
