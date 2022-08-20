using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PollingInput
{
    public InputAction m_Action;

    public PollingInput(InputAction Action)
    {
        m_Action = Action;
        Debug.Assert(m_Action != null);
    }

    public abstract void Poll();
}

public class PollingAxis : PollingInput
{
    public float m_Value { get; private set; }

    public PollingAxis(InputAction Action) : base(Action) { }

    public override void Poll()
    {
        m_Value = m_Action.ReadValue<float>();
    }

    public bool HasInput()
    {
        return Mathf.Abs(m_Value) > InputSystem.settings.defaultDeadzoneMin;
    }

    public static implicit operator float(PollingAxis Value)
    {
        return Value.m_Value;
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

    public PollingButton(InputAction Action) : base(Action) { }

    public override void Poll()
    {
        bool Pressed = m_Action.ReadValue<float>() > InputSystem.settings.defaultButtonPressPoint;
        switch(m_Value)
        {
            case eButtonState.None:
                m_Value = Pressed ? eButtonState.Pressed : eButtonState.None;
                break;
            case eButtonState.Pressed:
                m_Value = Pressed ? eButtonState.Held : eButtonState.Released;
                break;
            case eButtonState.Held:
                m_Value = Pressed ? eButtonState.Held : eButtonState.Released;
                break;
            case eButtonState.Released:
                m_Value = Pressed ? eButtonState.Pressed : eButtonState.None;
                break;
        }
    }

    public static implicit operator eButtonState(PollingButton Value)
    {
        return Value.m_Value;
    }
}

public enum eInputAction
{
    Movement,
    Jump,
    Pause
}

public static class eButtonStateExtension
{
    public static bool IsPressed(this eButtonState State) { return State == eButtonState.Pressed; }
    public static bool IsPressedOrHeld(this eButtonState State) { return State == eButtonState.Pressed || State == eButtonState.Held; }
    public static bool IsHeld(this eButtonState State) { return State == eButtonState.Held; }
    public static bool IsReleased(this eButtonState State) { return State == eButtonState.Released; }
}

public class InputWrapper
{
    public static InputWrapper s_Instance { get; private set;}

    public static PollingAxis GetAxis(eInputAction Action)
    {
        return (PollingAxis) s_Instance.m_Value[(int)Action];
    }

    public static PollingButton GetButton(eInputAction Action)
    {
        return (PollingButton) s_Instance.m_Value[(int)Action];
    }

    public static eButtonState GetButtonState(eInputAction Action)
    {
        return ((PollingButton)s_Instance.m_Value[(int)Action]).m_Value;
    }

    private PollingInput[] m_Value = new PollingInput[3];

    public InputWrapper(InputActionAsset Asset)
    {
        s_Instance = this;

        m_Value[(int)eInputAction.Movement] = new PollingAxis(Asset.FindAction("Movement"));
        m_Value[(int)eInputAction.Jump]     = new PollingButton(Asset.FindAction("Jump"));
        m_Value[(int)eInputAction.Pause]    = new PollingButton(Asset.FindAction("Pause"));

        Asset.Enable();
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
    }

    public void OnUpdate()
    {
        InputSystem.Update();

        int Len = m_Value.Length;
        for(int i = 0; i < Len; i++)
        {
            m_Value[i].Poll();
        }
    }
}
