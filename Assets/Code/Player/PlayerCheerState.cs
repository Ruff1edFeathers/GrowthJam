using UnityEngine;

public class PlayerCheerState : PlayerState
{
    public PlayerCheerState(PlayerController Controller) : base(Controller) {}

    private float m_Timer;

    public override void OnEnter()
    {
        base.OnEnter();

        Speed    = 0.0f;
        Velocity = Vector2.zero;

        CurrentSprite = m_Controller.m_Cheer0;
        m_Timer = m_Controller.m_CheerTime;
    }

    public override PlayerState OnUpdate()
    {
        if(m_Timer > 0.0f)
        {
            m_Timer -= Time.deltaTime;
            return base.OnUpdate();
        }

        CurrentSprite = CurrentSprite == m_Controller.m_Cheer0 ? m_Controller.m_Cheer1 : m_Controller.m_Cheer0;
        m_Timer = m_Controller.m_CheerTime;

        return base.OnUpdate();
    }
}
