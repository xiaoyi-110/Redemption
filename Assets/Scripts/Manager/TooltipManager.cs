using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoSingleton<TooltipManager>
{
    [SerializeField] private TooltipUI tooltipUI;
    public Vector2 fixedPosition;

    protected override void Awake()
    {
        base.Awake();
        tooltipUI.gameObject.SetActive(false);

        fixedPosition = new Vector2(0, 0);
    }

    public void Show(string text)
    {
        if (tooltipUI == null) return;

        tooltipUI.SetText(text);
        tooltipUI.gameObject.SetActive(true);

        tooltipUI.GetComponent<RectTransform>().anchoredPosition = fixedPosition;
    }
    public void Hide()
    {
        if (tooltipUI != null)
        {
            tooltipUI.gameObject.SetActive(false);
        }
    }
}
