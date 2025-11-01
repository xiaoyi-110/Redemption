using Unity.VisualScripting;
using UnityEngine;

public class DoorFrameVisibility : MonoBehaviour
{
    public Camera mainCamera;
    public float minDistance = 15f;
    private float minAlpha = 0.2f;

    private Renderer doorFrameRenderer;
    private SpriteRenderer leftDoorSpriteRenderer;
    private SpriteRenderer rightDoorSpriteRenderer;
    private MetroDoor metroDoor;
    private Color originalColor;
    void Start()
    {
        doorFrameRenderer = GetComponent<Renderer>();
        metroDoor= GetComponentInChildren<MetroDoor>();
        if (doorFrameRenderer == null)
        {
            Debug.LogError("Renderer not found on Door Frame.");
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        GameObject leftDoor = transform.Find("MetroDoor/LeftDoor")?.gameObject;
        GameObject rightDoor = transform.Find("MetroDoor/RightDoor")?.gameObject;
        if (leftDoor != null)
        {
            leftDoorSpriteRenderer = leftDoor.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("LeftDoor object not found.");
        }

        if (rightDoor != null)
        {
            rightDoorSpriteRenderer = rightDoor.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("RightDoor object not found.");
        }
        if (doorFrameRenderer != null)
        {
            originalColor = doorFrameRenderer.material.color;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        if (distance<minDistance)
        {
           
                SetTransparency(minAlpha);
            }
            else
            {
                SetTransparency(1f);
            }

        if (metroDoor!=null && leftDoorSpriteRenderer != null && rightDoorSpriteRenderer != null)
        {
            bool isOpen = metroDoor.currentState==MetroDoor.DoorState.Open;
            if (isOpen)
            {
                doorFrameRenderer.sortingOrder = Mathf.Min(leftDoorSpriteRenderer.sortingOrder, rightDoorSpriteRenderer.sortingOrder) - 1;
            }
            else
            {
                doorFrameRenderer.sortingOrder =leftDoorSpriteRenderer.sortingOrder;
            }
        }
    }

    private void SetTransparency(float alpha)
    {
        Color newColor = originalColor;
        newColor.a = alpha;
        doorFrameRenderer.material.color = newColor;

        if (leftDoorSpriteRenderer != null)
        {
            Color leftDoorColor = leftDoorSpriteRenderer.color;
            leftDoorColor.a = alpha;
            leftDoorSpriteRenderer.material.color = leftDoorColor;
        }

        if (rightDoorSpriteRenderer != null)
        {
            Color rightDoorColor = rightDoorSpriteRenderer.color;
            rightDoorColor.a = alpha;
            rightDoorSpriteRenderer.material.color = rightDoorColor;
        }
    }
}
