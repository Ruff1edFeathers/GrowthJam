using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public void OnUpdate(PlatformManager Manager)
    {
        SideResults Result = Manager.GetSide(transform.position);
        Debug.DrawRay(Result.Position, Result.Normal, Color.blue);
        Debug.DrawRay(Result.Position, Result.Right, Color.green);

    }
}
