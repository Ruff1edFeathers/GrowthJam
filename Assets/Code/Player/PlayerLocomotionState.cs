using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedSubState : PlayerState
{
    public PlayerGroundedState m_GroundedState;
    public bool m_CanJump;

    public PlayerGroundedSubState(PlayerGroundedState GroundedState) : base(GroundedState.m_Controller)
    {
        m_GroundedState = GroundedState;
    }
}

//State for grounded movement & idle?
public class PlayerGroundedState : PlayerState
{
    public PlayerIdleState m_IdleState;
    public PlayerMoveState m_MoveState;
    public PlayerSkidState m_SkidState;
    public PlayerStopState m_StopState;

    public PlayerGroundedState(PlayerController Controller) : base(Controller)
    {
        m_IdleState = new PlayerIdleState(this);
        m_MoveState = new PlayerMoveState(this);
        m_SkidState = new PlayerSkidState(this);
        m_StopState = new PlayerStopState(this);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        SubState = Mathf.Abs(Speed) > 0f ? m_MoveState : m_IdleState;
    }

    public override PlayerState OnUpdate()
    {
        //Make sure we are still grounded, otherwise transition to the airbourne state
        if (!Grounded)
        {
            return m_Controller.m_AirbourneState;
        }

        //Calculate Movement along side delta and Project Movement onto surface normal
        //Vector3 Movement = -Side.Delta * Speed;
        //float Movement_Len = Movement.magnitude;
        //Movement = Vector3.Normalize(Movement);
        //
        //float SurfaceDot = Vector3.Dot(Movement, Grounded_Normal);
        //Vector3 SurfaceVelocity = Vector3.Normalize(Movement - Grounded_Normal * SurfaceDot) * Movement_Len;

        Vector2 NewVelocity = Velocity;
        NewVelocity.x = Speed;

        if (m_Controller.CheckCollision(NewVelocity, out RaycastHit Hit))
        {
            //We've hit something while grounded... say good bye to your speed
            //TODO: Play Animation & Sound effect :)
            Speed = 0;
            NewVelocity = Vector2.zero;
            Debug.Log("Hit Something");
        }

        Velocity = NewVelocity;

        if (((PlayerGroundedSubState)SubState).m_CanJump && InputWrapper.GetButtonState(eInputAction.Jump).IsPressed())
        {
            //Move to jump state!
            return m_Controller.m_JumpState;
        }

        return base.OnUpdate();
    }
}

public class PlayerIdleState : PlayerGroundedSubState
{
    public PlayerIdleState(PlayerGroundedState GroundedState) : base(GroundedState) 
    {
        m_CanJump = true;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        //Player is Idle
        CurrentSprite = m_Controller.m_Stand;
    }

    public override PlayerState OnUpdate()
    {
        //Check if the player wants to move
        if (InputWrapper.GetAxis(eInputAction.Movement).HasInput())
            return m_GroundedState.m_MoveState;

        return base.OnUpdate();
    }
}

public class PlayerMoveState : PlayerGroundedSubState
{
    public PlayerMoveState(PlayerGroundedState GroundedState) : base(GroundedState)
    {
        m_CanJump = true;
    }

    private float m_StopTimer;

    public override void OnEnter()
    {
        base.OnEnter();

        m_StopTimer = 0.0f;
        CurrentSprite = m_Controller.m_Walk1;
    }

