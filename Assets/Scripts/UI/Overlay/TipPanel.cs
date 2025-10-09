using UnityEngine;
using UnityEngine.UI;

public class TipPanel : UIBase
{
    public override UIType UIType => UIType.Overlay;
    public override string PanelName => "TipPanel";

    [SerializeField] private Text tipText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        else
        {
            Debug.LogWarning("TipPanel: CloseButton Î´°ó¶¨£¡");
        }
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        gameObject.SetActive(true);

        if (args.Length > 0 && args[0] is string tip)
        {
            tipText.text = tip;
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        gameObject.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        UIManager.Instance.ClosePanel(PanelName);
    }
}
