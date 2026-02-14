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
    PointsCountUp,
    HitTarget
}

[Serializable]
public struct SoundEntry
{
    public Sound id;
    public AudioClip[] clips;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Music playlist")]
    public AudioClip[] musicClips;
    [Range(0f, 3f)] public float musicFade = .5f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;

    [Header("SFX Volume")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("SFX list")]
    public SoundEntry[] sfxEntries;

    [Range(1, 32)] public int sfxVoices = 8;
    public AudioMixerGroup musicBus, sfxBus;

    readonly Dictionary<Sound, AudioClip[]> sfx = new();
    readonly List<AudioSource> sfxSources = new();

    AudioSource musicSource, b;
    int lastMusic = -1;
    bool toggle = false;

    bool gameOver = false;
    bool gamePaused = false;

    readonly Dictionary<Sound, AudioSource> activeLoops = new();

    const string MUSIC_PARAM = "MusicVolume";
    const string SFX_PARAM = "SFXVolume";

    const string MUSIC_PREF = "MusicVolume";
    const string SFX_PREF = "SFXVolume";

    void Start()
    {
        StartCoroutine(MusicLoop());
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build SFX dictionary
        foreach (var e in sfxEntries)
            if (e.clips != null && e.clips.Length > 0)
                sfx[e.id] = e.clips;

        // Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicBus;
        musicSource.loop = true;

        // Load saved volumes
        musicVolume = PlayerPrefs.GetFloat(MUSIC_PREF, musicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SFX_PREF, sfxVolume);

        ApplyMusicVolume();
        ApplySfxVolume();

        // Start random music
        if (musicClips.Length > 0)
        {
            lastMusic = UnityEngine.Random.Range(0, musicClips.Length);
            musicSource.clip = musicClips[lastMusic];
            musicSource.Play();
        }

        // Create SFX pool
        for (int i = 0; i < sfxVoices; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxBus;
            sfxSources.Add(src);
        }
    }

    // ================= MUSIC =================

    void ApplyMusicVolume()
    {
        if (musicBus == null) return;

        float db = Mathf.Log10(Mathf.Max(musicVolume, 0.0001f)) * 20f;
        musicBus.audioMixer.SetFloat(MUSIC_PARAM, db);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyMusicVolume();
        PlayerPrefs.SetFloat(MUSIC_PREF, musicVolume);
        PlayerPrefs.Save();
    }

    IEnumerator MusicLoop()
    {
        while (true)
        {
            int nextIdx = PickNewTrack();
            if (nextIdx < 0) yield break;

            yield return new WaitForSeconds(
                Mathf.Max(0f, musicSource.clip.length - musicFade)
            );

            musicSource.clip = musicClips[nextIdx];
            musicSource.Play();
        }
    }

    int PickNewTrack()
    {
        if (musicClips == null || musicClips.Length == 0)
            return -1;

        int idx;
        if (musicClips.Length > 1)
        {
            do
            {
                idx = UnityEngine.Random.Range(0, musicClips.Length);
            }
            while (idx == lastMusic);
        }
        else idx = 0;

        lastMusic = idx;
        return idx;
    }

    // ================= SFX =================

    void ApplySfxVolume()
    {
        if (sfxBus == null) return;

        float db = Mathf.Log10(Mathf.Max(sfxVolume, 0.0001f)) * 20f;
        sfxBus.audioMixer.SetFloat(SFX_PARAM, db);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplySfxVolume();
        PlayerPrefs.SetFloat(SFX_PREF, sfxVolume);
        PlayerPrefs.Save();
    }

    // ================= PLAYING SOUNDS =================

    public void PlaySound(Sound id, float vol = 1f)
    {
        if (!sfx.TryGetValue(id, out var clips) || clips.Length == 0)
            return;

        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        GetFreeSfxSource().PlayOneShot(clip, vol);
    }

    AudioSource GetFreeSfxSource()
    {
        foreach (var s in sfxSources)
            if (!s.isPlaying) return s;

        return sfxSources[UnityEngine.Random.Range(0, sfxSources.Count)];
    }

    // ================= LOOPS =================

    public void PlayLoop(Sound id, float vol = 1f)
    {
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

        src.loop = false;
        src.Stop();
        src.clip = null;
        activeLoops.Remove(id);
    }

    // ================= PAUSE =================

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

    public void ResetAudio()
    {
        gameOver = false;
        musicSource.UnPause();
    }
}
