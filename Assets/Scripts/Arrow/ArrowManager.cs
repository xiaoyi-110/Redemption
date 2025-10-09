using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoSingleton<ArrowManager>
{

    [Header("箭头配置")]
    public GameObject arrowPrefab;
    public Transform arrowsHolder;

    [Header("音效")]
    //[SerializeField] private AudioSource victorySound;
    //[SerializeField] private AudioSource missSound;

    private Queue<Arrow> arrows = new Queue<Arrow>();
    private Arrow currentArrow;
    private MetroDoor currentDoor;

    public float waveTime = 9f;
    private bool isFinish;

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

        for (int i = 0; i < length; i++)
        {
            GameObject arrowObj = ObjectPoolManager.Instance.GetObject(arrowPrefab);
            arrowObj.transform.SetParent(arrowsHolder);
            

            Arrow arrow = arrowObj.GetComponent<Arrow>();
            int randomDir = Random.Range(0, 4);
            arrow.Setup(randomDir);
            arrowObj.transform.localPosition += new Vector3(i * 100, 0, 0);
            arrows.Enqueue(arrow);
        }

        if (arrows.Count > 0)
            currentArrow = arrows.Dequeue();
        else
            currentArrow = null;
    }

    public void TypeArrow(KeyCode inputKey)
    {
        if (isFinish || currentArrow == null)
            return;

        bool correct = ConvertKeyCodeToInt(inputKey) == currentArrow.arrowDir;

        if (correct)
        {
            currentArrow.SetToFinishState();
        }
        else
        {
            currentArrow.SetToErrorState();
        }

        currentDoor?.RecordInput(correct);

        if (arrows.Count > 0)
            currentArrow = arrows.Dequeue();
        else
        {
            isFinish = true;
            //Debug.Log($"箭头波完成，总正确数: {currentDoor?.correctInputs}");
        }
    }

    public void ForceFinish()
    {
        if (!isFinish)
        {
            currentDoor?.FinishWave();
            ClearWave();
        }
    }

    public void ClearWave()
    {
        for (int i = arrowsHolder.childCount - 1; i >= 0; i--)
        {
            Transform arrow = arrowsHolder.GetChild(i);
            ObjectPoolManager.Instance.ReturnObject(arrowPrefab, arrow.gameObject);
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