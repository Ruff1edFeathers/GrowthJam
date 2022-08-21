using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public Animation m_Animator;
    public float m_JumpForce;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController Controller = other.GetComponentInParent<PlayerController>();
        if (Controller == null)
            return;

        //Make sure the player is airbourne, so falling down onto the jump pad
        if (Controller.State != Controller.m_AirbourneState)
            return;

        //Put player into jump state
        Controller.m_JumpState.m_JumpForce = m_JumpForce;
        Controller.State = Controller.m_JumpState;

        //Play Spring Animation
        m_Animator.Play("SpringAnim");
    }
}
