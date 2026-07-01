using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;
    public BoxCollider jukeboxTriggerCol;

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
    private bool clipsPreloaded = false;

    private string jazzAudioClipsPath = "Audio/Music/JukeBox/Jazz";
    private string latinAudioClipsPath = "Audio/Music/JukeBox/Latin";

    private bool isPlayerInsideTrigger = false;
    private bool useLatinPlaylist = false;
    private Coroutine jukeboxLoopCoroutine;

    private void Start()
    {
        LoadResources();
        StartCoroutine(PreloadAudioClipsAndMaybeStart());
    }

    void LoadResources()
    {
        AudioClip[] loadedMusic = Resources.LoadAll<AudioClip>(jazzAudioClipsPath);
        musicClips.AddRange(loadedMusic);

        diskInsertClip = Resources.Load<AudioClip>("Audio/Tavern/JukeBox/disk_insert");
        vinylCrackleClip = Resources.Load<AudioClip>("Audio/Tavern/JukeBox/vinyl_crackle");

        if (musicClips.Count == 0) Debug.LogWarning("No audio clip found in Resources/Audio/Music/JukeBox/");
    }

    public void StartJukebox()
    {
        if (!isRunning)
        {
            if (!clipsPreloaded)
            {
                StartCoroutine(PreloadAudioClipsAndMaybeStart());
                return;
            }

            isRunning = true;
            jukeboxLoopCoroutine = StartCoroutine(JukeboxLoop());
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
        audioSource.volume = 0.1f;
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

    IEnumerator PreloadAudioClipsAndMaybeStart()
    {
        List<AudioClip> toLoad = new List<AudioClip>();
        toLoad.AddRange(musicClips);
        if (diskInsertClip != null) toLoad.Add(diskInsertClip);
        if (vinylCrackleClip != null) toLoad.Add(vinylCrackleClip);

        float overallTimeout = 10f; 
        float startedAt = Time.realtimeSinceStartup;

        foreach (var clip in toLoad)
        {
            if (clip == null) continue;

            if (clip.loadState == AudioDataLoadState.Loaded) continue;

            bool started = clip.LoadAudioData();
            if (!started) Debug.LogWarning($"Jukebox: cannot launch LoadAudioData() for {clip.name}");
        }

        bool allLoaded = false;
        while (true)
        {
            allLoaded = true;
            foreach (var clip in toLoad)
            {
                if (clip == null) continue;
                if (clip.loadState != AudioDataLoadState.Loaded)
                {
                    allLoaded = false;
                    break;
                }
            }

            if (allLoaded) break;

            if (Time.realtimeSinceStartup - startedAt > overallTimeout)
            {
                Debug.LogWarning("Jukebox: timeout when preloading audio clips. Some clips might not be ready and can cause latency.");
                break;
            }

            yield return null;
        }

        clipsPreloaded = true;

        if (jukeboxTriggerCol != null) jukeboxTriggerCol.enabled = IsCurrentDayValidForPlaylistChange();
        if (IsActive) StartJukebox();
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

    public void TryExecutePlaylistChange()
    {
        if (!IsCurrentDayValidForPlaylistChange()) return;
        if (!isPlayerInsideTrigger) return;

        useLatinPlaylist = !useLatinPlaylist;
        string targetPath = useLatinPlaylist ? latinAudioClipsPath : jazzAudioClipsPath;

        AudioClip[] loadedMusic = Resources.LoadAll<AudioClip>(targetPath);

        if (loadedMusic.Length > 0)
        {
            musicClips.Clear();
            musicClips.AddRange(loadedMusic);

            if (isRunning)
            {
                if (jukeboxLoopCoroutine != null) StopCoroutine(jukeboxLoopCoroutine);

                audioSource.Stop();
                StopMusicVFX();

                jukeboxLoopCoroutine = StartCoroutine(JukeboxLoop());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInsideTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInsideTrigger = false;
    }

    private bool IsCurrentDayValidForPlaylistChange()
    {
        if (GameStateManager.Instance == null) return false;

        DayID currentDay = GameStateManager.Instance.GetCurrentDayID();

        return currentDay == DayID.Day5 || currentDay == DayID.Day6 || currentDay == DayID.Day7;
    }

    public bool IsActive { get; set; } = true;
}
