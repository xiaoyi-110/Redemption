using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetManager : MonoSingleton<AssetManager>
{
    private List<ResourceRequest> m_LoadingTasks = new List<ResourceRequest>();

    private Action m_OnAllTasksCompleted;

    public SoundDatabase LoadedSoundDatabase { get; private set; }

    public void LoadInitialAssetsAsync(Action onComplete)
    {
        m_OnAllTasksCompleted = onComplete;
        m_LoadingTasks.Clear();

        ResourceRequest soundDatabaseRequest = Resources.LoadAsync<SoundDatabase>("DataTable/SoundDatabase");
        m_LoadingTasks.Add(soundDatabaseRequest);

        StartCoroutine(CheckLoadingProgress());
    }

    private IEnumerator CheckLoadingProgress()
    {
        foreach (var task in m_LoadingTasks)
        {
            yield return task;
        }

        ProcessLoadedAssets();

        if (m_OnAllTasksCompleted != null)
        {
            m_OnAllTasksCompleted.Invoke();
        }

        m_LoadingTasks.Clear();
    }

    private void ProcessLoadedAssets()
    {
        foreach (var request in m_LoadingTasks)
        {
            if (request.asset is SoundDatabase soundDatabase)
            {
                LoadedSoundDatabase = soundDatabase;
            }
        }
    }
}