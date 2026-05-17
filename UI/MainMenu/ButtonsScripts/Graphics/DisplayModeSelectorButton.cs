using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DisplayModeSelectorButton : BaseMenuButton
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI displayModeText;

    [Header("Arrows")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;

    private SettingsManager settings;
    private MainMenuUI ui;

    private DisplayMode mode = DisplayMode.Borderless;

    protected override void Start()
    {
        base.Start();

        settings = SettingsManager.Instance;
        ui = MainMenuUI.Instance;

        SetupButton(leftButton, PreviousMode, leftText);
        SetupButton(rightButton, NextMode, rightText);

        if (settings != null) settings.OnSettingsApplied += RefreshFromSettings;

        RefreshFromSettings();
    }

    private void OnDestroy()
    {
        if (settings != null) settings.OnSettingsApplied -= RefreshFromSettings;
    }

    private void RefreshFromSettings()
    {
        if (settings == null) return;

        mode = settings.PendingSettings.displayMode;
        UpdateText();
    }

    public void NextMode()
    {
        mode = mode switch
        {
            DisplayMode.Windowed => DisplayMode.Borderless,
            DisplayMode.Borderless => DisplayMode.Fullscreen,
            DisplayMode.Fullscreen => DisplayMode.Windowed,
            _ => mode
        };

        Apply();
    }

    public void PreviousMode()
    {
        mode = mode switch
        {
            DisplayMode.Windowed => DisplayMode.Fullscreen,
            DisplayMode.Borderless => DisplayMode.Windowed,
            DisplayMode.Fullscreen => DisplayMode.Borderless,
            _ => mode
        };

        Apply();
    }

    private void Apply()
    {
        if (settings != null) settings.PendingSettings.displayMode = mode;

        UpdateText();
    }

    private void UpdateText()
    {
        if (displayModeText != null) displayModeText.text = mode.ToString();
    }

    private void SetupButton(Button button, UnityEngine.Events.UnityAction action, TextMeshProUGUI text)
    {
        if (button == null) return;

        button.onClick.AddListener(action);

        if (ui != null) button.onClick.AddListener(ui.PlayClickSound);

        SetupHover(button, text);
    }

    private void SetupHover(Button button, TextMeshProUGUI text)
    {
        if (button == null || text == null)
            return;

        EventTrigger trigger =
            button.GetComponent<EventTrigger>() ??
            button.gameObject.AddComponent<EventTrigger>();

        AddTrigger(trigger, EventTriggerType.PointerEnter, () =>
        {
            if (ui == null) return;

            text.color = ui.hoverTextColor;
            ui.PlayHoverSound();
        });

        AddTrigger(trigger, EventTriggerType.PointerExit, () =>
        {
            if (ui == null) return;

            text.color = ui.normalTextColor;
        });
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new() { eventID = type };

        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    public override void OnPointerEnter(PointerEventData eventData) { }
    public override void OnPointerExit(PointerEventData eventData) { }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (ui == null) return;

        if (leftText != null) leftText.color = ui.normalTextColor;
        if (rightText != null) rightText.color = ui.normalTextColor;
    }
}