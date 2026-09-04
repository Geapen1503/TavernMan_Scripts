using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Cinematic Move Objects")]
public class CinematicMoveObjectsMissionSO : MoveObjectsMissionSO
{
    protected override void MissionContentPlaying()
    {
        List<MoveObjectsMissionManager.MoveTaskRefs> refs = MoveObjectsMissionManager.Instance.GetRefsForMission(this);

        if (refs != null)
        {
            foreach (MoveObjectsMissionManager.MoveTaskRefs task in refs)
            {
                if (task.objectInScene != null) task.objectInScene.SetActive(true);
                if (task.targetZoneCollider != null) task.targetZoneCollider.gameObject.SetActive(true);
            }
        }

        if (vThirdPersonInput.Instance != null)
        {
            vThirdPersonInput.Instance.canGrab = false;
        }
    }

    public void OnCinematicTriggerFinished(BoatCinematicTrigger trigger)
    {
        if (onMissionFinishedDialogues != null && onMissionFinishedDialogues.Length > 0)
        {
            string randomEndingDialogue = onMissionFinishedDialogues[Random.Range(0, onMissionFinishedDialogues.Length)];
            PlayerUI.Instance.InjectDialogueToTavernMan(randomEndingDialogue, 3.0f);
        }

        CompleteMission();
    }
}