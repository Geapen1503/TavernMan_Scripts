using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Invector.vCharacterController;

[RequireComponent(typeof(Button))]
public class ButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Button button;
    public TextMeshProUGUI buttonText;
    public Image backgroundImage;

    private static AudioClip hoverClip;
    private static AudioClip clickClip;

    private static float lastHoverTime = 0f;
    private const float hoverCooldown = 0.05f;

    [Header("UI Colors")]
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.black;

    [Header("Background Colors")]
    public Color normalBackgroundColor = Color.clear;
    public Color hoverBackgroundColor = Color.white;

    private void Awake()
    {
        if (hoverClip == null) hoverClip = Resources.Load<AudioClip>("Audio/Buttons/marimba-hover");
        if (clickClip == null) clickClip = Resources.Load<AudioClip>("Audio/Buttons/marimba-click");

        if (backgroundImage == null) backgroundImage = GetComponent<Image>();

        button.onClick.AddListener(PlayClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.color = hoverTextColor;
        if (backgroundImage != null) backgroundImage.color = hoverBackgroundColor;

        PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.color = normalTextColor;
        if (backgroundImage != null) backgroundImage.color = normalBackgroundColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResetVisualState()
    {
        if (buttonText != null) buttonText.color = normalTextColor;
        if (backgroundImage != null) backgroundImage.color = normalBackgroundColor;
    }

    private void PlayHoverSound()
    {
        if (hoverClip == null) return;

        float now = Time.realtimeSinceStartup;
        if (now - lastHoverTime < hoverCooldown) return;

        var player = vThirdPersonController.Instance;
        if (player != null) player.PlayHoverSound(hoverClip);

        lastHoverTime = now;
    }

    private void PlayClickSound()
    {
        if (clickClip == null) return;

        var player = vThirdPersonController.Instance;
        if (player != null) player.PlayClickSound(clickClip);
    }
}
