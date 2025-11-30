using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Canvas mainMenuCanvas;

    [Header("MainMenuButtons")]
    public List<Button> menuButtons = new List<Button>();

    [Header("Button Texts")]
    public List<TextMeshProUGUI> menuTexts = new List<TextMeshProUGUI>();

    [Header("Button Audio")]
    public AudioSource buttonsAudioSource;

    private AudioClip hoverClip;
    private AudioClip clickClip;

    private float lastHoverTime = 0f;
    private float hoverCooldown = 0.05f;

    [Header("Text Colors")]
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.black;

    void Start()
    {
        hoverClip = Resources.Load<AudioClip>("Audio/Buttons/marimba-hover");
        clickClip = Resources.Load<AudioClip>("Audio/Buttons/marimba-click");

        for (int i = 0; i < menuButtons.Count; i++)
        {
            Button button = menuButtons[i];
            TextMeshProUGUI text = (i < menuTexts.Count) ? menuTexts[i] : null;

            AddHoverEffect(button, text);

            button.onClick.AddListener(PlayClickSound);
        }
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null) EventSystem.current.SetSelectedGameObject(null);
    }

    private void AddHoverEffect(Button button, TextMeshProUGUI text)
    {
        if (button == null || text == null) return;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((eventData) =>
        {
            text.color = hoverTextColor;
            PlayHoverSound();
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((eventData) => { text.color = normalTextColor; });
        trigger.triggers.Add(entryExit);

        EventTrigger.Entry entryDeselect = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
        entryDeselect.callback.AddListener((eventData) => { text.color = hoverTextColor; });
        trigger.triggers.Add(entryDeselect);
    }

    private void PlayHoverSound()
    {
        if (buttonsAudioSource == null || hoverClip == null) return;

        if (Time.time - lastHoverTime >= hoverCooldown)
        {
            buttonsAudioSource.PlayOneShot(hoverClip, 0.6f);
            lastHoverTime = Time.time;
        }
    }

    private void PlayClickSound()
    {
        if (buttonsAudioSource != null && clickClip != null) buttonsAudioSource.PlayOneShot(clickClip, 0.8f);
    }


    public void StartNewGame() => SceneManager.LoadScene(1);
    public void Settings() { }
    public void QuitGame() => Application.Quit();
}
