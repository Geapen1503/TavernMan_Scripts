using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day : MonoBehaviour
{
    public DayData data;

    public void StartDay()
    {
        StartCoroutine(StartDayRoutine());
        // Note to myself: this is just a prototype, you ought to write a coroutine that
        // ask the MissionSO if the mission is over before we call the next mission.StartMission();
    }

    private IEnumerator StartDayRoutine()
    {
        foreach (MissionSO mission in data.missions)
        {
            if (mission.missionState != MissionState.NotStarted)
            {
                Debug.Log("Cannot start the mission because the MissionState is not 'NotStarted'");
                continue;
            }

            mission.StartMission();

            yield return new WaitUntil(() => mission.missionState == MissionState.Completed);
        }

        Debug.Log("Day finished!");
    }

    public DialogueSO GetDialogueForNPC(NPCID npcID)
    {
        return data.dialogues.Find(d => d.npc == npcID);
    }

    public DayID GetDayId() { return data.dayID; }
}
