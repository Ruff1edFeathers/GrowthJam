using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : MonoBehaviour
{
    public static HUDUI s_Instance { get; private set; }

    public Texture2D m_HeartIcon;
    public Texture2D m_HeartBrokenIcon;

    public Color m_FilledColour;
    public Color m_BrokenColour;

    public RawImage m_HeartTemplate;

    private RawImage[] m_HealthPips;
    private int m_VisiblePipsLen;

    public void OnSetup()
    {
        s_Instance = this;

        //Hide the Template
        m_HeartTemplate.gameObject.SetActive(false);

        //Make sure HUD is hidden at startup
        SetState(false);
    }

    public void SetState(bool Visible)
    {
        gameObject.SetActive(Visible);
    }

    public void SetupHealth(int MaxHealth)
    {
        m_VisiblePipsLen = MaxHealth;
        System.Array.Resize(ref m_HealthPips, MaxHealth);

        int TotalLen = m_HealthPips.Length;
        for (int i = 0; i < TotalLen; i++)
        {
            if (m_HealthPips[i] == null)
            {
                GameObject    NewPip           = Instantiate(m_HeartTemplate.gameObject, m_HeartTemplate.transform.parent);
                RectTransform NewPip_Transform = (RectTransform)NewPip.transform;

                NewPip_Transform.anchoredPosition = new Vector2((i * NewPip_Transform.sizeDelta.x) + 5f, 0);

                m_HealthPips[i] = NewPip.GetComponent<RawImage>();
            }

            m_HealthPips[i].gameObject.SetActive(i < m_VisiblePipsLen);
        }
    }

    public void UpdateHealth(int Health)
    {
        Debug.Assert(Health <= m_VisiblePipsLen);

        for(int i = 0; i < m_VisiblePipsLen; i++)
        {
            m_HealthPips[i].texture = i <= Health - 1 ? m_HeartIcon : m_HeartBrokenIcon;
            m_HealthPips[i].color   = i <= Health - 1 ? m_FilledColour : m_BrokenColour;
        }
    }
}
