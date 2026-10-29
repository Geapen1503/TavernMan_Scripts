using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FinalEndingDialogueStep
{
    public NPCID speakerNPC;
    [TextArea(2, 5)] public string npcDialogueText;
    public string buttonText;

    public bool hasMonologue;
    public DialogueLine monologueLine;
}

[CreateAssetMenu(menuName = "Game/Final Ending Dialogue Sequence")]
public class FinalEndingDialogueSO : ScriptableObject
{
    public List<FinalEndingDialogueStep> sequence;
}
