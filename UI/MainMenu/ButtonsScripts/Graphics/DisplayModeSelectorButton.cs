using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DisplayModeSelectorButton : SelectorBase<DisplayMode>
{
    protected override void InitializeOptions()
    {
        options.Add(DisplayMode.Windowed);
        options.Add(DisplayMode.Borderless);
        options.Add(DisplayMode.Fullscreen);
    }

    protected override DisplayMode GetCurrentSetting(GameSettingsData settingsData) => settingsData.displayMode;
    protected override void SetSetting(GameSettingsData settingsData, DisplayMode value) => settingsData.displayMode = value;
    protected override string GetValueDisplayString(DisplayMode value) => value.ToString();
}