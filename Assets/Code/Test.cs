using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform Start;
    public Transform End;
    public Transform Point;

    private void OnDrawGizmos()
    {
        if (Start == null || End == null || Point == null)
            return;

        Gizmos.DrawLine(Start.position, End.position);
        Vector3 LinePoint = VectorUtil.ProjectPointOnLineSegment(Start.position, End.position, Point.position);
        Gizmos.DrawRay(LinePoint, Vector3.up);
    }
}
