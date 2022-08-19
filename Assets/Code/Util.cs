using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtil
{
    public static Vector3 ProjectPointOnLineSegment(Vector3 Start, Vector3 End, Vector3 Point)
    {
        Vector3 LineDelta = Start - End;
        Point = Start + Vector3.Project(Point - Start, End - Start);

        float StartDist  = (Point - Start).sqrMagnitude;
        float EndDist    = (Point - End).sqrMagnitude;
        float LineLength = LineDelta.sqrMagnitude;

        if (StartDist > LineLength || EndDist > LineLength) 
            return StartDist > EndDist ? End : Start;

        return Point;
    }
}
