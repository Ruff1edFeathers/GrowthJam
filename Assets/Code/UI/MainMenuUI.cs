using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public bool m_State { get; private set; }

    public void SetState(bool Visible)
    {
        m_State = Visible;

        gameObject.SetActive(m_State);

        if (m_State) Time.timeScale = 0.0f;
        else Time.timeScale = 1.0f;
    }
}
