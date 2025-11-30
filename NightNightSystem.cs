using UnityEngine;

public class NightNightSystem : MonoBehaviour
{
    public float rotationSpeed = 10f; 
    private bool reverse = false; 

    void Update()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;

        if (transform.rotation.eulerAngles.x <= 240f && !reverse) reverse = true; // 240° equals -120° in Unity
        else if (transform.rotation.eulerAngles.x >= 360f && reverse) reverse = false;

        float direction = reverse ? 1f : -1f;
        transform.Rotate(Vector3.right, direction * rotationStep);
    }
}
