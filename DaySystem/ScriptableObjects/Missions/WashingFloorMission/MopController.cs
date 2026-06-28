using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class MopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform brushTip;
    [SerializeField] private Transform mopHolder;

    [Header("Cleaning")]
    [SerializeField] private Vector2 brushSize = new Vector2(0.1f, 0.3f);
    [SerializeField] private float cleaningInterval = 0.05f;

    [Header("Animation")]
    [SerializeField] private float swayAmount = 0.05f;
    [SerializeField] private float swaySpeed = 8f;

    [SerializeField] private float rotateAmount = 15f;
    [SerializeField] private float rotateSpeed = 12f;

    private bool _isActive;
    private float _timer;
    private List<DirtyFloorOverlay> _targetOverlays = new List<DirtyFloorOverlay>();

    public bool IsActive => _isActive;

    public static MopController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple MopController instances detected. Keeping the first one.");
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (!_isActive || brushTip == null) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = cleaningInterval;
            ApplyCleaning();
        }
    }

    private void ApplyCleaning()
    {
        if (!_isActive) return;
        if (!Input.GetKey(vThirdPersonInput.Instance.leftClickInput)) return;

        Vector3 pos = brushTip.position;
        foreach (var overlay in _targetOverlays)
        {
            if (overlay == null || !overlay.isActiveAndEnabled || overlay.IsCompleted) continue;

            Bounds bounds = overlay.MeshRenderer.bounds;

            if (bounds.Contains(new Vector3(pos.x, bounds.center.y, pos.z))) overlay.CleanArea(brushTip, brushSize);
        }
    }

    public void MoveMopTo(Vector3 targetPoint, Vector3 normal)
    {
        Vector3 offset = brushTip.position - transform.position;
        transform.position = targetPoint - offset;

        if (normal != Vector3.zero)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        }
    }

    public void ReturnToInitialPosition()
    {
        transform.position = mopHolder.position;
        transform.rotation = mopHolder.rotation;
    }

    public void AnimateMop()
    {
        if (!_isActive) return;

        float time = Time.time;

        float x = Mathf.Sin(time * swaySpeed) * swayAmount;
        float z = Mathf.Cos(time * swaySpeed * 0.8f) * swayAmount;

        gameObject.transform.localPosition = gameObject.transform.localPosition + new Vector3(x, 0f, z);

        float rot = Mathf.Sin(time * rotateSpeed) * rotateAmount;
        gameObject.transform.localRotation = gameObject.transform.localRotation * Quaternion.Euler(0f, rot, rot * 0.5f);
    }

    public void Activate(List<DirtyFloorOverlay> overlays)
    {
        _targetOverlays = (overlays != null) ? new List<DirtyFloorOverlay>(overlays) : new List<DirtyFloorOverlay>();
        _isActive = true;
        _timer = 0f;
    }

    public void Deactivate()
    {
        _isActive = false;
        _targetOverlays.Clear();
    }
}
