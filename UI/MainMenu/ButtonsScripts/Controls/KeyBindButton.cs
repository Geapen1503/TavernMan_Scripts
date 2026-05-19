using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyBindButton : BaseMenuButton
{
    public enum ControlAction { Jump, Grab, Talk, Throw, Pause, Forward, Backward, Left, Right }

    [Header("KeyBind Configuration")]
    [SerializeField] private ControlAction actionToBind;
    [SerializeField] private TextMeshProUGUI keyText;

    private bool isListening = false;
    private SettingsManager settings;
    private MainMenuUI ui;

    protected override void Start()
    {
        base.Start();
        settings = SettingsManager.Instance;

        if (button != null) button.onClick.AddListener(StartListeningForInput);

        if (settings != null) settings.OnSettingsApplied += RefreshUI;

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (settings != null) settings.OnSettingsApplied -= RefreshUI;
    }

    private void RefreshUI()
    {
        if (settings == null || keyText == null) return;

        KeyCode currentKey = GetKeyFromSettings(settings.PendingSettings.controls);
        keyText.text = currentKey.ToString();
    }

    private void StartListeningForInput()
    {
        if (isListening) return;

        isListening = true;
        if (keyText != null) keyText.text = "..."; 
    }

    private void Update()
    {
        if (!isListening) return;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (keyCode == KeyCode.None) continue;

                if (Input.GetKeyDown(keyCode))
                {
                    if (keyCode == KeyCode.Escape && actionToBind != ControlAction.Pause)
                    {
                        isListening = false;
                        RefreshUI();
                        return;
                    }

                    SaveNewKey(keyCode);
                    return; 
                }
            }
        }
    }

    private void SaveNewKey(KeyCode newKey)
    {
        isListening = false;

        if (settings != null) SetKeyInSettings(settings.PendingSettings.controls, newKey);

        RefreshUI();

        if (keyText != null && MainMenuUI.Instance != null) keyText.color = MainMenuUI.Instance.hoverTextColor;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        base.OnPointerEnter(eventData);

        if (keyText != null && MainMenuUI.Instance != null) keyText.color = MainMenuUI.Instance.hoverTextColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ResetButtonColor();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetButtonColor();
    }

    private void ResetButtonColor()
    {
        if (keyText != null && MainMenuUI.Instance != null) keyText.color = MainMenuUI.Instance.normalTextColor;
    }

    private KeyCode GetKeyFromSettings(ControlsSettings controls)
    {
        return actionToBind switch
        {
            ControlAction.Jump => controls.jump,
            ControlAction.Grab => controls.grab,
            ControlAction.Talk => controls.talk,
            ControlAction.Throw => controls.throwKey,
            ControlAction.Pause => controls.pause,
            ControlAction.Forward => controls.forward,
            ControlAction.Backward => controls.backward,
            ControlAction.Left => controls.left,
            ControlAction.Right => controls.right,
            _ => KeyCode.None
        };
    }

    private void SetKeyInSettings(ControlsSettings controls, KeyCode newKey)
    {
        switch (actionToBind)
        {
            case ControlAction.Jump: controls.jump = newKey; break;
            case ControlAction.Grab: controls.grab = newKey; break;
            case ControlAction.Talk: controls.talk = newKey; break;
            case ControlAction.Throw: controls.throwKey = newKey; break;
            case ControlAction.Pause: controls.pause = newKey; break;
            case ControlAction.Forward: controls.forward = newKey; break;
            case ControlAction.Backward: controls.backward = newKey; break;
            case ControlAction.Left: controls.left = newKey; break;
            case ControlAction.Right: controls.right = newKey; break;
        }
    }
}