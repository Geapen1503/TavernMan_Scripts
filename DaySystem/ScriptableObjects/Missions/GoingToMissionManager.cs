using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingToMissionManager : MonoBehaviour
{
    public static GoingToMissionManager Instance { get; private set; }

    [System.Serializable]
    public struct MissionMapping
    {
        public GoingToMissionSO mission;
        public Collider targetZoneCollider; 
    }

    // Note to myself: if you want to add particles or sound to guide the player to the target,
    // You can simply put that col in another struct with the thing you want to add (particles?)
    // Then you call the struct instead of the Col

    public List<MissionMapping> mappings;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Collider GetColliderForMission(GoingToMissionSO mission)
    {
        foreach (MissionMapping mapping in mappings)
        {
            if (mapping.mission == mission) return mapping.targetZoneCollider;
        }

        return null;
    }
}
