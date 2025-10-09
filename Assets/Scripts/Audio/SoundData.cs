using UnityEngine;

[System.Serializable]
public class SoundData
{
    public string id;
    public AudioClip clip;
    public AudioGroup group;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 2f)] public float pitch = 1f;
    public bool loop = false;
}
