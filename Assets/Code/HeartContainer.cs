using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController Controller = other.GetComponentInParent<PlayerController>();
        if (Controller == null)
            return;

        Controller.AddHeart();
        gameObject.SetActive(false);
    }
}
