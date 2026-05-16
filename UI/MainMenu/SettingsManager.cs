using System;
using System.IO;
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

        OnSettingsApplied?.Invoke(); 
    }

    public void ResetPendingSettings()
    {
        PendingSettings = new GameSettingsData();

        ApplyPendingSettings();
    }

    private void ApplySettingsToSystem(GameSettingsData settings)
    {
        Screen.SetResolution(settings.resolutionWidth, settings.resolutionHeight, settings.isFullScreen);

        // Here we'll apply other settings later, just like that :
        // QualitySettings.SetQualityLevel(settings.qualityIndex);
        // QualitySettings.vSyncCount = settings.vSync ? 1 : 0;

        Debug.Log("[SettingsManager] Settings applying to engine.");
    }

    private GameSettingsData CloneSettings(GameSettingsData source)
    {
        return new GameSettingsData
        {
            resolutionWidth = source.resolutionWidth,
            resolutionHeight = source.resolutionHeight,
            isFullScreen = source.isFullScreen,
            vSync = source.vSync
        };
    }
}