using System;
using System.IO;
using Invector.vCharacterController;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private string savePath;

    public GameSettingsData CurrentSettings { get; private set; }
    public GameSettingsData PendingSettings { get; private set; }

    public event Action OnSettingsApplied;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "gamesettings.json");
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                CurrentSettings = JsonUtility.FromJson<GameSettingsData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SettingsManager] Cannot load settings : {e.Message}");
                CreateDefaultSettings();
            }
        }
        else
        {
            CreateDefaultSettings();
        }

        PendingSettings = CloneSettings(CurrentSettings);
        ApplySettingsToSystem(CurrentSettings);
    }

    private void CreateDefaultSettings()
    {
        CurrentSettings = new GameSettingsData();

        CurrentSettings.resolutionWidth = Screen.currentResolution.width;
        CurrentSettings.resolutionHeight = Screen.currentResolution.height;

        SaveSettingsToDisk();
    }

    public void SaveSettingsToDisk()
    {
        try
        {
            string json = JsonUtility.ToJson(CurrentSettings, true);
            File.WriteAllText(savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SettingsManager] Cannot save on disk : {e.Message}");
        }
    }

    public void ApplyPendingSettings()
    {
        CurrentSettings = CloneSettings(PendingSettings);
        ApplySettingsToSystem(CurrentSettings);
        SaveSettingsToDisk();

        if (OnSettingsApplied != null) OnSettingsApplied.Invoke();
    }

    public void ResetPendingSettingsToFactory()
    {
        PendingSettings = new GameSettingsData();

        if (OnSettingsApplied != null) OnSettingsApplied.Invoke();

        Debug.Log("[SettingsManager] Pending settings reset to factory. Waiting for Apply.");
    }

    private void ApplySettingsToSystem(GameSettingsData settings)
    {
        FullScreenMode unityMode = GetUnityMode(settings.displayMode);
        Screen.SetResolution(settings.resolutionWidth, settings.resolutionHeight, unityMode);
        QualitySettings.vSyncCount = settings.vSync ? 1 : 0;
        Application.targetFrameRate = settings.fpsLimit;

        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.ApplyRebindedControls(settings.controls);

        // Here we'll apply other settings later, just like that :
        // QualitySettings.SetQualityLevel(settings.qualityIndex);
        // QualitySettings.vSyncCount = settings.vSync ? 1 : 0;

        Debug.Log("[SettingsManager] Settings applied directly to engine.");
    }

    private FullScreenMode GetUnityMode(DisplayMode mode)
    {
        switch (mode)
        {
            case DisplayMode.Windowed:
                return FullScreenMode.Windowed;

            case DisplayMode.Borderless:
                return FullScreenMode.FullScreenWindow;

            case DisplayMode.Fullscreen:
                return FullScreenMode.ExclusiveFullScreen;

            default:
                return FullScreenMode.FullScreenWindow;
        }
    }

    private GameSettingsData CloneSettings(GameSettingsData source)
    {
        string json = JsonUtility.ToJson(source);
        return JsonUtility.FromJson<GameSettingsData>(json);
    }
}