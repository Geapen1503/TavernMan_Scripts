using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sounds Manager")]
    public CricketsSoundManager cricketsSoundManager;
    public WavesSoundManager wavesSoundManager;
    public TavernSoundManager tavernSoundManager;
    public JukeboxManager jukeboxManager;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private bool playerIsInTavern;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ExitTavern();
    }

    // When you have time rewrite theses 4 methods it looks awful
    public void PauseEnvironmentSounds()
    {
        foreach (var cricket in cricketsSoundManager.Crickets) cricket.Pause(); 
        foreach (var waves in wavesSoundManager.Waves) waves.Pause();
        foreach (var tavernSounds in tavernSoundManager.Sounds) tavernSounds.Pause();
        jukeboxManager.IsActive = false;
    }

    public void ResumeEnvironmentSounds()
    {
        foreach (var cricket in cricketsSoundManager.Crickets) cricket.UnPause(); 
        foreach (var waves in wavesSoundManager.Waves) waves.UnPause();

        if (playerIsInTavern == true)
        {
            jukeboxManager.IsActive = true;
            foreach (var tavernSounds in tavernSoundManager.Sounds) tavernSounds.UnPause();
        }
        else
        {
            jukeboxManager.IsActive = false;
            foreach (var tavernSounds in tavernSoundManager.Sounds) tavernSounds.Pause();
        }
    }

    public void EnterTavern()
    {
        foreach (var tavernSounds in tavernSoundManager.Sounds) tavernSounds.UnPause();
        foreach (var cricket in cricketsSoundManager.Crickets) StartCoroutine(FadeOut(cricket, fadeDuration));
        foreach (var waves in wavesSoundManager.waves) waves.Pause();

        playerIsInTavern = true;

        jukeboxManager.IsActive = true;
        jukeboxManager.StartJukebox();
    }

    public void ExitTavern()
    {
        foreach (var tavernSounds in tavernSoundManager.Sounds) tavernSounds.Pause();
        foreach (var cricket in cricketsSoundManager.Crickets) StartCoroutine(FadeIn(cricket, fadeDuration, 0.018f));
        foreach (var waves in wavesSoundManager.waves) waves.UnPause();

        playerIsInTavern = false;
        jukeboxManager.IsActive = false;
    }

    IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        audioSource.volume = 0f;
    }

    IEnumerator FadeIn(AudioSource audioSource, float duration, float targetVolume = 0.018f)
    {
        audioSource.volume = 0f;

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}
