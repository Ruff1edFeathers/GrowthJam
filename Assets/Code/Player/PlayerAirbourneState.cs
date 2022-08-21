using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirbourneState
{
    public PlayerJumpState(PlayerController Controller) : base(Controller) {}

    public float m_JumpForce;

    public override void OnEnter()
    {
        //Force Grounded to be false since we are about to jump
        Grounded  = false;
        Velocity += new Vector2(0, m_JumpForce);

        CurrentSprite = m_Controller.m_Jump;
    }

    public override PlayerState OnUpdate()
    {
        if (Velocity.y < 0.0f)
            return m_Controller.m_AirbourneState;

        return base.OnUpdate();
    }
}

public class PlayerAirbourneState : PlayerState
{
    public PlayerAirbourneState(PlayerController Controller) : base(Controller) {}

    public override void OnEnter()
    {
        base.OnEnter();

        CurrentSprite = m_Controller.m_Airbourne;
    }

    public override PlayerState OnUpdate()
    {
        if (Grounded)
        {
            return m_Controller.m_GroundedState;
        }

        Vector2 NewVelocity = Velocity;

        //Subtract gravity
        NewVelocity -= new Vector2(0, m_Controller.m_Gravity * Time.deltaTime);

        //Ensure X Velocity stays consistant whilst jumping
        NewVelocity.x = Speed + (InputWrapper.GetAxis(eInputAction.Movement) * m_Controller.m_AirbourneSpeed);

        if (m_Controller.CheckCollision(new Vector3(NewVelocity.x, 0), out RaycastHit Hit))
        {
            NewVelocity.x = -NewVelocity.x;
            Speed = -(Speed / 2.0f); //Invert speed as well to make sure when we land we resume in that direction
        }

        Velocity = NewVelocity;

        return base.OnUpdate();
    }
}
