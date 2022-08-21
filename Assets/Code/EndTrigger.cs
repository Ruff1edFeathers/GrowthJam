using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController Controller = other.GetComponentInParent<PlayerController>();
        if (Controller == null)
            return;

        Controller.State = Controller.m_CheerState;

        GameManager.s_Instance.m_HUDUI.Hide();
        GameManager.s_Instance.m_EndUI.SetActive(true);
    }
}
