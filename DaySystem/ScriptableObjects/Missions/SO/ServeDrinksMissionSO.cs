using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Missions/Serve Drinks To NPC")]
public class ServeDrinksMissionSO : MissionSO
{
    public DrinkType drinkType;
    public List<NPCID> npcsToServe;
    public ServeDialoguesLine[] serveDialoguesLines; 

    private List<NPCID> remainingNpcs;

    protected override void MissionContentPlaying()
    {
        remainingNpcs = new List<NPCID>(npcsToServe);

        foreach (NPCID id in npcsToServe)
        {
            NPC target = GameStateManager.Instance.GetNPC(id);
            if (target != null)
            {
                target.isServeable = true;
                target.RefreshInteractionState();
            }
        }

        Day.OnAnyNPCServed += CheckIfMissionComplete;
    }

    private void CheckIfMissionComplete(NPCID id)
    {
        if (remainingNpcs.Contains(id))
        {
            remainingNpcs.Remove(id);

            NPC target = GameStateManager.Instance.GetNPC(id);
            if (target != null)
            {
                target.isServeable = false;
                target.RefreshInteractionState();
            }

            Debug.Log($"Mission : {id} served. Remaining: {remainingNpcs.Count}");
        }

        if (remainingNpcs.Count == 0)
        {
            Day.OnAnyNPCServed -= CheckIfMissionComplete;
            CompleteMission();
        }
    }
}

public struct ServeDialoguesLine
{
    public string text;
    public float duration;
}