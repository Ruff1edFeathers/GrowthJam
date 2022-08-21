using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public PlayerController m_Controller;

    private PlayerState m_SubState;
    protected float m_TimeInState;

    public PlayerState SubState
    {
        get { return m_SubState; }
        set
        {
            if (m_SubState == value)
                return;

#if UNITY_EDITOR
            Debug.Log($"Transition SubState: {(m_SubState != null ? m_SubState.GetType().Name : "NULL")} ===> {(value != null ? value.GetType().Name : "NULL")}");
#endif

            m_SubState?.OnExit();
            m_SubState = value;
            m_SubState?.OnEnter();
        }
    }

    public SideResults Side { get { return m_Controller.m_Side; } }

    public Vector3 Position
    {
        get { return m_Controller.m_Position; }
        set { m_Controller.m_Position = value; } 
    }

    public Vector2 Velocity
    {
        get { return m_Controller.m_Velocity; }
        set { m_Controller.m_Velocity = value; }
    }

    public float Speed
    {
        get { return m_Controller.m_Speed; }
        set { m_Controller.m_Speed = value; }
    }

    public bool Grounded 
    { 
        get { return m_Controller.m_Grounded; }
        set { m_Controller.m_Grounded = value; }
    }

    public Vector3 Grounded_Normal { get { return m_Controller.m_GroundNormal; } }

    public Sprite CurrentSprite
    {
        get { return m_Controller.m_Renderer.sprite; }
        set { m_Controller.m_Renderer.sprite = value; }
    }

    public PlayerState(PlayerController Controller)
    {
        m_Controller = Controller;
    }

    public virtual void OnEnter()
    {
        m_TimeInState = 0;
        SubState?.OnEnter();
    }

    public virtual PlayerState OnUpdate()
    {
        if (SubState != null)
        {
            PlayerState NewState = SubState.OnUpdate();
            if (NewState != SubState)
                SubState = NewState;
        }

        m_TimeInState += Time.deltaTime;
        return this;
    }

    public virtual void OnExit()
    {
        SubState?.OnExit();
        m_TimeInState = 0;
    }
}