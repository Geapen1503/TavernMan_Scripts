using System;
using UnityEngine; 

[Serializable]
public class GameSettingsData
{
    public int resolutionWidth;
    public int resolutionHeight;
    public bool isFullScreen;
    public bool vSync;

    // Default settings right here:
    public GameSettingsData()
    {
        resolutionWidth = Screen.currentResolution.width;
        resolutionHeight = Screen.currentResolution.height;

        isFullScreen = true;
        vSync = true;
    }
}