    public override PlayerState OnUpdate()
    {
        //Claculate Speed
        PollingAxis MovementAxis = InputWrapper.GetAxis(eInputAction.Movement);

        //Check if the player is pushing in a different direction with enough speed built up to skid
        if(MovementAxis.HasInput() && Mathf.Abs(m_Controller.m_Speed) > m_Controller.m_SkidSpeed && Mathf.Sign(m_Controller.m_Speed) != Mathf.Sign(MovementAxis.m_Value))
        {
            //Player has changed direction, skid!!
            PlayerSkidState SkidState = m_GroundedState.m_SkidState;
            SkidState.m_StartSpeed = Speed;
            SkidState.m_TargetSpeed = Speed / 2;
            return SkidState;
        }

        //Check if the player is pushing on the stick
        if (!MovementAxis.HasInput())
        {
            //Give grace period to perform a skid instead!
            if (m_StopTimer >= m_Controller.m_StopDelay)
            {
                //No input, transition into the stopping state
                return m_GroundedState.m_StopState;
            }

            m_StopTimer += Time.deltaTime;
        }
        else
        {
            if (Mathf.Abs(Speed) < m_Controller.m_BaseSpeed)
                Speed = m_Controller.m_BaseSpeed * Mathf.Sign(MovementAxis);

            //Check if we want to run
            if (InputWrapper.GetButtonState(eInputAction.Run).IsPressedOrHeld())
            {
                //Player is pushing movement in same dir, accumulate speed
                Speed += MovementAxis * m_Controller.m_AccelSpeed * Time.deltaTime;
                Speed = Mathf.Clamp(Speed, -m_Controller.m_MaxSpeed, m_Controller.m_MaxSpeed); //Make sure we dont exceed the max speed
            }
            else if(Mathf.Abs(Speed) > m_Controller.m_BaseSpeed)
            {
                //Slow down to base speed
                Speed += m_Controller.m_StopFricition * Time.deltaTime * -Mathf.Sign(Speed);
            }
        }

        //Play running animation
        float TimeBetweenFrames = m_Controller.m_WalkAnimSpeed * (1f - Mathf.Clamp01(m_Controller.m_Speed / m_Controller.m_MaxSpeed));
        if (m_TimeInState > TimeBetweenFrames)
        {
            m_TimeInState = 0;
            CurrentSprite = CurrentSprite == m_Controller.m_Walk1 ? m_Controller.m_Walk2 : m_Controller.m_Walk1;
        }
        
        return base.OnUpdate();
    }
}

public class PlayerSkidState : PlayerGroundedSubState
{
    public float m_StartSpeed;
    public float m_TargetSpeed;

    public PlayerSkidState(PlayerGroundedState GroundedState) : base(GroundedState) { }

    public override void OnEnter()
    {
        base.OnEnter();

        CurrentSprite = m_Controller.m_Skid;
    }

    public override PlayerState OnUpdate()
    {
        //TODO: Play smoke particles, skid noise and maybe over rotate sprite?

        float Alpha = m_TimeInState / m_Controller.m_SkidTime;
        Speed = Mathf.Lerp(m_StartSpeed, m_TargetSpeed, Alpha);

        if(Alpha >= 1.0f)
        {
            //Finished Skid, invert the speed direction
            Speed = -Speed;

            if (InputWrapper.GetAxis(eInputAction.Movement).HasInput())
            {
                // Input on movement detected, transition into movement
                return m_GroundedState.m_MoveState;
            }
            else
            {
                // No movement detected, transition to stop state
                return m_GroundedState.m_StopState;
            }
        }

        return base.OnUpdate();
    }
}

public class PlayerStopState : PlayerGroundedSubState
{
    public PlayerStopState(PlayerGroundedState GroundedState) : base(GroundedState) { }

    public override void OnEnter()
    {
        base.OnEnter();

        CurrentSprite = m_Controller.m_Skid;
    }

    public override PlayerState OnUpdate()
    {
        //Remove Speed till theres nothing left
        Speed += m_Controller.m_StopFricition * Time.deltaTime * -Mathf.Sign(Speed);

        if(Mathf.Abs(Speed) < 0.1f)
        {
            //No more speed, so stop
            Speed = 0f;
            return m_GroundedState.m_IdleState;
        }

        if (InputWrapper.GetAxis(eInputAction.Movement).HasInput())
            return m_GroundedState.m_MoveState;

        return base.OnUpdate();
    }
}