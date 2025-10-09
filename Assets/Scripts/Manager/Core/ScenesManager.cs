using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoSingleton<ScenesManager>
{
    private List<string> m_CurrentLoadedScenes = new List<string>();


    public void LoadScene(string sceneName, object userData)
    {
        if (m_CurrentLoadedScenes.Contains(sceneName))
        {
            return;
        }
        StartCoroutine(LoadSceneAsync(sceneName, userData));

    }
    public void UnLoadAllScenes()
    {
        for (int i = 0; i < m_CurrentLoadedScenes.Count; i++)
        {
            SceneManager.UnloadSceneAsync(m_CurrentLoadedScenes[i]);
        }

        m_CurrentLoadedScenes.Clear();
    }

    IEnumerator LoadSceneAsync(string sceneName, object userData)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return asyncOperation;

        m_CurrentLoadedScenes.Add(sceneName);

        //LoadSceneSuccessEventArgs loadSceneSuccessEventArgs = LoadSceneSuccessEventArgs.Create(sceneName, userData);
        //EventManager.Instance.TriggerEvent(LoadSceneSuccessEventArgs.EventId, this, loadSceneSuccessEventArgs);
    }
}