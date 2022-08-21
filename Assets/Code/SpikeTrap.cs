using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private static List<SpikeTrap> s_Traps = new List<SpikeTrap>();

    public static void UpdateTraps()
    {
        int Len = s_Traps.Count;
        for(int i = 0; i < Len; i++)
        {
            s_Traps[i].OnUpdate();
        }
    }

    public float m_TimerOffset;
    public float m_TimerDuration;
    public float m_ExtendDuration;
    public float m_Shake_Margin;
    public float m_Shake_Strength;

    public GameObject m_SpikesClosed;
    public GameObject m_SpikesExtended;

    private bool m_Extended;
    private float m_Timer;

    private void OnEnable()
    {
        s_Traps.Add(this);
        m_Timer = m_TimerOffset % m_TimerDuration;

        SetState(false);
    }

    private void OnDisable()
    {
        s_Traps.Remove(this);
    }

    private void SetState(bool Extended)
    {
        m_Extended = Extended;
        m_SpikesClosed.SetActive(!Extended);
        m_SpikesExtended.SetActive(Extended);
    }

    private void OnUpdate()
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer > 0.0f)
        {
            if(!m_Extended && m_Timer < m_Shake_Margin)
            {
                //Begin Shaking the model
                Vector3 ShakeOffset = Random.insideUnitSphere * m_Shake_Strength;
                ShakeOffset.y = 0;

                Debug.Log("Shake Offset: " + m_Timer);
                m_SpikesClosed.transform.localPosition = ShakeOffset;
            }

            return;
        }

        //Reset Local position
        m_SpikesClosed.transform.localPosition = Vector3.zero;

        SetState(!m_Extended);
        m_Timer = m_Extended ? m_ExtendDuration : m_TimerDuration;
    }
}
