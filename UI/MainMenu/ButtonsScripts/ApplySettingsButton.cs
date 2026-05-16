using UnityEngine;

public class ApplySettingsButton : TextMenuButton
{
    protected override void Start()
    {
        base.Start();
        if (button != null) button.onClick.AddListener(OnApplyClicked);
    }

    private void OnApplyClicked()
    {
        if (SettingsManager.Instance != null) SettingsManager.Instance.ApplyPendingSettings();
    }
}