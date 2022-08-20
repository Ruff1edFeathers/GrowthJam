using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public bool m_State { get; private set; }

    public void OnSetup()
    {
        //Make sure its hidden
        SetState(false);
    }

    public void OnUpdate()
    {
        if (InputWrapper.GetButtonState(eInputAction.Pause).IsPressed())
            SetState(!m_State);
    }

    private void SetState(bool NewState)
    {
        m_State = NewState;
        gameObject.SetActive(m_State);

        //TODO: Might need to cache this
        if (m_State) Time.timeScale = 0.0f;
        else Time.timeScale = 1.0f;
    }
}
