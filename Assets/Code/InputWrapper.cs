using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PollingInput
{
    public abstract void Poll();
}

public class PollingAxis : PollingInput
{
    public float m_Value { get; private set; }

    public override void Poll()
    {

    }
}

public enum eButtonState
{
    None,
    Pressed,
    Held,
    Released
}

public class PollingButton : PollingInput
{
    public eButtonState m_Value { get; private set; }

    public override void Poll()
    {

    }
}

public class InputWrapper
{
    public static InputWrapper s_Instance { get; private set;}

    public InputWrapper(InputActionAsset Asset)
    {
        s_Instance = this;
    }

    public void OnUpdate()
    {

    }
}
