using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State for grounded movement & idle?
public class PlayerLocomotionState : PlayerState
{
    public PlayerLocomotionState(PlayerController Controller) : base(Controller)
    {
    }

    public override ePlayerFlags Flags => ePlayerFlags.Locomotion | ePlayerFlags.CanJump;
}
