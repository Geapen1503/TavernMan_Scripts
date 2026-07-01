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

        _activeOverlays = new List<DirtyFloorOverlay>();

        foreach (var overlay in overlays)
        {
            if (overlay == null) continue;

            overlay.gameObject.SetActive(true);
            overlay.InitializeOverlay();

            overlay.CompletionThreshold = 2f;

            _activeOverlays.Add(overlay);
        }

        var mopController = WashingFloorManager.Instance.MopController;
        var mop = WashingFloorManager.Instance.mop;
        if (mopController != null && mop != null)
        {
            mop.SetActive(true);
            mopController.Activate(_activeOverlays);
        }
    }

    public void CheckGlobalCompletion()
    {
        if (_activeOverlays == null || _activeOverlays.Count == 0) return;

        float totalProgress = 0f;
        foreach (var overlay in _activeOverlays) totalProgress += overlay.CleanedPercentage;

        float averageProgress = totalProgress / _activeOverlays.Count;

        if (averageProgress >= completionThreshold) FinalizeMission();
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
            overlay.gameObject.SetActive(false);
        }
        _activeOverlays = null;

        CompleteMission();
    }
}
