using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;


[CreateAssetMenu(menuName = "Game/Missions/WashingFloor")]
public class WashingFloorMissionSO : MissionSO
{
    [Range(0f, 1f)]
    [SerializeField] private float completionThreshold = 0.6f;

    private List<DirtyFloorOverlay> _activeOverlays;
    private int _totalTasks;
    private int _completedTasks;

    protected override void MissionContentPlaying()
    {
        WashingFloorManager.Instance.SetCurrentMission(this);
        var overlays = WashingFloorManager.Instance.GetCurrentMissionOverlays();
        if (overlays == null || overlays.Count == 0) return;

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

        var mopController = WashingFloorManager.Instance.MopController;
        var mop = WashingFloorManager.Instance.mop;
        if (mopController != null && mop != null)
        {
            mop.SetActive(true);
            mopController.Activate(_activeOverlays);
        }
    }

    private void OnOverlayCompleted(DirtyFloorOverlay overlay)
    {
        _completedTasks++;
        if (_completedTasks >= _totalTasks) FinalizeMission();
    }

    private void FinalizeMission()
    {
        var mopController = WashingFloorManager.Instance.MopController;
        var mop = WashingFloorManager.Instance.mop;
        if (mopController != null && mop != null)
        {
            mopController.Deactivate();
            mop.SetActive(false);
        }

        WashingFloorManager.Instance.ClearCurrentMission();

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
