using System.Collections.Generic;
using UnityEngine;

public class MopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform brushTip;

    [Header("Cleaning")]
    [SerializeField] private Vector2 brushSize = new Vector2(0.1f, 0.3f);
    [SerializeField] private float cleaningInterval = 0.05f;

    private bool _isActive;
    private float _timer;
    private List<DirtyFloorOverlay> _targetOverlays = new List<DirtyFloorOverlay>();

    public bool IsActive => _isActive;

    private void Update()
    {
        if (!_isActive || brushTip == null) return;
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer = cleaningInterval;

        ApplyCleaning();
    }

    private void ApplyCleaning()
    {
        Vector3 pos = brushTip.position;
        foreach (var overlay in _targetOverlays)
        {
            if (overlay == null || !overlay.isActiveAndEnabled || overlay.IsCompleted) continue;

            Bounds bounds = overlay.MeshRenderer.bounds;

            if (bounds.Contains(new Vector3(pos.x, bounds.center.y, pos.z))) overlay.CleanArea(brushTip, brushSize);
        }
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
