using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class MazePlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D collder;

    public float speed = 5f;
    public Action<bool> OnMazeExit;

    private List<Vector2> mousePositions = new List<Vector2>();
    private bool isTracking = false;
    [SerializeField] private float minDistance;
    [SerializeField] private Camera mazeCamera;
    private List<SpriteRenderer> SRs;

    public bool failTheMaze = false;
    public bool winTheMaze = false;

    private bool isLocked = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collder = GetComponent<BoxCollider2D>();
        SRs = new List<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isLocked)
        {
            MouseInput();
        }
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartTracing();
        }
        if (isTracking && Input.GetMouseButton(0))
        {
            ContinueTracing();
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndTracing();
        }
    }

    private void EndTracing()
    {
        isTracking = false;
        //Debug.Log("Stop tracking mouse positions");
        ResetRunAsync().Forget();
    }

    private void ContinueTracing()
    {
        Vector2 currentPos = Input.mousePosition;
        Vector2 lastPos = mousePositions[mousePositions.Count - 1];
        if (Vector2.Distance(currentPos, lastPos) > minDistance)
        {
            mousePositions.Add(currentPos);
            //Debug.Log("Tracking mouse positions");
            CheckForSpriteInteraction(currentPos);
        }
    }

    private void StartTracing()
    {
        mousePositions.Clear();


        Vector2 currentPos = Input.mousePosition;
        mousePositions.Add(currentPos);

        CheckForSpriteInteraction(currentPos);
        isTracking = true;
        //Debug.Log("Start tracking mouse positions");
    }

    private void CheckForSpriteInteraction(Vector2 screenPosition)
    {
        Vector3 worldPos = mazeCamera.ScreenToWorldPoint(screenPosition);
        Vector2 worldPoint2D = new Vector2(worldPos.x, worldPos.y);
        int mazeLayerMask = 1 << LayerMask.NameToLayer("Maze");
        RaycastHit2D hit = Physics2D.Raycast(worldPoint2D, Vector2.zero,0f,mazeLayerMask);

        if (hit.collider != null && hit.collider.CompareTag("MazeExit"))
        {
            isLocked = true;
            //Debug.Log("到达迷宫出口");
            winTheMaze = true;
            OnMazeExit?.Invoke(true);
        }
        //if (hit.collider == null)
        //{
        //    Debug.Log("Raycast hit nothing!");
        //}
        //else
        //{
        //    Debug.Log($"Hit: {hit.collider.name}, Tag: {hit.collider.tag}");
        //}
        if (hit.collider != null && hit.collider.CompareTag("MazePath"))
        {
            hit.collider.GetComponent<SpriteRenderer>().color = Color.blue;
            if (!SRs.Contains(hit.collider.GetComponent<SpriteRenderer>()))
                SRs.Add(hit.collider.GetComponent<SpriteRenderer>());
        }
        else if (hit.collider != null && hit.collider.CompareTag("MazeWall"))
        {
            //Debug.Log("与墙壁碰撞");
            isLocked = true;
            ResetRunWithHitWallAsync(hit.collider.GetComponent<SpriteRenderer>()).Forget();
        }

    }
    private async UniTaskVoid ResetRunWithHitWallAsync(SpriteRenderer sr)
    {
        sr.color = Color.blue;
        await UniTask.Delay(TimeSpan.FromSeconds(.1f)); // 延迟 0.1 秒
        await ResetRunAsync(); // 等待清理完成
        sr.color = Color.clear;

        failTheMaze = true; // 设置失败标记
        OnMazeExit?.Invoke(false); // 【关键】失败时主动通知 MazeManager
    }

    private async UniTask ResetRunAsync()
    {
        for (int i = 0; i < SRs.Count; i++)
        {
            SRs[i].color = Color.clear;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f)); // 延迟 0.1 秒
        }
        isLocked = false;
        OnMazeExit?.Invoke(false); // 主动通知 MazeManager 失败
    }

}
