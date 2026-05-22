using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextMenuButton : BaseMenuButton
{
    [Header("Text Transition")]
    [SerializeField] protected TextMeshProUGUI buttonText;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        base.OnPointerEnter(eventData); 

        if (buttonText != null && MainMenuUI.Instance != null)
        {
            buttonText.color = MainMenuUI.Instance.hoverTextColor;
        }
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

    protected virtual void ResetButtonColor()
    {
        if (buttonText != null && MainMenuUI.Instance != null)
        {
            buttonText.color = MainMenuUI.Instance.normalTextColor;
        }
    }
}