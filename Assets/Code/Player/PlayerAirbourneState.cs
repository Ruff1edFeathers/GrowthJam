using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirbourneState : PlayerState
{
    public PlayerAirbourneState(PlayerController Controller) : base(Controller)
    {
    }

    public override PlayerState OnUpdate()
    {
        if (Grounded)
            return m_Controller.m_GroundedState;

        //Fall
        Velocity += new Vector3(0, m_Controller.m_Gravity * Time.deltaTime, 0);

        return base.OnUpdate();
    }
}
