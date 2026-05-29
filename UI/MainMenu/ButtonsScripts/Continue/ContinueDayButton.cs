using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ContinueDayButton : TextMenuButton
{
    public DayID targetDay;
    public Color disabledTextColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private bool _isUnlocked;

    protected override void Start()
    {
        base.Start(); 
        SetupButtonState();
    }

    private void SetupButtonState()
    {
        GameProgressData progress = GameStateManager.ReadSaveData();
        _isUnlocked = progress.IsDayCompleted(targetDay);

        if (button != null)
        {
            button.interactable = true;

            button.onClick.AddListener(OnDayButtonClicked);
        }

        ResetButtonColor();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!_isUnlocked && buttonText != null)
        {
            buttonText.color = disabledTextColor;
        }
    }

    protected override void ResetButtonColor()
    {
        if (buttonText != null && MainMenuUI.Instance != null)
        {
            if (_isUnlocked) buttonText.color = MainMenuUI.Instance.normalTextColor;
            else buttonText.color = disabledTextColor;
        }
    }

    private void OnDayButtonClicked()
    {
        if (!_isUnlocked) return;

        GameStateManager.HasTargetDayToLoad = true;
        GameStateManager.TargetDayToLoad = targetDay;

        StartCoroutine(MainMenuUI.Instance.LoadSceneWithLoadingScreen(GameScenes.Game));
    }
}