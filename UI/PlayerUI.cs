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

    private Queue<string> dialogueQueue = new Queue<string>();
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
    public void InjectDialogueToTavernMan(string dialogue)
    {
        dialogueQueue.Enqueue(dialogue);

        if (!isProcessingDialogue) StartCoroutine(ProcessDialogueQueue());
    }

    public void InjectSequenceToTavernMan(string[] sequence)
    {
        foreach (string line in sequence) dialogueQueue.Enqueue(line);
        if (!isProcessingDialogue) StartCoroutine(ProcessDialogueQueue());
    }

    private IEnumerator ProcessDialogueQueue()
    {
        isProcessingDialogue = true;

        while (dialogueQueue.Count > 0)
        {
            string currentLine = dialogueQueue.Dequeue();
            float duration = baseDelay + (currentLine.Length * timePerChar);

            tavernManDialogue.text = "T : " + currentLine;

            yield return new WaitForSeconds(duration);
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
