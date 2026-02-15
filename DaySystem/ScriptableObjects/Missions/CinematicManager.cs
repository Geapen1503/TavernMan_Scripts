using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicManager : MonoBehaviour
{
    [System.Serializable]
    public struct CinematicMapping
    {
        public CinematicMissionSO mission;
        public PlayableDirector director;
    }

    public static CinematicManager Instance { get; private set; }
    [SerializeField] private List<CinematicMapping> mappings;

    public PlayableDirector GetDirector(CinematicMissionSO mission)
    {
        var mapping = mappings.Find(m => m.mission == mission);
        return mapping.director;
    }

    private void Awake() => Instance = this;
}