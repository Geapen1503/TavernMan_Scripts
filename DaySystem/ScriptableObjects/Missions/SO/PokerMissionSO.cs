using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Poker Mission")]
public class PokerMissionSO : MissionSO
{
    protected override void MissionContentPlaying()
    {
        if (PokerManager.Instance != null) PokerManager.Instance.StartPokerSequence(this);
    }

    public void FinishMission()
    {
        CompleteMission();
    }
}