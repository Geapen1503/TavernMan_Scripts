using UnityEngine;

public class QualitySelectorButton : SelectorBase<int>
{
    protected override void InitializeOptions()
    {
        for (int i = 0; i < QualitySettings.names.Length; i++) options.Add(i);
    }

    protected override int GetCurrentSetting(GameSettingsData settingsData) => settingsData.qualityIndex;
    protected override void SetSetting(GameSettingsData settingsData, int value) => settingsData.qualityIndex = value;

    protected override string GetValueDisplayString(int value)
    {
        string[] names = QualitySettings.names;
        return (value >= 0 && value < names.Length) ? names[value] : "Unknown";
    }
}