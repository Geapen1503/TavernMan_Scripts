using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DayData")]
public class DayData : ScriptableObject
{
    public DayID dayID;
    public List<MissionSO> missions;
    public List<DialogueSO> dialogues;
}
