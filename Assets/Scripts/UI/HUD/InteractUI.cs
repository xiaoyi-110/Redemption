using UnityEngine;
using UnityEngine.UI;

public class InteractUI : UIBase
{
    public override UIType UIType => UIType.HUD;
    public override string PanelName => "InteractUI";

    [SerializeField] private Text interactText;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            Debug.LogError("InteractUI: È±ÉÙ RectTransform ×é¼þ");
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
    }

    public void Refresh(InteractionManager.InteractRequest request)
    {
        if (request == null)
        {
            gameObject.SetActive(false);
            return;
        }

        interactText.text = request.text;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(request.worldPosition);

        if (screenPos.z > 0)
        {
            rectTransform.position = screenPos;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
    }
}
