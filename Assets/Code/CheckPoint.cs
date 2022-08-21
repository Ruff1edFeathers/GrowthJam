using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject m_Mesh;
    public Animation m_Animator;

    private bool m_Trigged;

    private void Awake()
    {
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
    }
}
