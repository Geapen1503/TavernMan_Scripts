using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class DialogueEntry
{
    public string dialogueText;
    public string buttonText;

    public DialogueEntry(string dialogue, string button)
    {
        dialogueText = dialogue;
        buttonText = button;
    }
}

public class DialogueUIManager : MonoBehaviour
{
    public static DialogueUIManager Instance { get; private set; }

    [Header("Main canvas")]
    public Canvas mainCanvas;

    [Header("UI Elements")]
    public TMP_Text dialogueTextBox;
    public Button nextButton;
    public TMP_Text buttonLabel;

    private List<DialogueEntry> dialogueSequence;
    private int currentDialogueIndex = 0;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        HideCanvas();

        if (nextButton != null) nextButton.onClick.AddListener(NextDialogue);
    }

    public void MoveCanvasToNPC(DialogueAnchor dialogueAnchor)
    {
        mainCanvas.transform.position = dialogueAnchor.transform.position;
        mainCanvas.transform.rotation = dialogueAnchor.transform.rotation;

        DisplayCanvas();
    }

    public void DisplayCanvas()
    {
        if (mainCanvas != null && mainCanvas.enabled == false) mainCanvas.enabled = true;
    }

    public void HideCanvas()
    {
        if (mainCanvas != null && mainCanvas.enabled == true) mainCanvas.enabled = false;
    }

    public void StartDialogueSequence(List<DialogueEntry> entries)
    {
        if (entries == null || entries.Count == 0) return;

        dialogueSequence = entries;
        currentDialogueIndex = 0;

        DisplayCanvas();
        ShowDialogue(currentDialogueIndex);
    }

    private void ShowDialogue(int index)
    {
        if (index < 0 || index >= dialogueSequence.Count) return;

        DialogueEntry entry = dialogueSequence[index];
        dialogueTextBox.text = entry.dialogueText;
        buttonLabel.text = entry.buttonText;
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogueSequence.Count) ShowDialogue(currentDialogueIndex);
        else EndDialogueSequence();
    }

    private void EndDialogueSequence()
    {
        HideCanvas();
        dialogueSequence = null;
        currentDialogueIndex = 0;

        var playerCam = FPCam.Instance;
        if (playerCam != null) playerCam.ExitDialogueMode();

        var playerInput = Invector.vCharacterController.vThirdPersonInput.Instance;
        if (playerInput != null) playerInput.UnfreezeInputs();
    }
}
