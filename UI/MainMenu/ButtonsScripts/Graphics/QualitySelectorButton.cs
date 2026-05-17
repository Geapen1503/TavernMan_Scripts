using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QualitySelectorButton : BaseMenuButton
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI qualityText;

    [Header("Arrows")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;

    private string[] qualityNames;
    private int currentIndex;

    private SettingsManager settings;
    private MainMenuUI menuUI;

    protected override void Start()
    {
        base.Start();

        settings = SettingsManager.Instance;
        menuUI = MainMenuUI.Instance;

        LoadQualityLevels();

        SetupButton(leftButton, PreviousQuality, leftText);
        SetupButton(rightButton, NextQuality, rightText);

        if (settings != null) settings.OnSettingsApplied += RefreshFromSettings;

        RefreshFromSettings();
    }

    private void OnDestroy()
    {
        if (settings != null) settings.OnSettingsApplied -= RefreshFromSettings;
    }

    private void LoadQualityLevels()
    {
        qualityNames = QualitySettings.names;
    }

    private void RefreshFromSettings()
    {
        if (settings == null || qualityNames == null || qualityNames.Length == 0) return;

        currentIndex = settings.PendingSettings.qualityIndex;

        if (currentIndex < 0 || currentIndex >= qualityNames.Length) currentIndex = 0;

        UpdateQualityText();
    }

    public void NextQuality()
    {
        ChangeQuality(1);
    }

    public void PreviousQuality()
    {
        ChangeQuality(-1);
    }

    private void ChangeQuality(int direction)
    {
        if (qualityNames == null || qualityNames.Length == 0) return;

        currentIndex = (currentIndex + direction + qualityNames.Length) % qualityNames.Length;

        ApplyQuality();
    }

    private void ApplyQuality()
    {
        if (settings != null) settings.PendingSettings.qualityIndex = currentIndex;

        UpdateQualityText();
    }

    private void UpdateQualityText()
    {
        if (qualityText == null || qualityNames == null || qualityNames.Length == 0) return;

        qualityText.text = qualityNames[currentIndex];
    }

    private void SetupButton(Button button, UnityEngine.Events.UnityAction action, TextMeshProUGUI text)
    {
        if (button == null) return;

        button.onClick.AddListener(action);

        if (menuUI != null) button.onClick.AddListener(menuUI.PlayClickSound);

        SetupHover(button, text);
    }

    private void SetupHover(Button button, TextMeshProUGUI text)
    {
        if (button == null || text == null) return;

        EventTrigger trigger =
            button.GetComponent<EventTrigger>() ??
            button.gameObject.AddComponent<EventTrigger>();

        AddTrigger(trigger, EventTriggerType.PointerEnter, () =>
        {
            if (menuUI == null) return;

            text.color = menuUI.hoverTextColor;
            menuUI.PlayHoverSound();
        });

        AddTrigger(trigger, EventTriggerType.PointerExit, () =>
        {
            if (menuUI == null) return;

            text.color = menuUI.normalTextColor;
        });
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction callback)
    {
        EventTrigger.Entry entry = new() { eventID = type };

        entry.callback.AddListener(_ => callback());
        trigger.triggers.Add(entry);
    }

    public override void OnPointerEnter(PointerEventData eventData) { }
    public override void OnPointerExit(PointerEventData eventData) { }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (menuUI == null) return;
        if (leftText != null) leftText.color = menuUI.normalTextColor;
        if (rightText != null) rightText.color = menuUI.normalTextColor;
    }
}