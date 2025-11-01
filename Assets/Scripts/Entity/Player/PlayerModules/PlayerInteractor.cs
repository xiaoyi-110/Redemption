using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InteractableObject;
using static PlayerController;

public class PlayerInteractor : MonoBehaviour
{
    public PlayerController player;

    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;

    public InteractableObject nearestInteractable;
    public bool awaitingSecondFPress = false;
    public MetroDoor poweredDoor = null;
    public event System.Action<MetroDoor> OnDoorPowered;
    private Dictionary<System.Type, InteractionManager.InteractRequest> activeRequests = new();

    private Collider2D[] colliderNpcCache = new Collider2D[10];
    private Collider2D[] colliderItemCache = new Collider2D[10];
    private Collider2D[] colliderDoorCache = new Collider2D[5];
    private Collider2D[] colliderExtinguishableCache = new Collider2D[10];
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void UpdateInteraction()
    {
        HandleInput();
        HandleInteractionInput();
        TryInteractWithNPC();
        CheckInteractables();
        CheckNearestMetroDoor();
        CheckPoweredDoor();
        HandleNPCPutOutFire();
        ManageInteractionRequests();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 input = (horizontal * transform.right + vertical * transform.up).normalized;
        horizontal = input.x;
        vertical = input.y;
    }
    public void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && nearestInteractable)
        {
            nearestInteractable.OnInteract(player);
        }
    }
    public void TryInteractWithNPC()
    {
        player.currentNPC=FindNearestNpc();

        if (player.currentNPC != null && player.currentNPC.gameObject.activeSelf)
        {
            bool dialogueFinished = DialogManager.Instance.finishedNpcIds.Contains(player.currentNPC.npcID);
            string interactText= "[T]交谈";
            if (dialogueFinished && player.currentNPC.CompareTag("UnconsciousNPC")) {
                interactText = "[C]扛起";
            }
            var request = new InteractionManager.InteractRequest
            {
                text =interactText,
                worldPosition = player.currentNPC.transform.position + Vector3.down * 0.5f,
                priority = InteractionPriorities.NPC,
                source = player.currentNPC
            };
            RegisterRequest(typeof(NPC), request);
            if (Input.GetKeyDown(KeyCode.T)) player.currentNPC.Interact(player);
        }
        else
        {
            UnregisterRequest(typeof(NPC));
        }
    }

    public NPC FindCarriableNPC()
    {
        NPC CarriableNPC = null;
        NPC closestNPC=FindNearestNpc();
        if(closestNPC != null&&closestNPC.CompareTag("UnconsciousNPC"))
        {
            CarriableNPC = closestNPC;
        }

        return CarriableNPC;
    }

    public NPC FindNearestNpc()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, interactRadius, colliderNpcCache);
        NPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < count; i++)
        {
            var collider = colliderNpcCache[i];
            NPC npc = collider.GetComponent<NPC>();
            if (npc != null && npc.gameObject.activeSelf)
            {
                float distance = Vector2.Distance(transform.position, npc.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNPC = npc;
                }
            }
        }
        return closestNPC;
    }

    public NPC FindDroppableNPC()
    {
        if (player.carriedNPC != null)
        {
            return player.carriedNPC;
        }
        return null;
    }

    private void HandleNPCPutOutFire()
    {
        if (player.currentNPC != null)
        {
            CombustibleItem burningItem = player.currentNPC.GetComponentInChildren<CombustibleItem>();

            if (burningItem != null && burningItem.isBurning)
            {
                StartCoroutine(ExtinguishAfterDelay(burningItem));
            }
        }
    }

    private IEnumerator ExtinguishAfterDelay(CombustibleItem item)
    {
        yield return new WaitForSeconds(3f);
        item.Extinguish();
    }

    private void CheckInteractables()
    {
       
        nearestInteractable = GetNearestInteractable();

        if (nearestInteractable != null && !(nearestInteractable is CombustibleItem ci && ci.isBurning))
        {
            string text = "[F]拾取";
            Vector3 worldPos = nearestInteractable.transform.position + Vector3.down * 0.5f;

            if (nearestInteractable is Window window)
            {
                text = window.IsBroken() ? "[F]救出NPC" : "敲击";
                worldPos = window.transform.position + Vector3.up * 0.5f;
            }

            if(nearestInteractable is VentExit ventExit)
            {
                text = "[F]进入";
                worldPos = ventExit.transform.position + Vector3.up * 0.5f;
            }

            var request = new InteractionManager.InteractRequest
            {
                text = text,
                worldPosition = worldPos,
                priority = InteractionPriorities.Item,
                source = nearestInteractable
            };

            RegisterRequest(typeof(InteractableObject), request);
        }
        else
        {
            UnregisterRequest(typeof(InteractableObject));
        }
    }

    private InteractableObject GetNearestInteractable()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, interactRadius, colliderItemCache);
        InteractableObject nearest = null;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < count; i++)
        {
            {
                var obj = colliderItemCache[i].GetComponent<InteractableObject>();
                if (!obj) continue;

                if (obj == PlayerEquipmentManager.Instance.EquippedMask || obj == PlayerEquipmentManager.Instance.EquippedItem) continue;

                float dist = Vector2.Distance(transform.position, obj.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = obj;
                }
            }
        }
        return nearest;
    }

    private void CheckNearestMetroDoor()    
    {
        MetroDoor nearestMetroDoor=GetNearestMetroDoor();
        if (nearestMetroDoor != null)
        {
            if ((nearestMetroDoor.currentFault == MetroDoor.FaultType.Type1 ||
                nearestMetroDoor.currentFault == MetroDoor.FaultType.Type2) &&
                nearestMetroDoor.currentState == MetroDoor.DoorState.Closed&&PlayerEquipmentManager.Instance.EquippedItem==null)
            {
                string text = "[F]开门";
                Vector3 worldPos = nearestMetroDoor.transform.position + Vector3.down * 0.5f;

                var request = new InteractionManager.InteractRequest
                {
                    text = text,
                    worldPosition = worldPos,
                    priority = InteractionPriorities.Door,
                    source = nearestMetroDoor
                };

                RegisterRequest(typeof(MetroDoor), request);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    var door = nearestMetroDoor;
                    door.TryInteract();
                }
            }

        }
        else
        {
            UnregisterRequest(typeof(MetroDoor));
        }
    }

    public MetroDoor GetNearestMetroDoor()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, interactRadius, colliderDoorCache);

        MetroDoor closestDoor = null;
        float minDist = Mathf.Infinity;

        for(int i=0;i<count;i++)
        {
            var hit = colliderDoorCache[i];
            if (hit.CompareTag("MetroDoor"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestDoor = hit.GetComponent<MetroDoor>();
                }
            }
        }

        return closestDoor;
    }

    public void SetPoweredDoor(MetroDoor door)
    {
        poweredDoor = door;
        awaitingSecondFPress = true;

        OnDoorPowered?.Invoke(door);
    }
    private async void CheckPoweredDoor()
    {
        if (Input.GetKeyDown(KeyCode.F) && poweredDoor != null)
        {
            if (awaitingSecondFPress)
            {
                var door = poweredDoor;
                if (door.currentFault == MetroDoor.FaultType.Type3)
                {
                    door.currentFault = MetroDoor.FaultType.Type1;
                }
                else if (door.currentFault == MetroDoor.FaultType.Type4)
                {
                    door.currentFault = MetroDoor.FaultType.Type2;
                }
                else if (door.currentFault == MetroDoor.FaultType.Type5)
                {
                    await door.faultHandler.HandleMazePuzzleWithNoChangeAsync(door);
                }

                door.TryInteract();
                awaitingSecondFPress = false;
                poweredDoor = null;
            }
        }
    }
    
    public IExtinguishable FindNearestExtinguishable()
    {
        int count= Physics2D.OverlapCircleNonAlloc(transform.position, interactRadius, colliderExtinguishableCache);
        IExtinguishable nearestExtinguishable = null;
        float closestDistance = Mathf.Infinity;

        for(int i=0;i<count;i++)
        {
            var col = colliderExtinguishableCache[i];
            IExtinguishable extinguishable = col.GetComponent<IExtinguishable>();

            if (extinguishable != null)
            {
                if (extinguishable is CombustibleItem combustible && combustible.isBurning)

                {
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        nearestExtinguishable = extinguishable;
                    }
                }
            }
        }
        return nearestExtinguishable;
    }
    
    
    private void ManageInteractionRequests()
    {
        // 自动清理无效请求
        var keysToRemove = activeRequests.Where(p => p.Value.source == null).Select(p => p.Key).ToList();
        foreach (var key in keysToRemove)
        {
            InteractionManager.Instance.UnregisterInteract(activeRequests[key]);
            activeRequests.Remove(key);
        }
    }

    private void RegisterRequest(System.Type type, InteractionManager.InteractRequest request)
    {
        if (activeRequests.TryGetValue(type, out var existing))
        {
            InteractionManager.Instance.UnregisterInteract(existing);
        }
        InteractionManager.Instance.RegisterInteract(request);
        activeRequests[type] = request;
    }

    private void UnregisterRequest(System.Type type)
    {
        if (activeRequests.TryGetValue(type, out var request))
        {
            InteractionManager.Instance.UnregisterInteract(request);
            activeRequests.Remove(type);
        }
    }
}
