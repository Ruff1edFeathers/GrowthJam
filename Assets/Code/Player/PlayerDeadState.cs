using System.Collections;
using System.Collections.Generic;
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

        //Play Animation of player falling off platform
        m_Controller.m_Animator.Play(m_Controller.m_DeathAnim.name);
        CameraController.s_Instance.m_State = eCameraState.RotateAround;
    }
}
