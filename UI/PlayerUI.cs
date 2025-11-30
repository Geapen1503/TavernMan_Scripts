using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressKeyText;
    public TextMeshProUGUI fpsText;

    [Header("Settings")]
    [SerializeField] private bool fpsTextEnabled = true;

    private float pollingTime = 1f;
    private float time;
    private int frameCount;

    private bool previousFpsState;

    public static PlayerUI Instance { get; private set; }

    private void Awake()
    {
        if (pressKeyText != null) pressKeyText.text = string.Empty;

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

    public void ShowPressKey(string message)
    {
        if (pressKeyText != null) pressKeyText.text = message;
    }

    public void HidePressKey()
    {
        if (pressKeyText != null) pressKeyText.text = string.Empty;
    }

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
