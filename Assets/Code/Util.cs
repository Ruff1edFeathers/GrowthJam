using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtil
{
    public static Vector2 FlattenXZToXY(this Vector3 Value)
    {
        return new Vector2(Value.x, Value.z);
    }
}
