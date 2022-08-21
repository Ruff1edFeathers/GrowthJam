using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEnv : MonoBehaviour
{
    public Transform[] m_Targets;
    public Transform m_Tracking;
    public float m_StartHeight = 3f;
    public float m_TrackingPadding = 5f;
    public float m_AnimationSpeed_Min = 0.25f;
    public float m_AnimationSpeed_Max = 0.5f;
    public float m_OriginHeightOffset = -2f;

    public AnimationCurve m_HeightCurve;

    private float m_LastHeight;
    private List<AnimData> m_Children   = new List<AnimData>();
    private List<AnimData> m_Animatings = new List<AnimData>();

    private void Awake()
    {
        int Targets_Len = m_Targets.Length;
        for(int t = 0; t < Targets_Len; t++)
        {
            int Child_Len = m_Targets[t].childCount;
            for (int c = 0; c < Child_Len; c++)
            {
                Transform child = m_Targets[t].GetChild(c);

                //Only Grab Children which are active and have a renderer under them
                if (child.gameObject.activeInHierarchy && child.GetComponentInChildren<Renderer>() != null)
                {
                    AnimData Data = new AnimData(child);

                    //Check if block is above start height
                    if (Data.m_Position.y >= m_StartHeight)
                    {
                        //It is, so hide and add it to the list
                        Data.m_Target.gameObject.SetActive(false);
                        m_Children.Add(Data);
                    }
                }
            }
        }

        //Sort the children based on their Y Position
        m_Children.Sort((A, B) => A.m_Position.y.CompareTo(B.m_Position.y));
    }

    private void StartNewAnimations()
    {
        for (int i = m_Children.Count - 1; i >= 0; i--)
        {
            AnimData Data = m_Children[i];

            if (Data.m_Position.y < m_LastHeight + m_TrackingPadding)
            {
                //Move Block to be in the center of the tower and activate it
                Data.m_AnimStartPos      = new Vector3(0, m_LastHeight + m_OriginHeightOffset, 0);
                Data.m_AnimStartRotation = Random.rotation;
                Data.m_AnimSpeed         = Random.Range(m_AnimationSpeed_Min, m_AnimationSpeed_Max);

                Data.m_Target.position   = Data.m_AnimStartPos;
                Data.m_Target.rotation   = Data.m_AnimStartRotation;
                Data.m_Target.localScale = Vector3.zero;

                Data.m_Target.gameObject.SetActive(true);

                m_Animatings.Add(Data);
                m_Children.RemoveAt(i);
            }
        }
    }

    private void UpdateAnimations()
    {
        if (m_Animatings.Count <= 0)
            return;

        for (int i = m_Animatings.Count - 1; i >= 0; i--)
        {
            AnimData Data = m_Animatings[i];
            Data.m_AnimAlpha += Data.m_AnimSpeed * Time.deltaTime;

            Vector3 NewPosition;
            NewPosition.x = Mathf.Lerp(Data.m_AnimStartPos.x, Data.m_Position.x, Data.m_AnimAlpha);
            NewPosition.z = Mathf.Lerp(Data.m_AnimStartPos.z, Data.m_Position.z, Data.m_AnimAlpha);
            NewPosition.y = Data.m_AnimStartPos.y + (Mathf.Abs(Data.m_Position.y - Data.m_AnimStartPos.y) * m_HeightCurve.Evaluate(Data.m_AnimAlpha));

            Data.m_Target.position   = NewPosition;
            Data.m_Target.rotation   = Quaternion.Lerp(Data.m_AnimStartRotation, Data.m_Rotation, Data.m_AnimAlpha);
            Data.m_Target.localScale = Data.m_Scale * Data.m_AnimAlpha;

            if (Data.m_AnimAlpha >= 1.0f)
            {
                Data.m_Target.position   = Data.m_Position;
                Data.m_Target.rotation   = Data.m_Rotation;
                Data.m_Target.localScale = Data.m_Scale;
                m_Animatings.RemoveAt(i);
                continue;
            }

            m_Animatings[i] = Data;
        }
    }

    private void Update()
    {
        UpdateAnimations();

        float NewHeight = m_Tracking.position.y;
        if(NewHeight <= m_LastHeight)
            return;
        m_LastHeight = NewHeight;

        StartNewAnimations();
    }

    struct AnimData
    {
        public Transform  m_Target;
        public Vector3    m_Position;
        public Quaternion m_Rotation;
        public Vector3    m_Scale;

        public Vector3 m_AnimStartPos;
        public Quaternion m_AnimStartRotation;
        public float   m_AnimSpeed;
        public float   m_AnimAlpha;

        public AnimData(Transform Target)
        {
            m_Target   = Target;
            m_Position = Target.position;
            m_Rotation = Target.rotation;
            m_Scale    = Target.localScale;

            m_AnimStartPos = Vector3.zero;
            m_AnimStartRotation = Quaternion.identity;
            m_AnimSpeed    = 0;
            m_AnimAlpha    = 0;
        }
    }
}
