using System.Collections.Generic;
using UnityEngine;

public class AnimatePlants : MonoBehaviour
{
    public Transform m_Target;
    public float m_GrowSpeed_Min = 1.5f;
    public float m_GrowSpeed_Max = 2f;
    public float m_DesiredDistance = 0.5f;

    private List<Transform> m_PlantGroups = new List<Transform>();
    private List<AnimData>  m_Animating   = new List<AnimData>();

    public void OnSetup()
    {
        int Len = m_Target.childCount;
        for (int i = 0; i < Len; i++)
        {
            Transform Child = m_Target.GetChild(i);
            Child.gameObject.SetActive(false);
            m_PlantGroups.Add(Child);
        }
    }

    public void OnUpdate(PlayerController Controller)
    {
        for (int i = m_PlantGroups.Count - 1; i >= 0; i--)
        {
            Transform Group = m_PlantGroups[i];
            if (Vector3.Distance(Group.position, Controller.m_Position) < m_DesiredDistance)
            {
                Group.gameObject.SetActive(true);

                int Len = Group.childCount;
                for (int c = 0; c < Len; c++)
                {
                    AnimData Data = new AnimData(Group.GetChild(c));
                    Data.m_Target.localScale = Vector3.zero;
                    Data.m_AnimSpeed = Random.Range(m_GrowSpeed_Min, m_GrowSpeed_Max);

                    m_Animating.Add(Data);
                }
                m_PlantGroups.RemoveAt(i);
            }
        }

        for (int i = m_Animating.Count - 1; i >= 0; i--)
        {
            AnimData Data = m_Animating[i];
            Data.m_AnimAlpha += Data.m_AnimSpeed * Time.deltaTime;

            Data.m_Target.localScale = Data.m_Scale * Data.m_AnimAlpha;

            if(Data.m_AnimAlpha >= 1.0f)
            {
                Data.m_Target.localScale = Data.m_Scale;

                m_Animating.RemoveAt(i);
                continue;
            }

            m_Animating[i] = Data;
        }
    }

    struct AnimData
    {
        public Transform m_Target;
        public Vector3   m_Scale;

        public float m_AnimSpeed;
        public float m_AnimAlpha;

        public AnimData(Transform Target)
        {
            m_Target    = Target;
            m_Scale     = m_Target.localScale;

            m_AnimSpeed = 0;
            m_AnimAlpha = 0;
        }
    }
}
