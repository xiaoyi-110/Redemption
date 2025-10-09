using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("UI Canvas Layers")]
    public Canvas HUDCanvas;
    public Canvas WindowCanvas;
    public Canvas PopupCanvas;
    public Canvas SystemCanvas;
    public Canvas OverlayCanvas;

    private readonly Dictionary<string, UIBase> openedPanels = new Dictionary<string, UIBase>();
    private readonly Dictionary<string, UIBase> residentPanels = new Dictionary<string, UIBase>();
    private readonly Dictionary<string, AsyncOperationHandle<GameObject>> loadedHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();
    private readonly Dictionary<string, Queue<UIBase>> uiPools = new Dictionary<string, Queue<UIBase>>();

    public void Init()
    {
        UIBase[] uis = FindObjectsOfType<UIBase>(true);
        foreach (var ui in uis)
        {
            if (!residentPanels.ContainsKey(ui.PanelName))
            {
                residentPanels.Add(ui.PanelName, ui);
                ui.gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 异步打开 UI 面板。用于动态加载或对象池中没有可用实例的UI。
    /// 兼容常驻UI
    /// </summary>
    public async UniTask<UIBase> OpenPanel(string panelName, params object[] args)
    {
        // 1. 检查面板是否已经打开
        if (openedPanels.TryGetValue(panelName, out var ui))
        {
            ui.OnOpen(args);
            return ui;
        }

        // 2. 检查面板是否为常驻 UI
        if (residentPanels.TryGetValue(panelName, out ui))
        {
            ui.gameObject.SetActive(true);
            ui.OnOpen(args);
            openedPanels[panelName] = ui;
            return ui;
        }

        // 3. 尝试从对象池拿
        if (uiPools.TryGetValue(panelName, out var pool) && pool.Count > 0)
        {
            ui = pool.Dequeue();
            ui.gameObject.SetActive(true);
            SetParentCanvas(ui.UIType, ui.transform);
            ui.OnOpen(args);
            openedPanels[panelName] = ui;
            return ui;
        }

        // 4. 对象池里没有，动态加载并实例化
        return await LoadAndInstantiateUIAsync(panelName, args);
    }

    private async UniTask<UIBase> LoadAndInstantiateUIAsync(string panelName, object[] args)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(panelName);
        await handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"无法实例化 UI：'{panelName}'");
            return null;
        }

        GameObject uiInstance = handle.Result;
        UIBase uiBase = uiInstance.GetComponent<UIBase>();

        if (uiBase == null)
        {
            Debug.LogError($"'{panelName}' 缺少 UIBase 组件。");
            Addressables.ReleaseInstance(uiInstance);
            return null;
        }

        SetParentCanvas(uiBase.UIType, uiInstance.transform);

        if (!openedPanels.ContainsKey(panelName))
            openedPanels.Add(panelName, uiBase);

        if (!loadedHandles.ContainsKey(panelName))
            loadedHandles[panelName] = handle;

        uiBase.OnOpen(args);
        return uiBase;
    }

    public void ClosePanel(string panelName)
    {
        if (!openedPanels.ContainsKey(panelName)) return;

        UIBase ui = openedPanels[panelName];
        ui.OnClose();
        openedPanels.Remove(panelName);

        if (residentPanels.ContainsKey(panelName))
        {
            ui.gameObject.SetActive(false);
        }
        else
        {
            // 临时 UI：放回对象池
            if (!uiPools.ContainsKey(panelName))
                uiPools[panelName] = new Queue<UIBase>();

            ui.gameObject.SetActive(false);
            uiPools[panelName].Enqueue(ui);
        }
    }

    private void CloseAllPanels()
    {
        List<string> panelsToClose = new List<string>(openedPanels.Keys);
        foreach (string panelName in panelsToClose)
        {
            ClosePanel(panelName);
        }
    }

    private void SetParentCanvas(UIType type, Transform uiTransform)
    {
        Canvas parentCanvas = null;
        switch (type)
        {
            case UIType.HUD: parentCanvas = HUDCanvas; break;
            case UIType.Window: parentCanvas = WindowCanvas; break;
            case UIType.Popup: parentCanvas = PopupCanvas; break;
            case UIType.System: parentCanvas = SystemCanvas; break;
            case UIType.Overlay: parentCanvas = OverlayCanvas; break;
        }

        if (parentCanvas != null)
            uiTransform.SetParent(parentCanvas.transform, false);
        else
            Debug.LogError($"UIManager: 未找到 UIType '{type}' 对应的 Canvas。");
    }

    public bool IsPanelOpen(string panelName)
    {
        return openedPanels.ContainsKey(panelName);
    }

    public void OnBackToMenu()
    {
        CloseAllPanels();
        OpenPanel("StartPanel").Forget();
    }

    public UIBase GetOpenedPanel(string panelName)
    {
        if (openedPanels.ContainsKey(panelName))
            return openedPanels[panelName];
        return null;
    }

    public void ShowOptions(string[] options)
    {
        DialoguePanel dialoguePanel = GetOpenedPanel("DialoguePanel") as DialoguePanel;
        if (dialoguePanel != null)
            dialoguePanel.ShowOptions(options);
        else
            Debug.LogError("试图显示对话选项，但 DialoguePanel 并未打开。");
    }

    // 可以在游戏退出时调用这个方法来释放所有对象池中的UI
    public void ReleaseAllPooledUI()
    {
        foreach (var kvp in loadedHandles)
        {
            Addressables.ReleaseInstance(kvp.Value);
        }
        loadedHandles.Clear();
        uiPools.Clear();
    }
}
