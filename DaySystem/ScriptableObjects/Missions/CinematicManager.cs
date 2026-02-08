using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicManager : MonoBehaviour
{
    [System.Serializable]
    public struct CinematicMapping
    {
        public CinematicID id;
        public PlayableDirector director;
    }

    public static CinematicManager Instance { get; private set; }
    [SerializeField] private List<CinematicMapping> mappings;

    public PlayableDirector GetDirector(CinematicID id)
    {
        var mapping = mappings.Find(m => m.id == id);
        return mapping.director;
    }

    private void Awake() => Instance = this;
}