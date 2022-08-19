using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Add support for multiple PlatformSides, could have different segment counts
//Add support for live updating the points?! Potentially have a side shifting as part of gameplay (thinking face)
[ExecuteInEditMode]
public class PlatformSides : MonoBehaviour
{
    public static PlatformSides Instance;

    struct SideData
    {
        public Vector3 P0;
        public Vector3 P1;
        public Vector3 Normal;
    }

    public Transform[] m_ControlPoints = new Transform[2];

    private SideData[] m_Sides;

    public void TestPoint(Vector3 Position)
    {
        //Test Position against all the sides
        int Len = m_Sides.Length;
        for(int i = 0; i < Len; i++)
        {

        }
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
            Data.Normal = Vector3.Cross(Vector3.Normalize(Data.P1 - Data.P0), Vector3.up);
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
