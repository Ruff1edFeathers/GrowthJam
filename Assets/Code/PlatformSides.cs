using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SideResults
{
    public Vector3 Position; //Point along side
    public Vector3 Normal;
    public Vector3 Right; //Right Direction for this side
}

//Add support for multiple PlatformSides, could have different segment counts
//Add support for live updating the points?! Potentially have a side shifting as part of gameplay (thinking face)
[ExecuteInEditMode]
public class PlatformSides : MonoBehaviour
{
    public static PlatformSides Instance;

    public Transform[] m_ControlPoints = new Transform[2];

    struct SideData
    {
        public Vector3 P0;     //Start of Line
        public Vector3 P1;     //End of Line
        public Vector3 Delta;  //Direction along Line
        public Vector3 Normal; //Direction perpendicular to the line
    }

    private SideData[] m_Sides;

    public SideResults GetSide(Vector3 Position)
    {
        //Test Position against all the sides
        Vector3 ClosestSide_Point = Vector3.zero;
        float   ClosetSide_Dist   = float.MaxValue;
        int     ClosetSide_IDx    = -1;

        int Len = m_Sides.Length;
        for (int i = 0; i < Len; i++)
        {
            Vector3 SidePoint = VectorUtil.ProjectPointOnLineSegment(m_Sides[i].P0, m_Sides[i].P1, Position);
            float   Dist      = Vector3.Distance(SidePoint, Position);

            if(Dist < ClosetSide_Dist)
            {
                ClosestSide_Point = SidePoint;
                ClosetSide_Dist   = Dist;
                ClosetSide_IDx    = i;
            }
        }

        return new SideResults()
        {
            Position = ClosestSide_Point,
            Normal   = m_Sides[ClosetSide_IDx].Normal,
            Right    = m_Sides[ClosetSide_IDx].Delta
        };
    }

    private void OnEnable()
    {
        Instance = this;

        if (m_ControlPoints == null || m_ControlPoints.Length < 2)
            throw new System.Exception("Invalid amount of control points");

        //Calculate side data from the control points
        int Len = m_ControlPoints.Length;
        m_Sides = new SideData[Len];
        for(int i = 0; i < Len; i++)
        {
            SideData Data;
            Data.P0     = m_ControlPoints[i].position;
            Data.P1     = m_ControlPoints[(i + 1) % Len].position;
            Data.Delta  = Vector3.Normalize(Data.P1 - Data.P0);
            Data.Normal = Vector3.Normalize(Vector3.Cross(Data.Delta, Vector3.up));
            m_Sides[i]  = Data;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_Sides == null)
            return;

        int Len = m_Sides.Length;
        for(int i = 0; i < Len; i++)
        {
            Gizmos.DrawLine(m_Sides[i].P0, m_Sides[i].P1);
            Gizmos.DrawRay(Vector3.Lerp(m_Sides[i].P0, m_Sides[i].P1, 0.5f), m_Sides[i].Normal);
        }
    }
}
