using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerController Controller) : base(Controller) {}

    public override void OnEnter()
    {
        base.OnEnter();

        //Kill All Velocity
        Speed = 0;
        Velocity = Vector3.zero;

        CurrentSprite = m_Controller.m_Hurt;

        //Play Animation of player falling off platform
        m_Controller.m_Animator.Play(m_Controller.m_DeathAnim.name);
    }

    public override PlayerState OnUpdate()
    {
        if (m_Controller.m_Animator.isPlaying)
            return base.OnUpdate();

        //Reset Sprite
        m_Controller.m_Renderer.color = Color.white;
        m_Controller.m_Renderer.transform.localPosition = Vector3.zero;
        m_Controller.m_Renderer.transform.localRotation = Quaternion.identity;

        //Respawn
        Position = m_Controller.m_RespawnPoint;
        m_Controller.m_Health = m_Controller.m_MaxHealth;
        HUDUI.s_Instance.UpdateHealth(m_Controller.m_MaxHealth);

        return m_Controller.m_GroundedState;
    }
}
