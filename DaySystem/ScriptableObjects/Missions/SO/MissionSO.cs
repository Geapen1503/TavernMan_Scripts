using System.Collections.Generic;
using UnityEngine;

public abstract class MissionSO : ScriptableObject
{
    [HideInInspector] public MissionState missionState = MissionState.NotStarted;
    [TextArea(2, 5)] public string missionDescription = string.Empty;

    //private void OnEnable() // You might want to rethink that if you want to code a savegame system
    //{
    //    missionState = MissionState.NotStarted;
    //    // I've done it like that to prevent already played missions from not starting (especially cutscenes)
    //}

    public virtual string GetDescription()
    {
        return missionDescription;
    }

    public void ResetMission()
    {
        missionState = MissionState.NotStarted;
    }

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
