using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;


[CreateAssetMenu(menuName = "Game/Missions/WashingFloor")]
public class WashingFloorMissionSO : MissionSO
{
    [Range(0f, 1f)]
    [SerializeField] private float completionThreshold = 0.85f;

    private List<DirtyFloorOverlay> _activeOverlays;
    private int _totalTasks;
    private int _completedTasks;

    protected override void MissionContentPlaying()
    {
        var overlays = WashingFloorManager.Instance.GetOverlaysForMission(this);
        if (overlays == null || overlays.Count == 0)
        {
            CompleteMission();
            return;
        }

        _totalTasks = 0;
        _completedTasks = 0;
        _activeOverlays = new List<DirtyFloorOverlay>();

        foreach (var overlay in overlays)
        {
            if (overlay == null) continue;

            overlay.gameObject.SetActive(true);
            overlay.InitializeOverlay();
            overlay.CompletionThreshold = completionThreshold;
            overlay.CleaningCompleted += OnOverlayCompleted;

            _activeOverlays.Add(overlay);
            _totalTasks++;
        }

        var mop = WashingFloorManager.Instance.MopController;
        if (mop != null) mop.Activate(_activeOverlays);
    }

    private void OnOverlayCompleted(DirtyFloorOverlay overlay)
    {
        _completedTasks++;
        if (_completedTasks >= _totalTasks) FinalizeMission();
    }

    private void FinalizeMission()
    {
        var mop = WashingFloorManager.Instance.MopController;
        if (mop != null) mop.Deactivate();

        foreach (var overlay in _activeOverlays)
        {
            if (overlay == null) continue;
            overlay.CleaningCompleted -= OnOverlayCompleted;
            overlay.gameObject.SetActive(false);
        }
        _activeOverlays = null;
        CompleteMission();
    }
}
