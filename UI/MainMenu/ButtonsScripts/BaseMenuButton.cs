using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }

    protected virtual void Start()
    {
        if (button != null && MainMenuUI.Instance != null)
        {
            button.onClick.AddListener(MainMenuUI.Instance.PlayClickSound);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        if (MainMenuUI.Instance != null) MainMenuUI.Instance.PlayHoverSound();
    }

    public virtual void OnPointerExit(PointerEventData eventData) { }

    protected virtual void OnDisable() { }
}