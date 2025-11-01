using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class ArrowManager : MonoSingleton<ArrowManager>
{

    [Header("箭头配置")]
    public GameObject ArrowPrefab;
    public Transform arrowsHolder;

    [Header("音效")]
    //[SerializeField] private AudioSource victorySound;
    //[SerializeField] private AudioSource missSound;

    private Queue<Arrow> arrows = new Queue<Arrow>();
    private Arrow currentArrow;
    private MetroDoor currentDoor;

    private float waveTime = 9f;
    private bool isFinish;
    private int _currentCorrectInputs;
    private int _totalArrowCount;
    private CancellationTokenSource waveCts;
    private void Start()
    {
        ClearWave();
    }

    public void CreateWave(int length, MetroDoor door)
    {
        arrowsHolder.gameObject.SetActive(true);
        //Debug.Log($"正在生成箭头，数量: {length}");
        arrows.Clear();
        isFinish = false;
        currentDoor = door;

        _currentCorrectInputs = 0;
        _totalArrowCount = length;

        waveCts = new CancellationTokenSource();
        StartTimerAsync(waveCts.Token).Forget();

        for (int i = 0; i < length; i++)
        {
            GameObject arrowObj = ObjectPoolManager.Instance.GetObject(ArrowPrefab);
            arrowObj.transform.SetParent(arrowsHolder);
            
            Arrow arrow = arrowObj.GetComponent<Arrow>();
            int randomDir = UnityEngine.Random.Range(0, 4);
            arrow.Setup(randomDir);
            arrowObj.transform.localPosition += new Vector3(i * 100, 0, 0);
            arrows.Enqueue(arrow);
        }

        if (arrows.Count > 0)
            currentArrow = arrows.Dequeue();
        else
            currentArrow = null;
    }

    private async UniTask StartTimerAsync(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(waveTime), ignoreTimeScale: false, cancellationToken: token);
            if (!token.IsCancellationRequested)
            {
                await TriggerPuzzleCompleteEvent(false);
            }                
        }
        catch (OperationCanceledException)
        {
            // 波被提前结束，取消计时
        }
    }

    public void TypeArrow(KeyCode inputKey)
    {
        if (isFinish || currentArrow == null)
            return;

        bool correct = ConvertKeyCodeToInt(inputKey) == currentArrow.ArrowDir;

        if (correct)
        {
            currentArrow.SetToFinishState();
            _currentCorrectInputs++;
        }
        else
        {
            currentArrow.SetToErrorState();
        }

        //currentDoor?.RecordInput(correct);
        if (arrows.Count > 0)
            currentArrow = arrows.Dequeue();
        else
        {
            bool puzzleSuccess = (_currentCorrectInputs == _totalArrowCount);
            TriggerPuzzleCompleteEvent(puzzleSuccess).Forget();
            //Debug.Log($"箭头波完成，总正确数: {currentDoor?.correctInputs}");
        }
    }

    private async UniTask TriggerPuzzleCompleteEvent(bool success)
    {
        if (currentDoor == null) return;
        waveCts?.Cancel();
        isFinish = true;
        EventManager.Instance.TriggerEvent(this, OnPuzzleCompleteEventArgs.Create(success, currentDoor));
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        ClearWave();
    }

    public void ClearWave()
    {
        waveCts?.Cancel();
        waveCts?.Dispose();
        waveCts = null;
        for (int i = arrowsHolder.childCount - 1; i >= 0; i--)
        {
            Transform arrow = arrowsHolder.GetChild(i);
            ObjectPoolManager.Instance.ReturnObject(ArrowPrefab, arrow.gameObject);
        }

        arrows.Clear();
        currentArrow = null;
        currentDoor = null;
        isFinish = true;

        arrowsHolder.gameObject.SetActive(false);
    }

    public bool IsInWave()
    {
        return !isFinish;
    }

    private int ConvertKeyCodeToInt(KeyCode key)
    {
        return key switch
        {
            KeyCode.UpArrow => 0,
            KeyCode.DownArrow => 1,
            KeyCode.LeftArrow => 2,
            KeyCode.RightArrow => 3,
            _ => -1
        };
    }
}