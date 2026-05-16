using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { get; private set; }

    public Canvas mainMenuCanvas;

    [Header("Button Audio")]
    public AudioSource buttonsAudioSource;
    private AudioClip hoverClip;
    private AudioClip clickClip;

    private float lastHoverTime = 0f;
    private float hoverCooldown = 0.05f;

    [Header("Text Colors")]
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.black;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        hoverClip = Resources.Load<AudioClip>("Audio/Buttons/marimba-hover");
        clickClip = Resources.Load<AudioClip>("Audio/Buttons/marimba-click");
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void PlayHoverSound()
    {
        if (buttonsAudioSource == null || hoverClip == null) return;

        if (Time.time - lastHoverTime >= hoverCooldown)
        {
            buttonsAudioSource.PlayOneShot(hoverClip, 0.6f);
            lastHoverTime = Time.time;
        }
    }

    public void PlayClickSound()
    {
        if (buttonsAudioSource != null && clickClip != null)
        {
            buttonsAudioSource.PlayOneShot(clickClip, 0.8f);
        }
    }

    public void StartNewGame() => SceneManager.LoadScene(1);
    public void Settings() { }
    public void QuitGame() => Application.Quit();
}