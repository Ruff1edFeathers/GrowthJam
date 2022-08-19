using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private void Update()
    {
        if (PlatformSides.Instance != null)
        {
            SideResults Result = PlatformSides.Instance.GetSide(transform.position);
            Debug.DrawRay(Result.Position, Result.Normal, Color.blue);
            Debug.DrawRay(Result.Position, Result.Right, Color.green);
        }
    }
}
