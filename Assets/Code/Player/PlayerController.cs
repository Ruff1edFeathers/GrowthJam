using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerFlags
{
    None        = 0,
    Locomotion  = 1 << 0,
    CanJump     = 1 << 1,
    CanInteract = 1 << 3,
    CanCrouch   = 1 << 4
}

//TODO: Make this control animation that is being played on player
//TODO: Abstract for NPCs? Hmmm....
public abstract class PlayerState
{
    public PlayerController m_Controller;

    public abstract ePlayerFlags Flags { get; }

    public PlayerState(PlayerController Controller)
    {
        m_Controller = Controller;
    }

    public virtual void        OnEnter()  { }
    public virtual PlayerState OnUpdate() { return this; }
    public virtual void        OnExit()   { }
}

public class PlayerController : MonoBehaviour
{
    //Inspector Values
    [Header("Speeds")]
    public float m_BaseSpeed = 2.5f;
    public float m_AccelSpeed = 1.25f;
    public float m_MaxSpeed = 10.0f;

    [Header("Physics")]
    public CapsuleCollider m_Collider;
    public float m_MaxSlopeAngle = 45f;
    public float m_StopFricition = 1.5f;
    public float m_Gravity = 2f;
    public float m_RayCheckOffset = 0.25f;
    public int m_RayCheckCount = 4;
    public LayerMask m_GroundMask;

    public PlayerState State
    {
        get { return m_State; }
        set
        {
            m_State?.OnExit();
            m_State = value;
            m_State?.OnEnter();
        }
    }

    //Cached Values
    private PlayerState m_State;
    private SideResults m_Side;
    private float       m_Height;
    private float       m_Speed;
    private Vector3     m_Velocity;
    private Vector3     m_Position;
    private bool        m_Grounded;
    private Vector3     m_GroundedHit;
    private Vector3     m_GroundNormal;

    public PlayerLocomotionState m_LocomotionState;

    public void OnSetup(PlatformManager Manager)
    {
        m_LocomotionState = new PlayerLocomotionState(this);

        m_Height = m_Collider.height;

        State = m_LocomotionState;
    }

    public void OnUpdate(PlatformManager Manager)
    {
        if (State == null)
            return; //No State nothing to do

        //Update the current state
        PlayerState NewState = State.OnUpdate();

        //Check if the state has changed
        if (NewState != State)
        {
            State = NewState;

            //State could of changed so check
            if (State == null)
                return; //No State nothing to do
        }

        m_Position = transform.position;
        m_Side     = Manager.GetSide(m_Position);

        CheckGrounded();
        DoLocomotion();
    }

    private void CheckGrounded()
    { 
        //Check if the player is grounded
        //TODO: Multiple Casts and get the average normal for smoother movement?!
        Vector3 RayOffset = new Vector3(0, m_RayCheckOffset / 2f, 0);
        if (!Physics.Raycast(m_Position + RayOffset, -RayOffset, out RaycastHit Hit, m_RayCheckOffset, m_GroundMask, QueryTriggerInteraction.Ignore))
        {
            m_Grounded = false;
            return;
        }

        m_Grounded     = true;
        m_GroundedHit  = Hit.point;
        m_GroundNormal = Hit.normal;
    }

