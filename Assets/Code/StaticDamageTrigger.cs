using UnityEngine;

public class StaticDamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController Controller = other.GetComponentInParent<PlayerController>();
        if (Controller == null)
            return;

        Controller.TakeDamage();
    }
}
