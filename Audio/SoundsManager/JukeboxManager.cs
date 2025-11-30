using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Random Durations")]
    public float minCrackleDuration = 2f;
    public float maxCrackleDuration = 5f;
    public float minDelayBetweenTracks = 2f;
    public float maxDelayBetweenTracks = 4f;

    [Header("VFX")]
    public ParticleSystem jukeboxNotesParticles;

    private List<AudioClip> musicClips = new List<AudioClip>();
    private AudioClip diskInsertClip;
    private AudioClip vinylCrackleClip;

    private bool isRunning = false;

    private void Start()
    {
        LoadResources();
        
    }

    void LoadResources()
    {
        AudioClip[] loadedMusic = Resources.LoadAll<AudioClip>("Audio/Music/JukeBox");
        musicClips.AddRange(loadedMusic);

        diskInsertClip = Resources.Load<AudioClip>("Audio/Tavern/JukeBox/disk_insert");
        vinylCrackleClip = Resources.Load<AudioClip>("Audio/Tavern/JukeBox/vinyl_crackle");

        if (musicClips.Count == 0) Debug.LogWarning("No audio clip found in Resources/Audio/Music/JukeBox/");
    }

    public void StartJukebox()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(JukeboxLoop());
        }
    }

    void StartMusicVFX()
    {
        if (jukeboxNotesParticles != null && !jukeboxNotesParticles.isPlaying) jukeboxNotesParticles.Play();
    }

    void StopMusicVFX()
    {
        if (jukeboxNotesParticles != null && jukeboxNotesParticles.isPlaying) jukeboxNotesParticles.Stop();
    }

    void PlayInsertSFX()
    {
        if (!IsActive) return;
        audioSource.volume = 0.04f;
        audioSource.PlayOneShot(diskInsertClip);
    }

    void PlayMusic(AudioClip clip)
    {
        if (!IsActive) return;
        audioSource.volume = 0.05f;
        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.Play();
    }

    void PlayCrackle()
    {
        if (!IsActive) return;
        audioSource.volume = 0.025f;
        audioSource.loop = true;
        audioSource.clip = vinylCrackleClip;
        audioSource.Play();
    }

    IEnumerator JukeboxLoop()
    {
        List<AudioClip> remainingTracks = new List<AudioClip>(musicClips);

        while (true)
        {
            while (!IsActive)
            {
                audioSource.Stop();
                StopMusicVFX();
                yield return null;
            }

            if (remainingTracks.Count == 0) remainingTracks = new List<AudioClip>(musicClips);

            if (diskInsertClip != null) PlayInsertSFX();
            yield return new WaitForSeconds(diskInsertClip != null ? diskInsertClip.length : 1f);

            int index = Random.Range(0, remainingTracks.Count);
            AudioClip track = remainingTracks[index];
            remainingTracks.RemoveAt(index);

            StartMusicVFX();
            PlayMusic(track);
            yield return new WaitForSeconds(track.length);

            StopMusicVFX();

            if (vinylCrackleClip != null)
            {
                float crackleDuration = Random.Range(minCrackleDuration, maxCrackleDuration);

                PlayCrackle();
                yield return new WaitForSeconds(crackleDuration);

                audioSource.Stop();
                audioSource.loop = false;
            }

            yield return new WaitForSeconds(Random.Range(minDelayBetweenTracks, maxDelayBetweenTracks));
        }
    }


    public bool IsActive { get; set; } = true;
}
