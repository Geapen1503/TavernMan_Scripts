using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectsMissionManager : MonoBehaviour
{
    public static MoveObjectsMissionManager Instance { get; private set; }

    // NOTE TO MYSELF AS OF 12/04/2026 11AM: I reused and reenabled our old grabbable system,
    // it works yes, but you need to enable it only when a MoveObject mission is on, I don't 
    // want to shout raycast everywhere and have Update methods executing if there's nothing to grab
    // Once your done with that moving system, disable the grab input from the Update method in the input controller
    // and disable CheckGrabbableObjectOutline(); from the Start method of FPCam, and also HighlightGameObject(); from 
    // the Update method of FPCam. Just keep it disabled always, and enable it when one of these Mission is on.


    [System.Serializable]
    public struct MoveTaskRefs
    {
        public GameObject objectInScene;
        public Collider targetZoneCollider;
    }

    [System.Serializable]
    public struct MissionMapping
    {
        public MoveObjectsMissionSO mission;
        public List<MoveTaskRefs> tasks;
    }

    public List<MissionMapping> mappings;

    private void Awake()
    {
        Instance = this;
    }

    public List<MoveTaskRefs> GetRefsForMission(MoveObjectsMissionSO mission)
    {
        foreach (MissionMapping mapping in mappings)
        {
            if (mapping.mission == mission) return mapping.tasks;
        }

        return null;
    }
}
