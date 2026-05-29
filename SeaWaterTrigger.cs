using UnityEngine;

public class SeaWaterTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GrabbableObject"))
        {
            if (MoveObjectsMissionManager.Instance != null)
            {
                MoveObjectsMissionManager.Instance.ResetObjectFromWater(other.gameObject);
            }
        }
    }
}