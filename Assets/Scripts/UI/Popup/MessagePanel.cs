using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class MessagePanel : UIBase
{
    public override UIType UIType => UIType.Popup; 
    public override string PanelName => "MessagePanel";

    [SerializeField] private Text messageText;
    private CancellationTokenSource cts;

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        if (args.Length > 0 && args[0] is string message)
        {
            messageText.text = message;
            cts?.Cancel();
            cts = new CancellationTokenSource();
            HideMessageAfterDelayAsync(cts.Token).Forget();
        }
    }

    private async UniTaskVoid HideMessageAfterDelayAsync(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(5000, cancellationToken: token);
            UIManager.Instance.ClosePanel(PanelName);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        cts?.Cancel(); 
    }
}