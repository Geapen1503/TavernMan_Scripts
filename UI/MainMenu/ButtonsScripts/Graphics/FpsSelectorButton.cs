using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FpsSelectorButton : SelectorBase<int>
{
    protected override void InitializeOptions()
    {
        options.AddRange(new int[] { 30, 60, 90, 120, 144, 240, -1 });
    }

    protected override int GetCurrentSetting(GameSettingsData settingsData) => settingsData.fpsLimit;
    protected override void SetSetting(GameSettingsData settingsData, int value) => settingsData.fpsLimit = value;
    protected override string GetValueDisplayString(int value) => value == -1 ? "Unlimited" : $"{value} FPS";

    protected override int FindCurrentIndex(int currentValue)
    {
        int index = base.FindCurrentIndex(currentValue);
        return index < 0 ? 1 : index;
    }
}