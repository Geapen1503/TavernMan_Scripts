using UnityEngine;

public class LanguageSelectorButton : SelectorBase<GameLanguage>
{
    protected override void InitializeOptions()
    {
        options.Add(GameLanguage.English);
        // options.Add(GameLanguage.French);
    }

    protected override GameLanguage GetCurrentSetting(GameSettingsData settingsData)
    {
        return settingsData.language;
    }

    protected override void SetSetting(GameSettingsData settingsData, GameLanguage value)
    {
        settingsData.language = value;
    }

    protected override string GetValueDisplayString(GameLanguage value)
    {
        return value switch
        {
            GameLanguage.English => "English",
            GameLanguage.French => "Français",
            _ => value.ToString()
        };
    }
}