using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int m_MaxHealth = 5;
    public float m_DamageGracePeroid = 2f;

    //Inspector Values
    [Header("Speeds")]
    public float m_BaseSpeed = 2.5f;
    public float m_AccelSpeed = 1.25f;
    public float m_MaxSpeed = 10.0f;
    public float m_SkidSpeed = 2f;
    public float m_StopDelay = 0.25f;
    public float m_JumpForce = 20;
    public float m_SlideMargin = 3f;

    [Header("Physics")]
    public CapsuleCollider m_Collider;
    public float m_MaxSlopeAngle = 45f;
    public float m_StopFricition = 1.5f;
    public float m_SlideFriction = 0.75f;
    [Range(0f, 1f)]
    public float m_SlideHeightFrac = 0.75f;
    public float m_Gravity = 2f;
    public float m_RayCheckOffset = 0.25f;
    public int m_RayCheckCount = 4;
    public LayerMask m_GroundMask;

    [Header("Animation")]
    public SpriteRenderer m_Renderer;
    public Animation m_Animator;
    public AnimationClip m_DeathAnim;
    public Color m_HurtColour = Color.red;
    public float m_FlashSpeed = 0.25f;
    public Sprite m_Stand;
    public float m_WalkAnimSpeed = 0.25f;
    public Sprite m_Walk1;
    public Sprite m_Walk2;
    public float m_SkidTime = 0.5f;
    public Sprite m_Skid;
    public Sprite m_Slide;
    public Sprite m_Jump;
    public Sprite m_Airbourne;
    public Sprite m_Hurt;

    public PlayerState State
    {
        get { return m_State; }
        set
        {
            if (m_State == value)
                return;

#if UNITY_EDITOR
            Debug.Log($"Transition Main State: {(m_State != null ? m_State.GetType().Name : "NULL")} ===> {(value != null ? value.GetType().Name : "NULL")}");
#endif

            m_State?.OnExit();
            m_State = value;
            m_State?.OnEnter();
        }
    }

    //Cached Values
    private PlayerState m_State;
    private float m_DamgeGraceTimer;

    public SideResults m_Side;
    [System.NonSerialized] public float   m_Height;
    [System.NonSerialized] public float   m_Speed;
    [System.NonSerialized] public Vector2 m_Velocity;
    [System.NonSerialized] public Vector3 m_Position;
    [System.NonSerialized] public bool    m_Grounded;
    [System.NonSerialized] public Vector3 m_GroundedHit;
    [System.NonSerialized] public Vector3 m_GroundNormal;
    [System.NonSerialized] public int     m_Health;

    public PlayerGroundedState  m_GroundedState;
    public PlayerAirbourneState m_AirbourneState;
    public PlayerJumpState      m_JumpState;
    public PlayerDeadState      m_DeadState;

    public void OnSetup(PlatformManager Manager)
    {
        m_GroundedState  = new PlayerGroundedState(this);
        m_AirbourneState = new PlayerAirbourneState(this);
        m_JumpState      = new PlayerJumpState(this);
        m_DeadState      = new PlayerDeadState(this);

        m_Health = m_MaxHealth;
        m_Height = m_Collider.height;
        State    = m_GroundedState;

        //Update Health UI
        HUDUI.s_Instance.SetupHealth(m_MaxHealth);
        HUDUI.s_Instance.UpdateHealth(m_Health);
    }

    public void OnUpdate(PlatformManager Manager)
    {
        if (State == null)
            return; //No State nothing to do

        if(m_DamgeGraceTimer > 0.0f)
        {
            //Flash Hurt colour whilst in the damage grace peroid
            float FlashAlpha = Mathf.PingPong(Time.time, m_FlashSpeed) / m_FlashSpeed;
            m_Renderer.color = Color.Lerp(Color.white, m_HurtColour, FlashAlpha);
            m_DamgeGraceTimer -= Time.deltaTime;

            if(m_DamgeGraceTimer <= 0.0f)
            {
                m_Renderer.color = Color.white;
            }
        }

        int LastIDx = m_Side.IDx;
        m_Position = transform.position;
        m_Side     = Manager.GetSide(m_Position);

        if (LastIDx != m_Side.IDx)
        {
            //
        }

        if(m_State != m_JumpState)
            CheckGrounded();

        //Clamp X & Z position onto platform side
        m_Position.x = m_Side.Position.x;
        if (m_Grounded) m_Position.y = m_GroundedHit.y; //Player is grounded clamp to the ground y position
        m_Position.z = m_Side.Position.z;

        //Track speed to determine the direction the sprite should be facing
        m_Renderer.flipX = m_Speed > 0;

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

        //Calculate this frames velocity and subtract it from the new total
        Vector2 FrameVelocity = m_Velocity * Time.deltaTime;
        m_Velocity -= FrameVelocity;

        //Convert 2D Velocity into 3D Position
        m_Position += (m_Side.Delta * FrameVelocity.x) + new Vector3(0, FrameVelocity.y);

        //Apply new Rotation and positions
        transform.rotation = Quaternion.LookRotation(m_Side.Normal); //TODO: These rotations can be pre-calculated
        transform.position = m_Position;
    }

    public void TakeDamage()
    {
        if(m_DamgeGraceTimer > 0.0f)
        {
            //Avoid taking damage as we are immune for abit
            return;
        }

        m_Health--;
        HUDUI.s_Instance.UpdateHealth(m_Health);

        if (m_Health <= 0)
        {
            //We've died?! Change state right away
            State = m_DeadState;
            GameManager.s_Instance.GameOver();
            return;
        }

        //Enter into immunity grace peroid
        m_DamgeGraceTimer = m_DamageGracePeroid;
    }

    public void CheckGrounded()
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

    public bool CheckCollision(Vector2 Velocity, out RaycastHit Hit)
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
        Vector3 VelDir  = Vector3.Normalize((m_Side.Delta * Velocity.x) + new Vector3(0, Velocity.y));
        
        for (int i = 0; i < m_RayCheckCount; i++)
        {
            Vector3 Origin = m_Position + (Offset * i) + new Vector3(0, Padding, 0);
            if(Physics.Raycast(Origin, VelDir, out Hit, m_RayCheckOffset, m_GroundMask, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawRay(Origin, VelDir, Color.green);
                if (Vector3.Angle(Hit.normal, Vector3.up) > m_MaxSlopeAngle)
                {
                    return true;
                }
            }
            else
            {
                Debug.DrawRay(Origin, VelDir, Color.red);
            }
        }

        Hit = default;
        return false;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height - 25,  300, 25), "Speed: " + m_Speed);
        GUI.Label(new Rect(0, Screen.height - 50, 300, 25),  "Velocity: " + m_Velocity);
        GUI.Label(new Rect(0, Screen.height - 75, 300, 25),  "Health: " + m_Health);
    }

    
}
