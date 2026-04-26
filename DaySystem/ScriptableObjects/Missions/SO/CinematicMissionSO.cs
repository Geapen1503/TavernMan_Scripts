using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[CreateAssetMenu(menuName = "Game/Missions/Cinematic")]
public class CinematicMissionSO : MissionSO
{
    [Header("Configuration")]
    public float startDelay = 0f;
    public DialogueLine[] dialogueSequence;

    protected override void MissionContentPlaying()
    {
        PlayableDirector director = CinematicManager.Instance.GetDirector(this);

        if (director != null)
        {
            PlayerUI.Instance.InjectSequenceToTavernMan(dialogueSequence, startDelay);
            director.Play();

            CinematicManager.Instance.StartCoroutine(EndMissionAfterDelay(director));
        }
    }

    private IEnumerator EndMissionAfterDelay(PlayableDirector director)
    {
        yield return new WaitForSeconds((float)director.duration);

        yield return null;

        CompleteMission();
    }
}

[System.Serializable]
public struct DialogueLine
{
    public string text;
    public float duration;
    public float pauseAfterDuration;
}