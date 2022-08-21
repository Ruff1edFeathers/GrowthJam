using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPlatform : MonoBehaviour
{
    private static List<SlidingPlatform> s_Platforms = new List<SlidingPlatform>();

    public static void UpdatePlatforms()
    {
        int Len = s_Platforms.Count;
        for (int i = 0; i < Len; i++)
        {
            s_Platforms[i].OnUpdate();
        }
    }

    public float     m_SlideAmount;
    public float     m_SlideSpeed;
    public float     m_SlideDelayOffset;
    public float     m_SlideDelay;
    public Transform m_Target;

    private bool  m_SlideIn;
    private float m_Timer;
    private float m_Alpha = 0.0f;

    private void OnEnable()
    {
        s_Platforms.Add(this);
        m_Timer = m_SlideDelayOffset % (m_SlideDelay * 2);
    }

    private void OnDisable()
    {
        s_Platforms.Remove(this);
    }

    private void OnUpdate()
    {
        if(m_Timer > 0.0f)
        {
            m_Timer -= Time.deltaTime;
            return;
        }

        m_Alpha += m_SlideSpeed * Time.deltaTime;

        Vector3 Start  = new Vector3(0, 0, m_SlideIn ? 0 : m_SlideAmount);
        Vector3 Target = new Vector3(0, 0, m_SlideIn ? m_SlideAmount : 0);

        m_Target.localPosition = Vector3.Lerp(Start, Target, m_Alpha);

        if(m_Alpha > 1f)
        {
            m_SlideIn = !m_SlideIn;
            m_Alpha   = 0.0f;
            m_Timer   = m_SlideDelay;
        }
    }
}
