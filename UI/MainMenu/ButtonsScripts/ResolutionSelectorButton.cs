using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResolutionSelectorButton : BaseMenuButton
{
    [Header("Selector UI Elements")]
    [SerializeField] private TextMeshProUGUI resolutionText;

    [Header("Left Arrow Setup")]
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private TextMeshProUGUI leftArrowText;

    [Header("Right Arrow Setup")]
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private TextMeshProUGUI rightArrowText;

    private List<Resolution> filteredResolutions = new List<Resolution>();
    private int currentResolutionIndex = 0;

    protected override void Start()
    {
        base.Start();
        InitResolutions();

        if (leftArrowButton != null) leftArrowButton.onClick.AddListener(PreviousResolution);
        if (rightArrowButton != null) rightArrowButton.onClick.AddListener(NextResolution);

        if (MainMenuUI.Instance != null)
        {
            if (leftArrowButton != null) leftArrowButton.onClick.AddListener(MainMenuUI.Instance.PlayClickSound);
            if (rightArrowButton != null) rightArrowButton.onClick.AddListener(MainMenuUI.Instance.PlayClickSound);
        }

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnSettingsApplied += UpdateUIFromSettingsManager;
        }

        SetupArrowHoverEvents();
        UpdateUIFromSettingsManager();
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnSettingsApplied -= UpdateUIFromSettingsManager;
        }
    }

    private void InitResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        HashSet<string> uniqueResolutions = new HashSet<string>();

        foreach (var res in allResolutions)
        {
            string resString = res.width + "x" + res.height;
            if (!uniqueResolutions.Contains(resString))
            {
                uniqueResolutions.Add(resString);
                filteredResolutions.Add(res);
            }
        }
    }

    private void UpdateUIFromSettingsManager()
    {
        if (SettingsManager.Instance == null || filteredResolutions.Count == 0) return;

        int targetWidth = SettingsManager.Instance.PendingSettings.resolutionWidth;
        int targetHeight = SettingsManager.Instance.PendingSettings.resolutionHeight;

        currentResolutionIndex = 0;
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == targetWidth && filteredResolutions[i].height == targetHeight)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        UpdateResolutionText();
    }

    public void NextResolution()
    {
        if (filteredResolutions.Count == 0) return;
        currentResolutionIndex = (currentResolutionIndex + 1) % filteredResolutions.Count;
        SelectResolution();
    }

    public void PreviousResolution()
    {
        if (filteredResolutions.Count == 0) return;
        currentResolutionIndex = (currentResolutionIndex - 1 + filteredResolutions.Count) % filteredResolutions.Count;
        SelectResolution();
    }

    private void SelectResolution()
    {
        Resolution targetRes = filteredResolutions[currentResolutionIndex];

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.PendingSettings.resolutionWidth = targetRes.width;
            SettingsManager.Instance.PendingSettings.resolutionHeight = targetRes.height;
        }

        UpdateResolutionText();
    }

    private void UpdateResolutionText()
    {
        if (resolutionText != null && filteredResolutions.Count > 0)
        {
            Resolution res = filteredResolutions[currentResolutionIndex];
            resolutionText.text = $"{res.width} x {res.height}";
        }
    }

    private void SetupArrowHoverEvents()
    {
        if (leftArrowButton != null && leftArrowText != null)
        {
            EventTrigger triggerLeft = leftArrowButton.gameObject.GetComponent<EventTrigger>() ?? leftArrowButton.gameObject.AddComponent<EventTrigger>();
            AddArrowTrigger(triggerLeft, leftArrowText);
        }

        if (rightArrowButton != null && rightArrowText != null)
        {
            EventTrigger triggerRight = rightArrowButton.gameObject.GetComponent<EventTrigger>() ?? rightArrowButton.gameObject.AddComponent<EventTrigger>();
            AddArrowTrigger(triggerRight, rightArrowText);
        }
    }

    private void AddArrowTrigger(EventTrigger trigger, TextMeshProUGUI arrowText)
    {
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((eventData) =>
        {
            if (MainMenuUI.Instance != null)
            {
                arrowText.color = MainMenuUI.Instance.hoverTextColor;
                MainMenuUI.Instance.PlayHoverSound();
            }
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((eventData) =>
        {
            if (MainMenuUI.Instance != null) arrowText.color = MainMenuUI.Instance.normalTextColor;
        });
        trigger.triggers.Add(entryExit);
    }

    public override void OnPointerEnter(PointerEventData eventData) { }
    public override void OnPointerExit(PointerEventData eventData) { }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (MainMenuUI.Instance != null)
        {
            if (leftArrowText != null) leftArrowText.color = MainMenuUI.Instance.normalTextColor;
            if (rightArrowText != null) rightArrowText.color = MainMenuUI.Instance.normalTextColor;
        }
    }
}