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
    private NPCID currentInteractingNPC;

    private List<EndingDialogueStep> endingSequence;
    private bool isEndingSequence = false;
    private bool isMonologueCooldown = false;

    private List<FinalEndingDialogueStep> finalEndingSequence;
    private System.Func<NPCID, DialogueAnchor> finalEndingAnchorLookup;
    private bool isFinalEndingSequence = false;


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

    public void StartDialogueSequence(List<DialogueEntry> entries, NPCID npcID)
    {
        if (entries == null || entries.Count == 0) return;

        IsInDialogue = true;

        currentInteractingNPC = npcID;
        dialogueSequence = entries;
        currentDialogueIndex = 0;

        DisplayCanvas();
        ShowDialogue(currentDialogueIndex);
    }

    public void StartEndingDialogueSequence(List<EndingDialogueStep> entries, NPCID npcID)
    {
        if (entries == null || entries.Count == 0) return;

        isEndingSequence = true;
        isMonologueCooldown = false;
        IsInDialogue = true;
        currentInteractingNPC = npcID;
        endingSequence = entries;
        currentDialogueIndex = 0;

        DisplayCanvas();
        ShowEndingDialogue(currentDialogueIndex);
    }

    public void StartFinalEndingDialogueSequence(List<FinalEndingDialogueStep> entries, System.Func<NPCID, DialogueAnchor> anchorLookup)
    {
        if (entries == null || entries.Count == 0) return;

        isFinalEndingSequence = true;
        isEndingSequence = false;
        isMonologueCooldown = false;
        IsInDialogue = true;
        finalEndingSequence = entries;
        finalEndingAnchorLookup = anchorLookup;
        currentDialogueIndex = 0;

        DisplayCanvas();
        ShowFinalEndingDialogue(currentDialogueIndex);
    }

    private void ShowDialogue(int index)
    {
        if (index < 0 || index >= dialogueSequence.Count) return;

        DialogueEntry entry = dialogueSequence[index];
        dialogueTextBox.text = entry.dialogueText;
        buttonLabel.text = entry.buttonText;

        if (nextButton != null) nextButton.gameObject.SetActive(true);
    }

    private void ShowEndingDialogue(int index)
    {
        EndingDialogueStep entry = endingSequence[index];
        dialogueTextBox.text = entry.npcDialogueText;
        buttonLabel.text = entry.buttonText;

        if (nextButton != null) nextButton.gameObject.SetActive(true);
    }

    private void ShowFinalEndingDialogue(int index)
    {
        if (index < 0 || index >= finalEndingSequence.Count) return;

        FinalEndingDialogueStep entry = finalEndingSequence[index];

        if (finalEndingAnchorLookup != null)
        {
            DialogueAnchor anchor = finalEndingAnchorLookup(entry.speakerNPC);
            if (anchor != null) MoveCanvasToNPC(anchor);
        }

        dialogueTextBox.text = entry.npcDialogueText;
        buttonLabel.text = entry.buttonText;

        if (nextButton != null) nextButton.gameObject.SetActive(true);
    }

    private void NextDialogue()
    {
        if (isMonologueCooldown) return;

        if (isFinalEndingSequence)
        {
            StartCoroutine(HandleFinalEndingMonologueTransition());
        }
        else if (isEndingSequence)
        {
            StartCoroutine(HandleEndingMonologueTransition());
        }
        else
        {
            currentDialogueIndex++;
         
            if (currentDialogueIndex < dialogueSequence.Count) ShowDialogue(currentDialogueIndex);
            else EndDialogueSequence();
        }
    }

    private void EndDialogueSequence()
    {
        IsInDialogue = false;
        isFinalEndingSequence = false;
        isEndingSequence = false;
        finalEndingSequence = null;
        finalEndingAnchorLookup = null;

        HideCanvas();
        dialogueSequence = null;
        currentDialogueIndex = 0;

        var playerCam = FPCam.Instance;
        if (playerCam != null) playerCam.ExitDialogueMode();

        var playerInput = Invector.vCharacterController.vThirdPersonInput.Instance;
        if (playerInput != null) playerInput.UnfreezeInputs();
        
        if (Day.Instance != null) Day.Instance.NotifyDialogueEnded(currentInteractingNPC);
    }

    private IEnumerator HandleEndingMonologueTransition()
    {
        isMonologueCooldown = true;

        if (nextButton != null) nextButton.gameObject.SetActive(false);

        EndingDialogueStep step = endingSequence[currentDialogueIndex];

        if (step.hasMonologue && PlayerUI.Instance != null)
        {
            DialogueLine[] monoArray = new DialogueLine[] { step.monologueLine };
            PlayerUI.Instance.InjectSequenceToTavernMan(monoArray, 0f);

            float waitTime = step.monologueLine.duration + step.monologueLine.pauseAfterDuration;
            yield return new WaitForSeconds(waitTime);
        }

        isMonologueCooldown = false;
        currentDialogueIndex++;

        if (currentDialogueIndex < endingSequence.Count) ShowEndingDialogue(currentDialogueIndex);
        else EndDialogueSequence();
    }

    private IEnumerator HandleFinalEndingMonologueTransition()
    {
        isMonologueCooldown = true;

        if (nextButton != null) nextButton.gameObject.SetActive(false);

        FinalEndingDialogueStep step = finalEndingSequence[currentDialogueIndex];

        if (step.hasMonologue && PlayerUI.Instance != null)
        {
            DialogueLine[] monoArray = new DialogueLine[] { step.monologueLine };
            PlayerUI.Instance.InjectSequenceToTavernMan(monoArray, 0f);

            float waitTime = step.monologueLine.duration + step.monologueLine.pauseAfterDuration;
            yield return new WaitForSeconds(waitTime);
        }

        isMonologueCooldown = false;
        currentDialogueIndex++;

        if (currentDialogueIndex < finalEndingSequence.Count) ShowFinalEndingDialogue(currentDialogueIndex);
        else EndDialogueSequence();
    }

    public bool IsInDialogue { get; private set; }
}
