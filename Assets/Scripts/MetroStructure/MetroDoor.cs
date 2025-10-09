using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
public class MetroDoor : MonoBehaviour
{
    public enum DoorState { Open, Closed, Jammed }
    public enum FaultType { None, Type1, Type2, Type3, Type4, Type5 }

    [Header("门的状态")]
    public DoorState currentState = DoorState.Closed;
    public FaultType currentFault = FaultType.None;
    private bool isPowered = false;

    [Header("门的属性")]
    public float openSpeed = 1.0f;
    public AudioClip openSound;

    [Header("摄像机距离检测")]
    public Camera mainCamera;
    public float minDistance = 12f;
    //private SpriteRenderer[] spriteRenderers;


    private Animator anim;
    private AudioSource audioSource;

    private float arrowDuration = 5f;
    private float arrowTime = 0;
    public bool isDancing;
    public int correctInputs;
    private bool isSolvingPuzzle = false;

    private PlayerController player;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (ArrowManager.Instance == null)
            //ArrowManager.Instance = FindObjectOfType<ArrowManager>();

        if (MazeManager.instance == null)
            MazeManager.instance = FindObjectOfType<MazeManager>();

        player = FindObjectOfType<PlayerController>();
    }


    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentState= DoorState.Closed;
        anim.SetInteger("DoorState", (int)currentState);
    }

    void Update()
    {
        if (isDancing)
        {
            arrowTime += Time.deltaTime;
            if (arrowTime > arrowDuration)
            {
                FinishWave();
            }
        }
        anim.SetInteger("DoorState", (int)currentState);
    }


    public void SetPowered(bool value)
    {
        isPowered = value;
    }

    public void HandlePower()
    {

            if (currentFault == MetroDoor.FaultType.Type3 ||
                currentFault == MetroDoor.FaultType.Type4 ||
                currentFault == MetroDoor.FaultType.Type5)
            {
                SetPowered(true);
                player.PlayerInteractor.SetPoweredDoor(this);
                //Debug.Log("车门已通电，请再次按下 F 以修复故障");
                UIManager.Instance.OpenPanel("MessagePanel", "车门已通电，请再次按下 F 以修复故障").Forget();
            }
    }
    public void TryInteract()
    {
        if (isSolvingPuzzle)
        {
            //Debug.Log("谜题进行中，无法交互");
            return;
        }

        if (!isPowered)
        {
            if (PlayerEquipmentManager.Instance.EquippedItem == null&&(currentFault == FaultType.Type3 || currentFault == FaultType.Type4||currentFault==FaultType.Type5))
            {
                UIManager.Instance.OpenPanel("MessagePanel", "需要备用电池才能修复门").Forget();
                //Debug.Log("需要备用电池才能修复门！");
                return;
            }
        }

        HandleFaultUpdate(player);
    }

    public void TryOpenWithCrowbar()
    {
        if (currentFault == FaultType.Type1 || currentFault == FaultType.Type2)
        {
            Debug.Log("车门已打开！");
            OpenDoor(); 
        }
        else
        {
            Debug.Log("这个门无法用撬棍打开！");
        }
    }
    private void HandleFaultUpdate(PlayerController player)
    {
        if (isSolvingPuzzle)
            return;
        if (PlayerEquipmentManager.Instance.EquippedItem == null&&currentState==DoorState.Closed)
        {
            switch (currentFault)
            {
                case FaultType.Type1:
                    StartArrowPuzzle();
                    break;
                case FaultType.Type2:
                    StartMazePuzzle();
                    break;
                default:
                    break;
            }
        }
    }


    public void OpenDoor()
    {
        if (currentState == DoorState.Jammed)
        {
            //Debug.Log("门正在打开...");
            currentState = DoorState.Open;
            anim.SetInteger("DoorState", (int)currentState);
            if (openSound)
                audioSource.PlayOneShot(openSound);
        }
        else
        {
            //Debug.Log("门无法打开！");
        }
    }
    #region Arrow
    private void StartArrowPuzzle()
    {
        //Debug.Log("进入QQ炫舞解谜界面...");
        isSolvingPuzzle = true;
        arrowTime = 0f;
        if (ArrowManager.Instance != null)
        {
            ArrowManager.Instance.CreateWave(10, this);
        }
        else
        {
            Debug.LogError("ArrowManager instance is null");
        }
        isDancing = true;
        correctInputs = 0;
    }

    public void FinishWave()
    {
        isDancing = false;
        isSolvingPuzzle = false;
        ArrowManager.Instance?.ClearWave();
        //Debug.Log($"结束解谜，正确数为 {correctInputs}");
        UIManager.Instance.OpenPanel("MessagePanel", "解谜失败，门状态不变!").Forget();

        if (correctInputs == 10)
        {
            //Debug.Log("QQ炫舞解谜成功，门变为撬棍状态");
            UIManager.Instance.OpenPanel("MessagePanel", "解谜成功，门变为卡住状态!").Forget();
            currentState = DoorState.Jammed;
            anim.SetInteger("DoorState", (int)currentState);
        }
    }

    public void RecordInput(bool isCorrect)
    {
        if (isCorrect)
        {
            correctInputs++;
        }
    }
    #endregion

    #region Maze
    private void StartMazePuzzle()
    {
        //Debug.Log("进入迷宫解谜界面...");
        isSolvingPuzzle = true;
        //Camera Add new overlay camera.
        SceneManager.sceneLoaded += OnMazeSceneLoaded;
        SceneManager.LoadScene("Maze", LoadSceneMode.Additive);
    }

    private void OnMazeSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
           // Debug.Log("迷宫场景加载完成，开始迷宫解谜逻辑");

            CameraManager.instance.OnSceneLoaded(scene, mode);
            MazeManager.instance?.StartMazePuzzle(OnMazeSolved, OnMazeFailed);
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnMazeSceneLoaded;
        }
    }
    public IEnumerator HandleMazePuzzleWithNoChange(MetroDoor door)
    {
        yield return StartCoroutine(door.StartMazePuzzleWithNoChange((success) =>
        {
            if (success) // 新增条件判断
            {
                //Debug.Log("迷宫解谜完成，后续操作执行！");
                door.currentFault = MetroDoor.FaultType.Type1;
                isSolvingPuzzle = false;
            }
            else
            {
                //Debug.Log("解谜失败，不执行后续操作");
                isSolvingPuzzle = false;
            }
        }));

    }
    public IEnumerator StartMazePuzzleWithNoChange(Action<bool> onCompleted) { 

       // Debug.Log("进入迷宫解谜界面但不会影响门状态...");
        isSolvingPuzzle = true;
        // 定义局部回调函数
        bool isCompleted = false;
        Action onLocalSolved = () =>
        {
            //Debug.Log("迷宫解谜成功，但门状态不变");
            UIManager.Instance.OpenPanel("MessagePanel", "按下F进入下一解谜阶段").Forget();
            isCompleted = true;
            onCompleted?.Invoke(true);
        };

        Action onLocalFailed = () =>
        {
            //Debug.Log("迷宫解谜失败！");
            isCompleted = true;
            onCompleted?.Invoke(false);
        };

        // 注册一次性场景加载监听
        SceneManager.sceneLoaded += OnMazeSceneLoaded_NoChange;
        SceneManager.LoadScene("Maze", LoadSceneMode.Additive);

        // 等待场景加载并激活
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Maze").isLoaded);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Maze"));

        // 确保只在此处触发解谜逻辑
        if (MazeManager.instance != null)
        {
            MazeManager.instance.StartMazePuzzle(onLocalSolved, onLocalFailed);
        }
        else
        {
            //Debug.LogError("MazeManager 实例未找到！");
            onCompleted?.Invoke(false);
        }

        // 等待解谜完成
        yield return new WaitUntil(() => isCompleted);

        // 清理资源
        SceneManager.sceneLoaded -= OnMazeSceneLoaded_NoChange;
        yield return StartCoroutine(UnloadMazeScene());
    }


    private void OnMazeSceneLoaded_NoChange(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            CameraManager.instance.OnSceneLoaded(scene, mode);
        }
    }
    private void OnMazeSolved()
    {
        //Debug.Log("迷宫解谜成功，门变为卡住状态");
        isSolvingPuzzle = false;
        StartCoroutine(UnloadMazeScene());
        UIManager.Instance.OpenPanel("MessagePanel", "迷宫解谜成功，门变为卡住状态!").Forget();
        currentState = DoorState.Jammed;
        anim.SetInteger("DoorState", (int)currentState);    
    }

    private void OnMazeFailed()
    {
        //Debug.Log("迷宫解谜失败！");
        isSolvingPuzzle = false;
        UIManager.Instance.OpenPanel("MessagePanel", "迷宫解谜失败!").Forget();
        StartCoroutine(UnloadMazeScene());
    }

    private IEnumerator UnloadMazeScene()
    {
        Scene mazeScene = SceneManager.GetSceneByName("Maze");
        if (mazeScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(mazeScene);
            //Debug.Log("迷宫场景已卸载");
            Resources.UnloadUnusedAssets(); // 清理残留资源
        }
    }
    #endregion

}
