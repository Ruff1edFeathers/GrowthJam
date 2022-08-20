using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerFlags
{
    None        = 0,
    Locomotion  = 1 << 0,
    CanJump     = 1 << 1,
    CanInteract = 1 << 3
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
    public float m_BaseSpeed = 2.5f;
    public float m_AccelSpeed = 1.25f;
    public float m_MaxSpeed = 10.0f;
    public float m_Friction = 0.25f;
    public float m_Gravity = 2f;
    public float m_GroundCheckOffset = 0.25f;
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
    private Vector3     m_Position;
    private bool        m_Grounded;
    private Vector3     m_GroundedHit;
    private Vector3     m_GroundNormal;

    public PlayerLocomotionState m_LocomotionState;

    public void OnSetup(PlatformManager Manager)
    {
        m_LocomotionState = new PlayerLocomotionState(this);

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
        Vector3 RayOffset = new Vector3(0, m_GroundCheckOffset / 2f, 0);
        if (!Physics.Raycast(m_Position + RayOffset, -RayOffset, out RaycastHit Hit, m_GroundCheckOffset, m_GroundMask, QueryTriggerInteraction.Ignore))
        {
            m_Grounded = false;
            return;
        }

        m_Grounded     = true;
        m_GroundedHit  = Hit.point;
        m_GroundNormal = Hit.normal;
    }

    private void DoLocomotion()
    {
        Vector3 Velocity = Vector3.zero;
        Vector3 NewPosition = m_Position;

        //Clamp X & Z position onto platform side
        //Y is unclamped for airbourne movement
        NewPosition.x = m_Side.Position.x;
        NewPosition.z = m_Side.Position.z;

        //Check if we are both grounded and state wants us to handle locomotion
        if (m_Grounded && (State.Flags & ePlayerFlags.Locomotion) != 0)
        {
            //Player is grounded clamp to the ground y position
            NewPosition.y = m_GroundedHit.y;

            //Calculate Movement along side delta   
            Velocity += -m_Side.Delta * m_BaseSpeed * InputWrapper.GetAxis(eInputAction.Movement);

            //Check if state lets us jump
            if ((State.Flags & ePlayerFlags.CanJump) != 0 && InputWrapper.GetButtonState(eInputAction.Jump).IsPressed())
            {
                Debug.Log("Jump... Implement :)");
            }
        }

        if (!m_Grounded)
            Velocity.y -= m_Gravity;

        //Apply new Rotation and positions
        transform.rotation = Quaternion.LookRotation(m_Side.Normal);
        transform.position = NewPosition + (Velocity * Time.deltaTime);
    }
}
