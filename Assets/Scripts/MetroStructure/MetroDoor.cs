using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("摄像机距离检测")]
    public Camera mainCamera;
    public float minDistance = 12f;


    private Animator anim;
    private MazeLoader mazeLoader;
    public DoorFaultHandler faultHandler;

    private PlayerController player;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (mainCamera == null)
            mainCamera = Camera.main;
        mazeLoader = GetComponent<MazeLoader>();
        player = FindObjectOfType<PlayerController>();
        faultHandler = GetComponent<DoorFaultHandler>();
    }

    void Start()
    {
        currentState= DoorState.Closed;
        anim.SetInteger("DoorState", (int)currentState);
    }

    void Update()
    {
        anim.SetInteger("DoorState", (int)currentState);
    }

    public void SetState(DoorState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
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
        if (faultHandler.IsSolvingPuzzle)
        {
            //Debug.Log("谜题进行中，无法交互");
            return;
        }

        if (!isPowered)
        {
            if (PlayerEquipmentManager.Instance.EquippedItem == null&&(currentFault == FaultType.Type3 || currentFault == FaultType.Type4||currentFault==FaultType.Type5))
            {
                UIManager.Instance.OpenPanel("MessagePanel", "需要备用电池才能修复门").Forget();
                return;
            }
        }

        faultHandler.HandleFaultUpdate(player);
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

    public void OpenDoor()
    {
        if (currentState == DoorState.Jammed)
        {
            SetState(DoorState.Open);
        }
    }    

}
