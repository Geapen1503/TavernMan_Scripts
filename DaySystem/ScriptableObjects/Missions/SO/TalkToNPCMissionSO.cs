using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Talk To NPC")]
public class TalkToNPCMissionSO : MissionSO
{
    protected override void MissionContentPlaying()
    {
        Debug.Log("Damn nigaud we need to talk (coming from TalkToNPCMissionSO)");

        //OnTalkDone();
    }

    public void OnTalkDone()
    {
        CompleteMission();
    }
}
