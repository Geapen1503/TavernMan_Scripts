using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Dialogue Sequence")]
public class DialogueSO : ScriptableObject
{
    public NPCID npc;
    public List<DialogueEntry> sequence;
}
