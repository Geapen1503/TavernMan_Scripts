using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectsMissionManager : MonoBehaviour
{
    public static MoveObjectsMissionManager Instance { get; private set; }


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
