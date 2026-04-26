using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressKeyText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI tavernManDialogue;

    [Header("Dialogue Settings")]
    [SerializeField] private float baseDelay = 1.0f;
    [SerializeField] private float timePerChar = 0.05f;

    [Header("Settings")]
    [SerializeField] private bool fpsTextEnabled = true;

    private float pollingTime = 1f;
    private float time;
    private int frameCount;
    private bool previousFpsState;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isProcessingDialogue = false;

    public static PlayerUI Instance { get; private set; }

    private void Awake()
    {
        if (pressKeyText != null) pressKeyText.text = string.Empty;
        if (tavernManDialogue != null) tavernManDialogue.text = string.Empty;

        ApplyFpsState();

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PlayerUI instances detected. Keeping the first one.");
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (fpsTextEnabled != previousFpsState) ApplyFpsState();
        if (!fpsTextEnabled) return;

        FPSCounter();
    }

    /* Tavern Man Dialogues methods */
    public void InjectDialogueToTavernMan(string dialogue, float customDuration = 0f, float customPause = 0f)
    {
        DialogueLine newLine = new DialogueLine{ text = dialogue, duration = customDuration, pauseAfterDuration = customPause };
        dialogueQueue.Enqueue(newLine);

        if (!isProcessingDialogue) StartCoroutine(ProcessDialogueQueue());
    }

    public void InjectSequenceToTavernMan(DialogueLine[] sequence, float initialDelay = 0f)
    {
        foreach (DialogueLine line in sequence) dialogueQueue.Enqueue(line);
        if (!isProcessingDialogue) StartCoroutine(ProcessDialogueQueue(initialDelay));
    }

    private IEnumerator ProcessDialogueQueue(float delayBeforeStart = 0f)
    {
        isProcessingDialogue = true;

        if (delayBeforeStart > 0) yield return new WaitForSeconds(delayBeforeStart);

        while (dialogueQueue.Count > 0)
        {
            DialogueLine currentLine = dialogueQueue.Dequeue();

            float finalDuration = currentLine.duration > 0
                ? currentLine.duration
                : baseDelay + (currentLine.text.Length * timePerChar);

            // Display
            tavernManDialogue.text = "T : " + currentLine.text;
            yield return new WaitForSeconds(finalDuration);

            // And breath (in the air)
            tavernManDialogue.text = string.Empty;
            float totalPause = 0.3f + currentLine.pauseAfterDuration;
            yield return new WaitForSeconds(totalPause);
        }

        tavernManDialogue.text = string.Empty;
        isProcessingDialogue = false;
    }

    public void ClearDialogues()
    {
        dialogueQueue.Clear();
        StopCoroutine(ProcessDialogueQueue());
        isProcessingDialogue = false;
        tavernManDialogue.text = string.Empty;
    }

    /* Press Key methods */
    public void ShowPressKey(string message)
    {
        if (pressKeyText != null) pressKeyText.text = message;
    }

    public void HidePressKey()
    {
        if (pressKeyText != null) pressKeyText.text = string.Empty;
    }


    /* FPS Counter methods */
    private void FPSCounter()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            fpsText.text = $"{frameRate} fps";

            time = 0f;
            frameCount = 0;
        }
    }

    private void ApplyFpsState()
    {
        previousFpsState = fpsTextEnabled;
        if (fpsText != null) fpsText.enabled = fpsTextEnabled;
    }

    public void ToggleFPSCounter()
    {
        fpsTextEnabled = !fpsTextEnabled;
        ApplyFpsState();
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
