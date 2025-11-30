using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public Transform CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;
    [Tooltip("Rotation speed of the camera")]
    public float RotationSpeed = 1.0f;

    private float _cinemachineTargetPitch;
    private bool _isCurrentDeviceMouse = true;

    [Header("Input Settings")]
    public string rotateCameraXInput = "Mouse X";
    public string rotateCameraYInput = "Mouse Y";

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        float lookX = Input.GetAxis(rotateCameraXInput);
        float lookY = Input.GetAxis(rotateCameraYInput);

        if (Mathf.Abs(lookX) > 0.01f || Mathf.Abs(lookY) > 0.01f)
        {
            float deltaTimeMultiplier = _isCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += lookY * RotationSpeed * deltaTimeMultiplier;
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            transform.Rotate(Vector3.up * lookX * RotationSpeed * deltaTimeMultiplier);
        }
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
