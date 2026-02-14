using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Collections;

public enum Sound
{
    GameWin,
    GameLose,
    Wind,
    SlowdownActivation,
    SlowdownRunning,
    Hit,
    Speedup,
    Gunshot,
    PointsCountUp
}

[Serializable]          // visible in Inspector
public struct SoundEntry
{
    public Sound id;            // enum tag
    public AudioClip[] clips;   // drag 1-N clips here
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Music playlist")]
    public AudioClip[] musicClips;          // drag ~10 tracks here
    [Range(0f, 3f)] public float musicFade = .5f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;

    [Header("SFX list")]
    public SoundEntry[] sfxEntries;     // <<< drag pairs here

    [Range(1, 32)] public int sfxVoices = 8;
    public AudioMixerGroup musicBus, sfxBus;

    readonly Dictionary<Sound, AudioClip[]> sfx = new();
    readonly List<AudioSource> sfxSources = new();
    AudioSource musicSource, b;
    int lastMusic = -1;
    bool toggle = false;

    bool gameOver = false;     // are we in the end-screen state?
    bool gamePaused = false;   // did we pause music ourselves?

    readonly Dictionary<Sound, AudioSource> activeLoops = new();

    const string MUSIC_PARAM = "MusicVolume";
    const string MUSIC_PREF = "MusicVolume";


    void Start() => StartCoroutine(MusicLoop());   // kick it off

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);

        // build map once
        foreach (var e in sfxEntries)
            if (e.clips != null && e.clips.Length > 0)
                sfx[e.id] = e.clips;

        // music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicBus;
        musicSource.loop = true;

        // start with a random track right away (optional — the coroutine will keep shuffling)
        if (musicClips.Length > 0)
        {
            lastMusic = UnityEngine.Random.Range(0, musicClips.Length);
            musicSource.clip = musicClips[lastMusic];
            musicSource.Play();
        }

        // SFX pool
        for (int i = 0; i < sfxVoices; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxBus;
            sfxSources.Add(src);
        }
    }
    void PlayRandomMusic()
    {
        if (musicClips == null || musicClips.Length == 0)
            return;

        int idx;
        do
        {
            idx = UnityEngine.Random.Range(0, musicClips.Length);
        }
        while (musicClips.Length > 1 && idx == lastMusic);

        lastMusic = idx;
        musicSource.clip = musicClips[idx];
        musicSource.Play();
    }
    void ApplyMusicVolume()
    {
        if (musicBus == null)
            return;

        // Convert 0–1 slider value to decibels
        float db = Mathf.Log10(Mathf.Max(musicVolume, 0.0001f)) * 20f;

        AudioMixer mixer = musicBus.audioMixer;
        mixer.SetFloat(MUSIC_PARAM, db);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyMusicVolume();
        PlayerPrefs.SetFloat(MUSIC_PREF, musicVolume);
    }


    IEnumerator MusicLoop()
    {
        // assume you’ve already initialized AudioSources a & b, and a started playing
        while (true)
        {
            int nextIdx = PickNewTrack();     // ← now you have a valid index
            if (nextIdx < 0) yield break;

            var nextSource = toggle ? musicSource : b;
            var prevSource = toggle ? b : musicSource;

            nextSource.clip = musicClips[nextIdx];
            nextSource.volume = 0f;
            nextSource.Play();

            // fade-out → swap → fade-in
            yield return FadeMusic(to: 0f, musicFade);
            musicSource.clip = musicClips[nextIdx];
            musicSource.Play();
            yield return FadeMusic(to: 1f, musicFade);

            toggle = !toggle;
            yield return new WaitForSeconds(nextSource.clip.length - musicFade);
        }
    }

    /// <summary>
    /// Picks a random index into musicClips that isn’t the same as lastMusic.
    /// </summary>
    int PickNewTrack()
    {
        if (musicClips == null || musicClips.Length == 0)
            return -1;

        int idx;
        // if there’s more than one clip, avoid repeating
        if (musicClips.Length > 1)
        {
            do
            {
                idx = UnityEngine.Random.Range(0, musicClips.Length);
            }
            while (idx == lastMusic);
        }
        else
        {
            idx = 0;
        }

        lastMusic = idx;    // remember for next time
        return idx;
    }


    IEnumerator FadeMusic(float to, float time)
    {
        float start = musicSource.volume;
        float target = to * musicVolume;        // respect master slider
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }
        musicSource.volume = target;
    }

    /* ----------  API  ---------- */
    public void PlaySound(Sound id, float vol = 1f)
    {
        if (!sfx.TryGetValue(id, out var clips) || clips.Length == 0) return;

        // choose a random variant
        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        GetFreeSfxSource().PlayOneShot(clip, vol);
    }

    /* ---------- public controls ---------- */
    public void SetPaused(bool paused)
    {
        if (gamePaused == paused) return;
        gamePaused = paused;

        if (paused)
        {
            musicSource.Pause();
            foreach (var s in sfxSources) s.Pause();
        }
        else
        {
            musicSource.UnPause();
            foreach (var s in sfxSources) s.UnPause();
        }
    }

    /* helpers */
    AudioSource GetFreeSfxSource()
    {
        foreach (var s in sfxSources)
            if (!s.isPlaying) return s;
        return sfxSources[UnityEngine.Random.Range(0, sfxSources.Count)];
    }


    /* … later … reset when a new round starts … */
    public void ResetAudio()
    {
        gameOver = false;
        musicSource.UnPause();
    }

    public void PlayLoop(Sound id, float vol = 1f)
    {
        // already playing → do nothing
        if (activeLoops.ContainsKey(id))
            return;

        if (!sfx.TryGetValue(id, out var clips) || clips.Length == 0)
            return;

        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        var src = GetFreeSfxSource();
        src.clip = clip;
        src.loop = true;
        src.volume = vol;
        src.Play();

        activeLoops[id] = src;
    }


    public void StopLoop(Sound id)
    {
        if (!activeLoops.TryGetValue(id, out var src))
            return;

        try
        {
            src.loop = false;
            src.Stop();
            src.clip = null;

            activeLoops.Remove(id);
        }

        catch (Exception ex)
        {
            Debug.LogError("NO NEED TO TOUCH: " + ex.Message);
        }

    }


}