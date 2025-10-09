using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


public class ResultPanelUI : UIBase
{
    public override UIType UIType => UIType.System;
    public override string PanelName => "ResultPanelUI";

    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text messageText;
    [SerializeField] private RectTransform creditsContentTransform; // 制作人名单内容的 RectTransform

    [Header("Timings")]
    [SerializeField] private float fadeDuration = 2f; // 面板和文本淡入时间
    [SerializeField] private float waitBeforeScroll = 2f; // 显示结果文本后等待的时间
    [SerializeField] private float scrollSpeed = 30f; // 制作人名单滚动速度
    [SerializeField] private float scrollTime = 10f; // 制作人名单总滚动时间

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        gameObject.SetActive(true);

        string message = args.Length > 0 && args[0] is string ? (string)args[0] : "";
        if (messageText != null)
        {
            messageText.text = message;
        }

        PlaySequenceAsync().Forget();
    }


    private async UniTaskVoid PlaySequenceAsync()
    {

        canvasGroup.alpha = 0f;
        messageText.gameObject.SetActive(true);
        creditsContentTransform.gameObject.SetActive(false);

        await FadeInAsync();

        await UniTask.Delay(TimeSpan.FromSeconds(waitBeforeScroll));

        messageText.gameObject.SetActive(false);
        creditsContentTransform.gameObject.SetActive(true);

        await ScrollCreditsAsync();

        Debug.Log("制作人名单滚动结束，返回主菜单");
        UIManager.Instance.OnBackToMenu();

        UIManager.Instance.ClosePanel(PanelName);
    }


    private async UniTask FadeInAsync()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            canvasGroup.alpha = alpha;
            Color c = messageText.color;
            c.a = alpha;
            messageText.color = c;
            await UniTask.Yield();
        }
        canvasGroup.alpha = 1f;
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);
    }

    private async UniTask ScrollCreditsAsync()
    {
        float elapsedTime = 0f;
        Vector2 startPos = creditsContentTransform.anchoredPosition;

        while (elapsedTime < scrollTime)
        {
            float deltaY = scrollSpeed * Time.unscaledDeltaTime;
            creditsContentTransform.anchoredPosition += new Vector2(0f, deltaY);
            elapsedTime += Time.unscaledDeltaTime;
            await UniTask.Yield();
        }
    }
}
