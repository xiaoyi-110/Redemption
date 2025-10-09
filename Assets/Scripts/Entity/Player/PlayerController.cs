using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : EntityActor
{
    [Header("FSM")]
    private FSM<PlayerController> fsm;
    private List<FSMStateBase<PlayerController>> stateList;

    public PlayerInteractor PlayerInteractor { get; private set; }
    public PlayerAnimation PlayerAnimation { get; private set; }

    public class InteractionPriorities
    {
        public const int NPC = 0;
        public const int Item = 1;
        public const int Door = 2;
    }
    [Header("normal settings")]
    public float walkSpeed = 5f;
    [HideInInspector] public Vector2 movement;
    private CapsuleCollider2D col;
    [HideInInspector] public float speedMultiplier = 1f;

    private PlayerEquipmentManager equipmentManager;
    public PlayerTeleportManager PlayerTeleportManager { get; private set; }
    public NPC currentNPC;
    public NPC carriedNPC;

    public Light FollowLight { get; private set; }
   
    protected override void Awake()
    {
        base.Awake();
        PlayerInteractor = GetComponent<PlayerInteractor>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        col = GetComponent<CapsuleCollider2D>();
        Rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        FollowLight = transform.Find("FollowLight").GetComponent<Light>();

        //Debug.Log("Current Render Pipeline Asset: " + GraphicsSettings.renderPipelineAsset);
        //DontDestroyOnLoad(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        CreateFSM();
        PlayerTeleportManager = PlayerTeleportManager.Instance;
        equipmentManager = PlayerEquipmentManager.Instance;
        if(FollowLight != null)
        {
            FollowLight.gameObject.SetActive(true);
            FollowLight.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.RegisterEvent<TeleportSuccessEventArgs>(OnTeleportSuccessHandler);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UnRegisterEvent<TeleportSuccessEventArgs>(OnTeleportSuccessHandler);
        }
    }
    private void CreateFSM()
    {
        stateList = new List<FSMStateBase<PlayerController>>()
        {
            new PlayerIdleState(),
            new PlayerWalkingState(),
            new PlayerCarryingState(),
            new PlayerIllusionState(),
            new PlayerCrawlingState(),
        };

        fsm = new FSM<PlayerController>(this);

        foreach (var state in stateList)
        {
            fsm.AddState(state);
        }

        fsm.ChangeState<PlayerIdleState>();
    }

    protected override void OnUpdate()
    {
        if (!GameManager.Instance.isGameStarted)
            return;
        if (ArrowManager.Instance.IsInWave())
        {
            Rb.velocity = Vector2.zero;
            return;
        }

        fsm.Update();

        PlayerInteractor.UpdateInteraction();

        HandleInput();
    }


    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        Rb.velocity = movement * walkSpeed * speedMultiplier;
    }

    public bool TryCarryNPC()
    {
        NPC npcToCarry = PlayerInteractor.FindCarriableNPC();

        if (npcToCarry != null)
        {
            carriedNPC = npcToCarry;
            carriedNPC.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    public void TryDropNPC()
    {
        NPC npcToDrop = PlayerInteractor.FindDroppableNPC();

        if (npcToDrop != null)
        {
            npcToDrop.gameObject.SetActive(true);
            npcToDrop.stateMachine.Initialize(carriedNPC.unconsciousState);
            npcToDrop.transform.position = transform.position + new Vector3(5f, 0, 0);
            carriedNPC = null;
        }
    }

    public void RescueNPCFromWindow(Window window)
    {
        if (fsm.currentState is PlayerCarryingState)
        {
            carriedNPC = null;
            GameManager.Instance.rescuedNPC++;
            //Debug.Log("已救出 NPC！");
            fsm.ChangeState<PlayerIdleState>();
            if (GameManager.Instance.rescuedNPC == GameManager.Instance.maxNPC)
            {
                GameManager.Instance.isGameWon = true;
                GameManager.Instance.EndGame();
            }
        }
    }
    private void OnTeleportSuccessHandler(object sender, TeleportSuccessEventArgs args)
    {
        if (args.Player == this)
        {
            if (args.Type == TeleportType.EnterVent)
            {
                fsm.ChangeState<PlayerCrawlingState>();
            }
            else if (args.Type == TeleportType.ExitVent)
            {
                fsm.ChangeState<PlayerIdleState>();
            }
        }
    }

    public void UseVentilation()
    {
        PlayerTeleportManager.Instance.TeleportToVent(this).Forget();
    }

    public void UseVentExit(VentExit ventExit, Transform TargetExitPoint)
    {
        if (fsm.currentState is PlayerCrawlingState)
        {
            if (CanExitVent(ventExit))
            {
                PlayerTeleportManager.TeleportFromVent(this, TargetExitPoint).Forget();
            }
            else
            {
                UIManager.Instance.OpenPanel("MessagePanel","需要手电筒才能离开").Forget();
                //Debug.Log("需要手电筒才能离开");
            }
        }
    }

    private bool CanExitVent(VentExit ventExit)
    {
        // This is a much cleaner way to check the condition.
        // PlayerController checks its own equipped item, not the VentExit.
        if (!ventExit.RequiresFlashlight)
        {
            return true;
        }

        return PlayerEquipmentManager.Instance.EquippedItem is Flashlight flashlight && flashlight.isFlashlightOn;
    }

    private void HandleInput()
    {
        // 1. 处理UI开关输入 (Tab键)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIManager.Instance.IsPanelOpen("InventoryPanel"))
            {
                UIManager.Instance.ClosePanel("InventoryPanel");
                Time.timeScale = 1f; 
            }
            else
            {
                UIManager.Instance.OpenPanel("InventoryPanel").Forget();
                Time.timeScale = 0f; 
            }

            return;
        }

        HandleSettingsToggle();

        // 2. 如果UI已打开，处理背包内的输入
        if (UIManager.Instance.IsPanelOpen("InventoryPanel"))
        {
            HandleInventoryInput();
        }
        // 3. 如果UI已关闭，处理游戏内的物品使用和卸下输入
        else
        {
            HandleGameplayInput();
        }
    }
    private void HandleSettingsToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ToggleSettings();
        }
    }
    private void HandleInventoryInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EventManager.Instance.TriggerEvent(this, EquipItemEventArgs.Create());
        }
    }

    private void HandleGameplayInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (equipmentManager.EquippedItem != null)
            {
                equipmentManager.UnequipItem(equipmentManager.EquippedItem, true);
            }
            if (equipmentManager.EquippedMask != null)
            {
                equipmentManager.UnequipItem(equipmentManager.EquippedMask, true);
            }
        }

        HandleUseItemInput();
    }

    public void HandleUseItemInput()
    {
        if (equipmentManager.EquippedMask != null)
        {
             equipmentManager.EquippedMask.UseItem(this);
        }

        if (equipmentManager.EquippedItem != null)
        {
            switch (equipmentManager.EquippedItem.UseTrigger)
            {
                case UseTrigger.KeyF:
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        //Debug.Log("开始使用物品（按下 F 键）");
                        equipmentManager.EquippedItem.UseItem(this);
                    }
                    break;

                case UseTrigger.RightClick:
                    if (Input.GetMouseButtonDown(1))
                    {
                        //Debug.Log("开始使用物品（鼠标右键）");
                        equipmentManager.EquippedItem.UseItem(this);
                    }
                    break;

                case UseTrigger.OnEquip:
                    equipmentManager.EquippedItem.UseItem(this);
                    break;

                default:
                    //Debug.LogError("未知的使用触发条件！");
                    break;
            }
        }
    }
}