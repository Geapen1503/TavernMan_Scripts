using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class Day : MonoBehaviour
{
    public static Day Instance { get; private set; }

    public DayData data;

    public static System.Action<NPCID> OnAnyDialogueEnded;
    public static System.Action<NPCID> OnAnyNPCServed;

    private Coroutine _dayRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void LoadDayData(DayData newDayData)
    {
        data = newDayData;
    }

    public void StartDay()
    {
        StopCurrentDay();
        ResetAllMissions();
        _dayRoutine = StartCoroutine(StartDayRoutine());
    }

    private void ResetAllMissions()
    {
        if (data == null || data.missions == null) return;

        foreach (MissionSO mission in data.missions)
        {
            if (mission != null) mission.ResetMission();
        }
    }

    public void StopCurrentDay()
    {
        if (_dayRoutine != null)
        {
            StopCoroutine(_dayRoutine);
            _dayRoutine = null;
        }
    }

    private IEnumerator StartDayRoutine()
    {
        if (data == null) yield break;

        StartCoroutine(DisplayDay1IntroAdvices());

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

        if (GameStateManager.Instance != null) GameStateManager.Instance.CompleteCurrentDay();
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

    public DrinkType GetCurrentServiceType()
    {
        foreach (MissionSO mission in data.missions)
        {
            if (mission.missionState == MissionState.InProgress && mission is ServeDrinksMissionSO serveMission)
            {
                return serveMission.drinkType;
            }
        }
        return DrinkType.Beer;
    }

    private IEnumerator DisplayDay1IntroAdvices()
    {
        if (data == null || data.dayID != DayID.Day1) yield break;

        while (true)
        {
            MissionSO activeMission = GetCurrentMission();

            if (activeMission != null)
            {
                if (activeMission is CinematicMissionSO) yield return null;
                else break;
            }
            else
            {
                yield return null;
            }
        }

        if (PlayerUI.Instance != null)
        {
            PlayerUI.Instance.InjectDialogueToTavernMan("If you're lost while playing the game, just press the " + vThirdPersonInput.Instance.narratorInput + " key", 10.0f);
        }
    }

    public string GetCurrentMissionDescription()
    {
        if (data == null || data.missions == null) return string.Empty;

        foreach (MissionSO mission in data.missions)
        {
            if (mission != null && mission.missionState == MissionState.InProgress) return mission.GetDescription();
        }

        return string.Empty;
    }

    public MissionSO GetCurrentMission()
    {
        if (data == null || data.missions == null)  return null;
        
        foreach (MissionSO mission in data.missions)
        {
            if (mission != null && mission.missionState == MissionState.InProgress) return mission;
        }

        return null;
    }

    public DayID GetDayId() { return data.dayID; }
}
