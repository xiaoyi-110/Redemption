using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class InteractionManager : MonoSingleton<InteractionManager>
{
    public class InteractRequest
    {
        public string text;
        public Vector3 worldPosition;
        public int priority;
        public object source;
    }

    private List<InteractRequest> activeRequests = new List<InteractRequest>();
    private InteractRequest currentBestRequest;

    private void Update()
    {
        if (Time.frameCount % 5 == 0)
        {
            UpdateBestInteract();
        }
        if (currentBestRequest != null && UIManager.Instance.IsPanelOpen("InteractUI"))
        {
            var interactUI = UIManager.Instance.GetOpenedPanel("InteractUI") as InteractUI;
            if (interactUI != null)
            {
                interactUI.Refresh(currentBestRequest);
            }
        }
    }

    public void RegisterInteract(InteractRequest request)
    {
        if (!activeRequests.Contains(request))
        {
            activeRequests.Add(request);
            UpdateBestInteract();
        }
    }

    public void UnregisterInteract(InteractRequest request)
    {
        if (activeRequests.Contains(request))
        {
            activeRequests.Remove(request);
            UpdateBestInteract();
        }
    }

    private void UpdateBestInteract()
    {
        bool isBlockingUIOpen = UIManager.Instance.IsPanelOpen("DialoguePanel") ||
                                UIManager.Instance.IsPanelOpen("SettingsPanel") ||
                                UIManager.Instance.IsPanelOpen("InventoryPanel");

        if (activeRequests.Count == 0 || isBlockingUIOpen)
        {
            if (currentBestRequest != null)
            {
                currentBestRequest = null;
                UIManager.Instance.ClosePanel("InteractUI");
            }
            return;
        }

        var validRequests = activeRequests.Where(r => {
            if (r.source is CombustibleItem ci)
                return !ci.isBurning;
            return true;
        }).ToList();

        if (validRequests.Count == 0)
        {
            UIManager.Instance.ClosePanel("InteractUI");
            return;
        }

        var bestRequest = validRequests
            .OrderByDescending(r => r.priority)
            .FirstOrDefault();

        if (bestRequest != currentBestRequest)
        {
            currentBestRequest = bestRequest;
            UIManager.Instance.OpenPanel("InteractUI", currentBestRequest).Forget();
        }
    }
}