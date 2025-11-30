using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public Transform head; 
    public Transform playerCameraRoot;
    public float smoothing = 5f; 

    void LateUpdate()
    {
        if (head != null && playerCameraRoot != null) playerCameraRoot.position = Vector3.Lerp(playerCameraRoot.position, head.position, smoothing * Time.deltaTime);
    }
}
