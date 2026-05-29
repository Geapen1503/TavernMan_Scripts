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

    public override string GetDescription()
    {
        return string.Empty;
    }

    protected override void MissionContentPlaying()
    {
        PlayableDirector director = CinematicManager.Instance.GetDirector(this);

        if (director != null)
        {
            CinematicManager.Instance.StartCoroutine(PlayCinematicSmooth(director));
        }
    }

    private IEnumerator PlayCinematicSmooth(PlayableDirector director)
    {
        PlayerUI playerUI = PlayerUI.Instance;

        if (playerUI != null) playerUI.ShowCinematicOverlay();

        yield return null;

        if (playerUI != null)
        {
            playerUI.HideNarrator();
            playerUI.HidePressKey();
            playerUI.InjectSequenceToTavernMan(dialogueSequence, startDelay);
        }

        director.time = 0;
        director.Evaluate();

        yield return null; 

        director.Play();

        CinematicManager.Instance.StartCoroutine(EndMissionAfterDelay(director));

        yield return null;

        if (playerUI != null) playerUI.HideCinematicOverlay();
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