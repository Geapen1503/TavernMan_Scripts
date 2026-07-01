using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashingFloorManager : MonoBehaviour
{
    [System.Serializable]
    public struct MissionMapping
    {
        public WashingFloorMissionSO mission;
        public List<DirtyFloorOverlay> dirtyOverlays;
    }

    public GameObject mop;

    [SerializeField] private List<MissionMapping> mappings;
    [SerializeField] private MopController mopController;
    public MopController MopController => mopController;

    private WashingFloorMissionSO _currentMission;

    public static WashingFloorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetCurrentMission(WashingFloorMissionSO mission)
    {
        _currentMission = mission;
    }

    public void ClearCurrentMission()
    {
        _currentMission = null;
    }

    public void CheckCurrentMissionProgress()
    {
        if (_currentMission != null) _currentMission.CheckGlobalCompletion();
    }

    public List<DirtyFloorOverlay> GetCurrentMissionOverlays()
    {
        if (_currentMission == null) return null;

        return GetOverlaysForMission(_currentMission);
    }

    public List<DirtyFloorOverlay> GetOverlaysForMission(WashingFloorMissionSO mission)
    {
        foreach (var map in mappings) if (map.mission == mission) return map.dirtyOverlays;

        return null;
    }
}