using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashingFloorManager : MonoBehaviour
{
    public static WashingFloorManager Instance { get; private set; }

    [System.Serializable]
    public struct MissionMapping
    {
        public WashingFloorMissionSO mission;
        public List<DirtyFloorOverlay> dirtyOverlays;
    }

    [SerializeField] private List<MissionMapping> mappings;
    [SerializeField] private MopController mopController;
    public MopController MopController => mopController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public List<DirtyFloorOverlay> GetOverlaysForMission(WashingFloorMissionSO mission)
    {
        foreach (var map in mappings) if (map.mission == mission) return map.dirtyOverlays;

        return null;
    }
}