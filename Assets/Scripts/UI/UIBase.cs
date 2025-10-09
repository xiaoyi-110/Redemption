using UnityEngine;

public enum UIType
{
    World,
    HUD,
    Window,
    Popup,
    System,
    Overlay
}

public abstract class UIBase : MonoBehaviour
{
    public abstract UIType UIType { get; }
    public abstract string PanelName { get; }


    public virtual void OnOpen(params object[] args)
    {
        gameObject.SetActive(true);
        //Debug.Log($"UI: {PanelName} Opened.");
    }

    public virtual void OnClose()
    {
        gameObject.SetActive(false);
        //Debug.Log($"UI: {PanelName} Closed.");
    }

    //public virtual void BindData(object data) { }
}