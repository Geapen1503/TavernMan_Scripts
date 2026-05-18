using UnityEngine;

public class VSyncSelectorButton : SelectorBase<bool>
{
    protected override void InitializeOptions()
    {
        options.Add(false);
        options.Add(true);
    }

    protected override bool GetCurrentSetting(GameSettingsData settingsData) => settingsData.vSync;

    protected override void SetSetting(GameSettingsData settingsData, bool value) => settingsData.vSync = value;

    protected override string GetValueDisplayString(bool value) => value ? "VSync-On" : "VSync-Off";
}