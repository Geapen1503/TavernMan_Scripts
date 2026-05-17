using UnityEngine;

public class ResetSettingsButton : TextMenuButton
{
    protected override void Start()
    {
        base.Start();
        if (button != null) button.onClick.AddListener(OnResetClicked);
    }

    private void OnResetClicked()
    {
        if (SettingsManager.Instance != null) SettingsManager.Instance.ResetPendingSettingsToFactory();
    }
}