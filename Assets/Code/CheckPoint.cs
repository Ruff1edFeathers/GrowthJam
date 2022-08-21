using TMPro;

using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject m_Mesh;
    public Animation m_Animator;
    public TextMeshPro m_Text;
    public float m_WriteCharDelay = 0.25f;
    public float m_Delay = 0.5f;
    public float m_DeleteCharDelay = 0.1f;

    private bool m_Trigged;

    private void Awake()
    {
        m_Text.gameObject.SetActive(false);
        m_Mesh.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Trigged)
            return;

        PlayerController Controller = other.GetComponentInParent<PlayerController>();
        if (Controller == null)
            return;

        Controller.m_RespawnPoint = transform.position;

        m_Trigged = true;
        m_Mesh.SetActive(true);
        m_Animator.Play("CheckPointAnim");

        StartCoroutine(WriteOutText());
    }

    System.Collections.IEnumerator WriteOutText()
    {
        const string TargetStr = "Checkpoint Reached";
        int Len = TargetStr.Length;

        m_Text.gameObject.SetActive(true);
        for (int i = 0; i < Len; i++)
        {
            m_Text.text += TargetStr[i];
            yield return new WaitForSeconds(m_WriteCharDelay);
        }

        yield return new WaitForSeconds(m_Delay);

        for (int i = 0; i < Len; i++)
        {
            m_Text.text = m_Text.text.Remove(m_Text.text.Length - 1);
            yield return new WaitForSeconds(m_DeleteCharDelay);
        }
        m_Text.gameObject.SetActive(false);
    }
}
