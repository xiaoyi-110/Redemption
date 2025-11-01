using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
public class MazeManager : MonoBehaviour
{
    public static MazeManager Instance;
    private Action onMazeComplete; // 迷宫解谜成功后的回调
    private Action onMazeFailed; // 迷宫解谜失败后的回调

    public MazeGenerator mazeGenerator; // 迷宫生成器
    public MazePlayer player; // 监听玩家解谜进度
    [SerializeField] public GameObject playerPrefab;
    private void Awake()
    {
        Instance = this;
    }

    private void DestroyMaze()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // 销毁子物体
        }
    }

    public UniTask<bool> StartMazePuzzleAsync()
    {
        // 1. 生成迷宫和玩家
        player = mazeGenerator.GenerateMaze();

        // 2. 创建 UniTaskCompletionSource 来等待结果
        var tcs = new UniTaskCompletionSource<bool>();

        // 3. 设置 MazePlayer 的回调，当玩家退出时设置结果
        player.OnMazeExit = (success) =>
        {
            if (tcs.Task.Status == UniTaskStatus.Pending)
            {
                // 设置 UniTask 结果，这将解除 MetroDoor 中的 await
                tcs.TrySetResult(success);

                // 在返回结果后，立即清理迷宫
                DestroyMaze();
            }

        };

        // 4. 返回 UniTask，等待结果
        return tcs.Task;
    }

}