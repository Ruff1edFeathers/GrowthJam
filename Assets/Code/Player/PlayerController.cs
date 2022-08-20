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
    public float m_WalkSpeed;
    public float m_RunSpeed;

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

        //Check if state wants us to handle locomotion
        if ((State.Flags & ePlayerFlags.Locomotion) == 0)
            return;

        Vector3     Position = transform.position;
        SideResults Side = Manager.GetSide(Position);

        //Clamp X & Z position onto platform side
        //Y is unclamped for airbourne movement
        Position.x = Side.Position.x;
        Position.z = Side.Position.z;

        //Calculate Movement along side delta TODO: Replace with unity Input System
        float Speed      = Input.GetKey(KeyCode.LeftShift) ? m_RunSpeed : m_WalkSpeed;
        float Horizontal = Input.GetAxis("Horizontal");

        Vector3 Velocity = -Side.Delta * Horizontal * Speed;

        //Apply new Rotation and positions
        transform.rotation = Quaternion.LookRotation(Side.Normal);
        transform.position = Position + (Velocity * Time.deltaTime);

        //Check if state lets us jump
        if ((State.Flags & ePlayerFlags.CanJump) == 0)
            return;

        //TODO: Implement :)
    }
}
