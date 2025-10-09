using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Button))]

public class UIButtonBinder : MonoBehaviour
{
    public string panelName;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(()=>OnButtonClick().Forget());
        }
    }

    private async UniTaskVoid OnButtonClick()
    {
        if (string.IsNullOrEmpty(panelName))
        {
            Debug.LogWarning($"[UIButtonOpenPanel] {gameObject.name} isn't set panelName");
            return;
        }

        await UIManager.Instance.OpenPanel(panelName);
    }
}
