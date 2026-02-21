using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Talk To NPC")]
public class TalkToNPCMissionSO : MissionSO
{
    public NPCID targetNpc;
    public DialogueSO npcDialogue;

    protected override void MissionContentPlaying()
    {
        Day.OnAnyDialogueEnded += CheckIfMissionComplete;
        Debug.Log($"Mission Talk active pour {targetNpc}");
    }

    private void CheckIfMissionComplete(NPCID id)
    {
        if (id == targetNpc)
        {
            Day.OnAnyDialogueEnded -= CheckIfMissionComplete;
            CompleteMission();
            Debug.Log($"Mission Talk terminée avec {targetNpc}");
        }
    }

    public void OnTalkDone()
    {
        CompleteMission();
    }
}
