using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager Instance { get; private set; }

    [System.Serializable]
    public struct DrinkTimeline
    {
        public DrinkType type;
        public PlayableAsset timeline;
    }

    public PlayableDirector director;
    public List<DrinkTimeline> drinkTimelines;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayServeAnimation(DrinkType type, System.Action onComplete)
    {
        var entry = drinkTimelines.Find(t => t.type == type);

        if (entry.timeline != null)
        {
            director.playableAsset = entry.timeline;
            director.Play();

            StartCoroutine(WaitUntilTimelineEnds(onComplete));
        }
        else
        {
            Debug.LogWarning($"No timeline found for {type}");
            onComplete?.Invoke();
        }
    }

    private System.Collections.IEnumerator WaitUntilTimelineEnds(System.Action onComplete)
    {
        yield return new WaitForSeconds((float)director.duration);
        onComplete?.Invoke();
    }
}
