using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonTextFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.black;

    private void Awake()
    {
        if (buttonText != null) buttonText.color = normalTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.color = hoverTextColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.color = normalTextColor;
    }
}