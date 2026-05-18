using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SelectorBase<T> : BaseMenuButton
{
    [Header("UI Selector Base")]
    [SerializeField] protected TextMeshProUGUI valueText; 

    [Header("Arrows")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI leftArrowText;
    [SerializeField] private TextMeshProUGUI rightArrowText;

    protected List<T> options = new List<T>();
    protected int currentIndex;

    protected SettingsManager settings;
    protected MainMenuUI ui;

    protected override void Start()
    {
        base.Start();

        settings = SettingsManager.Instance;
        ui = MainMenuUI.Instance;

        InitializeOptions();

        SetupButton(leftButton, Previous, leftArrowText);
        SetupButton(rightButton, Next, rightArrowText);

        if (settings != null) settings.OnSettingsApplied += RefreshFromSettings;

        RefreshFromSettings();
    }

    protected virtual void OnDestroy()
    {
        if (settings != null) settings.OnSettingsApplied -= RefreshFromSettings;
    }

    protected abstract void InitializeOptions();
    protected abstract T GetCurrentSetting(GameSettingsData settingsData);
    protected abstract void SetSetting(GameSettingsData settingsData, T value);
    protected abstract string GetValueDisplayString(T value);

    protected virtual int FindCurrentIndex(T currentValue)
    {
        return options.IndexOf(currentValue);
    }

    public void Next() => ChangeIndex(1);
    public void Previous() => ChangeIndex(-1);

    private void ChangeIndex(int direction)
    {
        if (options.Count == 0) return;

        currentIndex = (currentIndex + direction + options.Count) % options.Count;
        ApplyValue();
    }

    protected virtual void RefreshFromSettings()
    {
        if (settings == null || options.Count == 0) return;

        T currentSetting = GetCurrentSetting(settings.PendingSettings);
        currentIndex = FindCurrentIndex(currentSetting);

        if (currentIndex < 0 || currentIndex >= options.Count) currentIndex = 0;

        UpdateUI();
    }

    private void ApplyValue()
    {
        if (settings != null && options.Count > 0) SetSetting(settings.PendingSettings, options[currentIndex]);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (valueText != null && options.Count > 0) valueText.text = GetValueDisplayString(options[currentIndex]);
    }

    private void SetupButton(Button button, UnityEngine.Events.UnityAction action, TextMeshProUGUI arrowText)
    {
        if (button == null) return;

        button.onClick.AddListener(action);
        if (ui != null) button.onClick.AddListener(ui.PlayClickSound);

        SetupHover(button, arrowText);
    }

    private void SetupHover(Button button, TextMeshProUGUI arrowText)
    {
        if (button == null || arrowText == null) return;

        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        AddTrigger(trigger, EventTriggerType.PointerEnter, () =>
        {
            if (ui == null) return;
            arrowText.color = ui.hoverTextColor;
            ui.PlayHoverSound();
        });

        AddTrigger(trigger, EventTriggerType.PointerExit, () =>
        {
            if (ui == null) return;
            arrowText.color = ui.normalTextColor;
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
        if (ui == null) return;

        if (leftArrowText != null) leftArrowText.color = ui.normalTextColor;
        if (rightArrowText != null) rightArrowText.color = ui.normalTextColor;
    }
}