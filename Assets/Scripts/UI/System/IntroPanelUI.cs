using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroPanelUI : UIBase
{
    public override UIType UIType => UIType.System;
    public override string PanelName => "IntroPanelUI";

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text text;
    [SerializeField] private float fadeDuration = 2;
    [SerializeField] private float waitBeforeFadeOut = 2f;
    [SerializeField] private float panelFadeDuration = 2f;

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);

        string message = args.Length > 0 && args[0] is string ? (string)args[0] : "";
        if (text != null)
        {
            text.text = message;
        }

        FadeSequenceAsync().Forget();
    }

    private async UniTaskVoid FadeSequenceAsync()
    {
        canvasGroup.alpha = 1f;
        SetTextAlpha(0f);

        await FadeTextInAsync();

        await UniTask.Delay(TimeSpan.FromSeconds(waitBeforeFadeOut));

        await FadeOutPanelAsync();

        UIManager.Instance.ClosePanel(PanelName);

        //UIManager.Instance.OpenPanel("SettingsPanel");

        Debug.Log("开始面板加载完成，继续流程");
    }

    private async UniTask FadeTextInAsync()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            SetTextAlpha(alpha);
            await UniTask.Yield();
        }
        SetTextAlpha(1f);
    }

    private async UniTask FadeOutPanelAsync()
    {
        float timer = 0f;
        while (timer < panelFadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(timer / panelFadeDuration);
            canvasGroup.alpha = alpha;
            await UniTask.Yield();
        }
        canvasGroup.alpha = 0f;
    }

    private void SetTextAlpha(float alpha)
    {
        if (text != null)
        {
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }
    }
}
