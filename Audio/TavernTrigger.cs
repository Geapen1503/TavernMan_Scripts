using UnityEngine;

public class TavernTrigger : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (player != null && other.gameObject == player) AudioManager.Instance.EnterTavern();
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && other.gameObject == player) AudioManager.Instance.ExitTavern();
    }
}
