using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResolutionSelectorButton : BaseMenuButton
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI resolutionText;

    [Header("Arrows")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;

    private readonly List<Resolution> resolutions = new();
    private int currentIndex;

    private SettingsManager settings;
    private MainMenuUI menuUI;

    protected override void Start()
    {
        base.Start();

        settings = SettingsManager.Instance;
        menuUI = MainMenuUI.Instance;

        LoadResolutions();

        SetupButton(leftButton, PreviousResolution, leftText);
        SetupButton(rightButton, NextResolution, rightText);

        if (settings != null) settings.OnSettingsApplied += RefreshFromSettings;

        RefreshFromSettings();
    }

    private void OnDestroy()
    {
        if (settings != null) settings.OnSettingsApplied -= RefreshFromSettings;
    }

    private void LoadResolutions()
    {
        HashSet<string> added = new();

        foreach (Resolution res in Screen.resolutions)
        {
            string key = $"{res.width}x{res.height}";

            if (added.Add(key)) resolutions.Add(res);
        }
    }

    private void RefreshFromSettings()
    {
        if (settings == null || resolutions.Count == 0) return;

        int width = settings.PendingSettings.resolutionWidth;
        int height = settings.PendingSettings.resolutionHeight;

        currentIndex = resolutions.FindIndex(r =>
            r.width == width &&
            r.height == height);

        if (currentIndex < 0) currentIndex = 0;

        UpdateResolutionText();
    }

    public void NextResolution()
    {
        ChangeResolution(1);
    }

    public void PreviousResolution()
    {
        ChangeResolution(-1);
    }

    private void ChangeResolution(int direction)
    {
        if (resolutions.Count == 0) return;

        currentIndex = (currentIndex + direction + resolutions.Count) % resolutions.Count;

        ApplyResolution();
    }

    private void ApplyResolution()
    {
        Resolution res = resolutions[currentIndex];

        if (settings != null)
        {
            settings.PendingSettings.resolutionWidth = res.width;
            settings.PendingSettings.resolutionHeight = res.height;
        }

        UpdateResolutionText();
    }

    private void UpdateResolutionText()
    {
        if (resolutionText == null || resolutions.Count == 0)
            return;

        Resolution res = resolutions[currentIndex];
        resolutionText.text = $"{res.width} x {res.height}";
    }

    private void SetupButton(
        Button button,
        UnityEngine.Events.UnityAction action,
        TextMeshProUGUI text)
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
            if (menuUI == null)  return;

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
        EventTrigger.Entry entry = new()
        {
            eventID = type
        };

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