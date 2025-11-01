using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.SceneManagement;

public class MazeLoader : MonoBehaviour
{
    private const string MazeSceneName = "Maze";

    public async UniTask LoadMazeSceneAsync()
    {
        await SceneManager.LoadSceneAsync(MazeSceneName, LoadSceneMode.Additive).ToUniTask();

        Scene mazeScene = SceneManager.GetSceneByName(MazeSceneName);
        if (mazeScene.isLoaded)
        {
            CameraManager.Instance.OnSceneLoaded(mazeScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(mazeScene);
        }
    }

    public async UniTask UnloadMazeSceneAsync()
    {
        Scene mazeScene = SceneManager.GetSceneByName(MazeSceneName);
        if (mazeScene.isLoaded)
        {
            await SceneManager.UnloadSceneAsync(mazeScene).ToUniTask();
            await UniTask.SwitchToMainThread();
            await Resources.UnloadUnusedAssets();
        }
    }
}
