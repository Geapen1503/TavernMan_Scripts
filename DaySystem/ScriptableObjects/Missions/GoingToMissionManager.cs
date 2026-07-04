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
        public GameObject optionalZoneIndicator;
    }

    // Note to myself: if you want to add particles or sound to guide the player to the target,
    // You can simply put that col in another struct with the thing you want to add (particles?)
    // Then you call the struct instead of the Col

    // Edit 04/07/2026: nope, that's shite, it would have reset all my trigger colls, I ain't doing that, so
    // I just added a GameObject beside it

    public List<MissionMapping> mappings;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool TryGetMissionData(GoingToMissionSO mission, out Collider targetCollider, out GameObject indicator)
    {
        foreach (MissionMapping mapping in mappings)
        {
            if (mapping.mission == mission)
            {
                targetCollider = mapping.targetZoneCollider;
                indicator = mapping.optionalZoneIndicator;
                return true; 
            }
        }

        targetCollider = null;
        indicator = null;
        return false;
    }
}
