using System;
using UnityEngine; 

[Serializable]
public class GameSettingsData
{
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public DisplayMode displayMode;
    public int qualityIndex;
    public bool vSync;
    public int fpsLimit;

    public GameLanguage language;

    public ControlsSettings controls;

    // Default settings right here:
    public GameSettingsData()
    {
        resolutionWidth = Screen.currentResolution.width;
        resolutionHeight = Screen.currentResolution.height;
        displayMode = DisplayMode.Borderless;
        qualityIndex = QualitySettings.GetQualityLevel();
        vSync = true;
        fpsLimit = 60;

        language = GameLanguage.English;

        controls = new ControlsSettings();
    }
}
