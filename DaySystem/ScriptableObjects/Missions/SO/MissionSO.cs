using System.Collections.Generic;
using UnityEngine;

public abstract class MissionSO : ScriptableObject
{
    [HideInInspector] public MissionState missionState = MissionState.NotStarted;

    public void StartMission()
    {
        if (missionState != MissionState.NotStarted) return;

        missionState = MissionState.InProgress;

        MissionContentPlaying();
    }

    protected abstract void MissionContentPlaying();

    protected void CompleteMission()
    {
        missionState = MissionState.Completed;
    }
}
