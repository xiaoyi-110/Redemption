using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoSingleton<AudioManager>
{
    private SoundDatabase m_soundDatabase;

    [Header("1")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;

    [Header("2")]
    public int poolSize = 10;
    private Queue<AudioSource> m_audioPool = new Queue<AudioSource>();

    private Dictionary<string, AudioSource> m_activeLoops = new Dictionary<string, AudioSource>();
    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            m_audioPool.Enqueue(audioSource);
        }
    }
    public void SetSoundDatabase(SoundDatabase database)
    {
        if (m_soundDatabase == null)
        {
            m_soundDatabase = database;
            m_soundDatabase.Init();
        }
        else
        {
            Debug.LogWarning("SoundDatabase is null");
        }
    }
    private AudioSource GetSource()
    {
        if (m_audioPool.Count > 0)
        {
            return m_audioPool.Dequeue();
        }
        else
        {
            return gameObject.AddComponent<AudioSource>();
        }
    }

    private void ReturnSource(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.clip = null;
            source.loop = false;
            m_audioPool.Enqueue(source);
        }
    }

    public void Play(string id)
    {
        SoundData soundData = m_soundDatabase.Get(id);
        if (soundData == null)
        {
            Debug.LogWarning($"Sound with ID '{id}' not found in the database.");
            return;
        }
        if (soundData.loop)
        {
            if (m_activeLoops.ContainsKey(id)) return;
            AudioSource source = GetSource();
            ApplySoundData(source, soundData);
            source.Play();
            m_activeLoops[id] = source;
        }
        else
        {
            StartCoroutine(PlayOnShot(soundData));
        }
    }

    private IEnumerator PlayOnShot(SoundData soundData)
    {
        AudioSource source = GetSource();
        ApplySoundData(source, soundData);
        source.Play();
        yield return new WaitForSeconds(soundData.clip.length / source.pitch);
        ReturnSource(source);
    }

    private void ApplySoundData(AudioSource source, SoundData soundData)
    {
        source.clip = soundData.clip;
        source.volume = soundData.volume * GetVolumeByGroup(soundData.group) * masterVolume;
        source.pitch = soundData.pitch;
        source.loop = soundData.loop;
    }

    public void Stop(string id)
    {
        if (m_activeLoops.TryGetValue(id, out AudioSource source))
        {
            source.Stop();
            ReturnSource(source);
            m_activeLoops.Remove(id);
        }
        else
        {
            Debug.LogWarning($"No active loop found for sound ID '{id}'.");
        }
    }

    private float GetVolumeByGroup(AudioGroup group)
    {
        return group switch
        {
            AudioGroup.BGM => bgmVolume,
            AudioGroup.SFX => sfxVolume,
            AudioGroup.UI => uiVolume,
            _ => 1f
        };
    }

    public IEnumerator FadeIn(string id, float duration)
    {
        Play(id);
        if (!m_activeLoops.TryGetValue(id, out AudioSource source))
        {
            Debug.LogWarning($"No active loop found for sound ID '{id}' to fade in.");
            yield break;
        }

        source.volume = 0;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0, masterVolume * GetVolumeByGroup(AudioGroup.BGM), t / duration);
            yield return null;
        }
    }
}