    private bool CheckCollision(Vector3 Velocity, out RaycastHit Hit)
    {
        //Check for collision along the projected velocity
        //TODO: Check if hit collision normal is less than the max accepted slope angle
        //TODO: Bounce off wall if velocity is greater than X Value, subtracting some velocity
        //Note: Only account for velocity that is greater than 0,
        //dont reflect negative values otherwise the player will bounce off the floor!

        //Offset by a small padding to make sure we dont collide with the floor while grounded
        const float Padding = 0.1f;
        float   Height  = m_Collider.height - Padding;
        Vector3 Offset  = new Vector3(0, Height / m_RayCheckCount, 0);
        Vector3 VelDir  = Vector3.Normalize(new Vector3(Velocity.x, Velocity.y > 0.0f ? Velocity.y : 0.0f, Velocity.z));

        for(int i = 0; i < m_RayCheckCount; i++)
        {
            Vector3 Origin = m_Position + (Offset * i) + new Vector3(0, Padding, 0);
            if(Physics.Raycast(Origin, VelDir, out Hit, m_RayCheckOffset, m_GroundMask, QueryTriggerInteraction.Ignore))
            {
                if(Vector3.Angle(Hit.normal, Vector3.up) > m_MaxSlopeAngle)
                {
                    return true;
                }
            }
        }

        Hit = default;
        return false;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 25), "Current Speed: " + m_Speed);
    }

    private void DoLocomotion()
    {
        Vector3 NewVelocity = m_Velocity;
        Vector3 NewPosition = m_Position;

        //Clamp X & Z position onto platform side
        //Y is unclamped for airbourne movement
        NewPosition.x = m_Side.Position.x;
        NewPosition.z = m_Side.Position.z;

        //Check if we are both grounded and state wants us to handle locomotion
        if (m_Grounded)
        {
            if ((State.Flags & ePlayerFlags.Locomotion) != 0)
            {
                //Player is grounded clamp to the ground y position
                NewPosition.y = m_GroundedHit.y;

                //Claculate Speed
                PollingAxis MovementAxis = InputWrapper.GetAxis(eInputAction.Movement);

                //Check if the player is pushing on the stick
                if (MovementAxis.HasInput())
                {
                    //Make sure the player is pushing in the same direction
                    if (Mathf.Abs(m_Speed) < 1f || Mathf.Sign(m_Speed) == Mathf.Sign(MovementAxis.m_Value))
                    {
                        //TODO: Play an Animation Here
                        m_Speed += InputWrapper.GetAxis(eInputAction.Movement) * m_BaseSpeed * Time.deltaTime;
                        m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed); //Make sure we dont exceed the max speed
                    }
                    else
                    {
                        //Player has pushed in opisite direction, permform a turn around which results in losing half the speed
                        //TODO: Play an animation here
                        m_Speed = -(m_Speed / 2);
                    }
                }
                else
                {
                    //TODO: Play an Animation Here
                    //No input, slow down the speed!
                    m_Speed -= m_Speed * m_StopFricition * Time.deltaTime;
                }

                //Calculate Movement along side delta and Project Movement onto surface normal
                Vector3 Movement = -m_Side.Delta * m_Speed;
                float Movement_Len = Movement.magnitude;
                Movement = Vector3.Normalize(Movement);

                float SurfaceDot = Vector3.Dot(Movement, m_GroundNormal);
                Vector3 SurfaceVelocity = Vector3.Normalize(Movement - m_GroundNormal * SurfaceDot) * Movement_Len;

                if (CheckCollision(NewVelocity, out RaycastHit Hit))
                {
                    //We've hit something while grounded... say good bye to your speed
                    //TODO: Play Animation & Sound effect :)
                    m_Speed = 0;
                    SurfaceVelocity = Vector3.zero;
                }

                NewVelocity = SurfaceVelocity;

                //Check if state lets us jump
                if ((State.Flags & ePlayerFlags.CanJump) != 0 && InputWrapper.GetButtonState(eInputAction.Jump).IsPressed())
                {
                    Debug.Log("Jump... Implement :)");
                }
            }
        }
        else
        {
            //Player is airbourne, TODO: Implement :)

        }

        if (!m_Grounded)
            NewVelocity.y -= m_Gravity;

        //Calculate this frames velocity and subtract it from the new total
        Vector3 FrameVelocity = NewVelocity * Time.deltaTime;
        m_Velocity = NewVelocity - FrameVelocity;

        //Apply new Rotation and positions
        transform.rotation = Quaternion.LookRotation(m_Side.Normal);
        transform.position = NewPosition + FrameVelocity;
    }
}
