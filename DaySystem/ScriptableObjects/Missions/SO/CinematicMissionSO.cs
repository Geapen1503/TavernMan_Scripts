using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[CreateAssetMenu(menuName = "Game/Missions/Cinematic")]
public class CinematicMissionSO : MissionSO
{
    [Header("Configuration")]
    public CinematicID cinematicKey; 
    public string[] dialogueSequence;

    protected override void MissionContentPlaying()
    {
        PlayableDirector director = CinematicManager.Instance.GetDirector(cinematicKey);

        if (director != null)
        {
            PlayerUI.Instance.InjectSequenceToTavernMan(dialogueSequence);
            director.Play();

            CinematicManager.Instance.StartCoroutine(EndMissionAfterDelay((float)director.duration));
        }
    }

    private IEnumerator EndMissionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CompleteMission();
    }
}
