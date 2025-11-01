using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetManager : MonoSingleton<AssetManager>
{
    private List<ResourceRequest> _LoadingTasks = new List<ResourceRequest>();

    private Action _OnAllTasksCompleted;

    public SoundDatabase LoadedSoundDatabase { get; private set; }
    public TextAsset LoadedGameMessagesJson { get; private set; }
    public void LoadInitialAssetsAsync(Action onComplete)
    {
        _OnAllTasksCompleted = onComplete;
        _LoadingTasks.Clear();

        ResourceRequest soundDatabaseRequest = Resources.LoadAsync<SoundDatabase>("DataTable/SoundDatabase");
        _LoadingTasks.Add(soundDatabaseRequest);
        ResourceRequest gameMessagesRequest = Resources.LoadAsync<TextAsset>("DataTable/GameMessages");
        _LoadingTasks.Add(gameMessagesRequest);
        StartCoroutine(CheckLoadingProgress());
    }

    private IEnumerator CheckLoadingProgress()
    {
        foreach (var task in _LoadingTasks)
        {
            yield return task;
        }

        ProcessLoadedAssets();

        if (_OnAllTasksCompleted != null)
        {
            _OnAllTasksCompleted.Invoke();
        }

        _LoadingTasks.Clear();
    }

    private void ProcessLoadedAssets()
    {
        foreach (var request in _LoadingTasks)
        {
            if (request.asset is SoundDatabase soundDatabase)
            {
                LoadedSoundDatabase = soundDatabase;
            }
            else if (request.asset is TextAsset textAsset)
            {
                if (request.asset.name == "GameMessages") 
                {
                    LoadedGameMessagesJson = textAsset;
                }
            }
            }
    }
}