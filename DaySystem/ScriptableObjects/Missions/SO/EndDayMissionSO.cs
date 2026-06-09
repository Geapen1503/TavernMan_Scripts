using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/End Day Mission")]
public class EndDayMissionSO : MissionSO
{
    [Header("Dialogue Configuration")]
    public EndingChairsDialogueSO dialogueSO;

    protected override void MissionContentPlaying()
    {
        if (EndDayManager.Instance != null)
        {
            EndDayManager.Instance.StartChairSequence(this);
        }
    }

    public void FinishMission()
    {
        CompleteMission();
    }
}
