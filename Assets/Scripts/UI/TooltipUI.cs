using UnityEngine;
using UnityEngine.UI;
public class TooltipUI : MonoBehaviour
{
    [SerializeField] private Text tooltipText;

    public void SetText(string text)
    {
        if (tooltipText == null) return;

        tooltipText.text = text;
    }
    }
