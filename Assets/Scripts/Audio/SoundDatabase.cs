using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Audio/SoundDatabase", order = 1)]
public class SoundDatabase : ScriptableObject
{
    public List<SoundData> sounds = new List<SoundData>();
    private Dictionary<string, SoundData> soundDictionary;

    public void Init()
    {
        soundDictionary = new Dictionary<string, SoundData>();
        foreach (var s in sounds)
        {
            if (!soundDictionary.ContainsKey(s.id))
            {
                soundDictionary.Add(s.id, s);
            }
            else
            {
                Debug.LogWarning($"Sound ID '{s.id}' already exists in the database. Skipping duplicate.");
            }
        }
    }

    public SoundData Get(string id)
    {
        if (soundDictionary == null)
        {
            Init();
        }
        if (soundDictionary.TryGetValue(id, out SoundData soundData))
        {
            return soundData;
        }
        else
        {
            Debug.LogWarning($"Sound ID '{id}' not found in the database.");
            return null;
        }
    }
}
