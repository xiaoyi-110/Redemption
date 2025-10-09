using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAirWall : MonoBehaviour
{
    private Collider2D col2D;
    private PlayerController player;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        col2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        bool shouldBlock = !(PlayerEquipmentManager.Instance.EquippedItem is Flashlight);
        col2D.enabled = shouldBlock; // 无手电筒时激活碰撞体
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && col2D.enabled)
        {
            UIManager.Instance.OpenPanel("MessagePanel", "该区域必须携带手电筒才能进入").Forget();
        }
    }
}