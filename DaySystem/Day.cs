using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day : MonoBehaviour
{
    public DayData data;

    public void StartDay(DayID dayID)
    {

    }

    public DialogueSO GetDialogueForNPC(NPCID npcID)
    {
        return data.dialogues.Find(d => d.npc == npcID);
    }

    public DayID GetDayId() { return data.dayID; }
}
