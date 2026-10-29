using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Final Ending Mission")]
public class FinalEndingMissionSO : MissionSO
{
    [Header("Dialogue Configuration")]
    public FinalEndingDialogueSO dialogueSO;

    protected override void MissionContentPlaying()
    {
        if (FinalEndingManager.Instance != null)
        {
            FinalEndingManager.Instance.StartFinalEnding(this);
        }
    }

    public void FinishMission()
    {
        CompleteMission();
    }
}
