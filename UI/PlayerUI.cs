using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressKeyText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI tavernManDialogue;
    public TextMeshProUGUI narratorText;

    [Header("Loading Screen References")]
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingBar;

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

    private bool isLoadingScreenVisible = false;
    private Coroutine loadingRoutine;

    public static PlayerUI Instance { get; private set; }

    private void Awake()
    {
        if (pressKeyText != null) pressKeyText.text = string.Empty;
        if (tavernManDialogue != null) tavernManDialogue.text = string.Empty;
        if (narratorText != null) narratorText.text = string.Empty;

        ApplyFpsState();

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PlayerUI instances detected. Keeping the first one.");
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (loadingScreen != null) loadingScreen.SetActive(false);
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


    /* Narrator Key methods */

    public void ShowNarrator(string message)
    {
        if (narratorText != null) narratorText.text = message;
    }

    public void HideNarrator()
    {
        if (narratorText != null) narratorText.text = string.Empty;
    }

    public bool IsNarratorActive()
    {
        if (narratorText == null || narratorText.text == string.Empty) return false;
        else return true;
    }

    /* Loading screen methods */

    public IEnumerator LoadSceneWithLoadingScreen(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        loadingBar.value = 0f;

        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            yield return null;
        }

        loadingBar.value = 1f;

        yield return new WaitForSecondsRealtime(0.1f);

        operation.allowSceneActivation = true;

        while (!operation.isDone) yield return null;

        loadingScreen.SetActive(false);
    }

    public void ShowCinematicOverlay()
    {
        if (loadingScreen != null) loadingScreen.SetActive(true);

        if (loadingBar != null) loadingBar.gameObject.SetActive(false);
    }

    public void HideCinematicOverlay()
    {
        if (loadingScreen != null) loadingScreen.SetActive(false);
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
