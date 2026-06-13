using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EndingDialogueStep
{
    [TextArea] public string npcDialogueText;
    public string buttonText;

    public bool hasMonologue;
    public DialogueLine monologueLine;
}

[CreateAssetMenu(menuName = "Game/Ending Dialogue Sequence")]
public class EndingChairsDialogueSO : ScriptableObject
{
    public List<EndingDialogueStep> sequence;
}