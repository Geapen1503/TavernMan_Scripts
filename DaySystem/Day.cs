using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day : MonoBehaviour
{
    public static Day Instance { get; private set; }

    public DayData data;
    public static System.Action<NPCID> OnAnyDialogueEnded;
    public static System.Action<NPCID> OnAnyNPCServed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartDay()
    {
        StartCoroutine(StartDayRoutine());
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
        foreach (MissionSO mission in data.missions)
        {
            if (mission.missionState == MissionState.InProgress && mission is TalkToNPCMissionSO talkMission)
            {
                if (talkMission.targetNpc == npcID) return talkMission.npcDialogue;
            }
        }

        // Default dialogue for the day
        return data.dialogues.Find(d => d.npc == npcID);
    }
    public void NotifyDialogueEnded(NPCID npcID)
    {
        OnAnyDialogueEnded?.Invoke(npcID);
    }

    public void NotifyNPCServed(NPCID npcID)
    {
        OnAnyNPCServed?.Invoke(npcID);
    }

    public DayID GetDayId() { return data.dayID; }
}
