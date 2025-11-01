using Cysharp.Threading.Tasks;
using UnityEngine;

public class DoorFaultHandler : MonoBehaviour
{
    private MetroDoor _door;
    private MazeLoader _mazeLoader;
    public bool IsSolvingPuzzle { get; private set; } = false;

    private void Awake()
    {
        _door = GetComponent<MetroDoor>();
        _mazeLoader = GetComponent<MazeLoader>();
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent<OnPuzzleCompleteEventArgs>(OnPuzzleCompleteHandler);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnRegisterEvent<OnPuzzleCompleteEventArgs>(OnPuzzleCompleteHandler);
    }

    private void OnPuzzleCompleteHandler(object sender, OnPuzzleCompleteEventArgs args)
    {
        if (args.Door != _door)
            return;
        IsSolvingPuzzle = false;

        if (args.Success)
        {
            if (_door.currentFault == MetroDoor.FaultType.Type1 || _door.currentFault == MetroDoor.FaultType.Type2)
            {
                UIManager.Instance.OpenPanel("MessagePanel", "解谜成功，门变为卡住状态!").Forget();
                _door.SetState(MetroDoor.DoorState.Jammed);
            }
        }
        else
        {
            UIManager.Instance.OpenPanel("MessagePanel", "解谜失败，门状态不变!").Forget();
        }
    }

    public void HandleFaultUpdate(PlayerController player)
    {
        if (IsSolvingPuzzle)
            return;
        if (PlayerEquipmentManager.Instance.EquippedItem == null && _door.currentState == MetroDoor.DoorState.Closed)
        {
            switch (_door.currentFault)
            {
                case MetroDoor.FaultType.Type1:
                    StartArrowPuzzle();
                    break;
                case MetroDoor.FaultType.Type2:
                    StartMazePuzzleAsync().Forget();
                    break;
                default:
                    break;
            }
        }
    }

    private void StartArrowPuzzle()
    {
        //Debug.Log("进入QQ炫舞解谜界面...");
        IsSolvingPuzzle = true;
        if (ArrowManager.Instance != null)
        {
            ArrowManager.Instance.CreateWave(10, _door);
        }
        else
        {
            Debug.LogError("ArrowManager instance is null");
        }

    }

    private async UniTask StartMazePuzzleAsync()
    {
        IsSolvingPuzzle = true;
        bool success = false;

        await _mazeLoader.LoadMazeSceneAsync();

        if (MazeManager.Instance != null)
        {
            success = await MazeManager.Instance.StartMazePuzzleAsync();
        }
        else
        {
            Debug.LogError("MazeManager instance is null");
        }

        await _mazeLoader.UnloadMazeSceneAsync(); // 卸载场景

        IsSolvingPuzzle = false;
        EventManager.Instance.TriggerEvent(this, OnPuzzleCompleteEventArgs.Create(success, _door));
    }

    public async UniTask HandleMazePuzzleWithNoChangeAsync(MetroDoor door)
    {
        // 调用新的 UniTask 方法并等待结果
        bool success = await StartNoChangeMazePuzzleAsync();

        if (success)
        {
            // Debug.Log("迷宫解谜完成，后续操作执行！");
            door.currentFault = MetroDoor.FaultType.Type1;
            IsSolvingPuzzle = false;
        }
        else
        {
            // Debug.Log("解谜失败，不执行后续操作");
            IsSolvingPuzzle = false;
        }
    }
    public async UniTask<bool> StartNoChangeMazePuzzleAsync()
    {
        IsSolvingPuzzle = true;
        bool success = false;

        await _mazeLoader.LoadMazeSceneAsync(); // 加载场景

        if (MazeManager.Instance != null)
        {
            success = await MazeManager.Instance.StartMazePuzzleAsync();

            if (success)
            {
                UIManager.Instance.OpenPanel("MessagePanel", "按下F进入下一解谜阶段").Forget();
            }
            else
            {
                UIManager.Instance.OpenPanel("MessagePanel", "迷宫解谜失败！").Forget();
            }
        }
        else
        {
            Debug.LogError("MazeManager 实例未找到！");
        }

        await _mazeLoader.UnloadMazeSceneAsync(); // 卸载场景

        IsSolvingPuzzle = false;
        return success;
    }
}