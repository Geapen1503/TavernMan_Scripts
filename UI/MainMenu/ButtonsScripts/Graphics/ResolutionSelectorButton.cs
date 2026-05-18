using System.Collections.Generic;
using UnityEngine;

public class ResolutionSelectorButton : SelectorBase<Resolution>
{
    protected override void InitializeOptions()
    {
        HashSet<string> added = new();
        foreach (Resolution res in Screen.resolutions)
        {
            string key = $"{res.width}x{res.height}";
            if (added.Add(key)) options.Add(res);
        }
    }

    protected override Resolution GetCurrentSetting(GameSettingsData settingsData)
    {
        return new Resolution { width = settingsData.resolutionWidth, height = settingsData.resolutionHeight };
    }

    protected override void SetSetting(GameSettingsData settingsData, Resolution value)
    {
        settingsData.resolutionWidth = value.width;
        settingsData.resolutionHeight = value.height;
    }

    protected override string GetValueDisplayString(Resolution value) => $"{value.width} x {value.height}";

    protected override int FindCurrentIndex(Resolution currentValue)
    {
        return options.FindIndex(r => r.width == currentValue.width && r.height == currentValue.height);
    }
}