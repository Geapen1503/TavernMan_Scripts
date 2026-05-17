using System;
using UnityEngine; 

[Serializable]
public class GameSettingsData
{
    public int resolutionWidth;
    public int resolutionHeight;
    public DisplayMode displayMode;
    public int qualityIndex;
    public bool vSync;

    // Default settings right here:
    public GameSettingsData()
    {
        resolutionWidth = Screen.currentResolution.width;
        resolutionHeight = Screen.currentResolution.height;

        displayMode = DisplayMode.Borderless;
        qualityIndex = QualitySettings.GetQualityLevel();
        vSync = true;
    }
}

public enum DisplayMode
{
    Windowed,
    Borderless,
    Fullscreen
}