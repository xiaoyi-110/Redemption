using System.Collections;
using UnityEngine;
public class CombustibleItem : InteractableObject,IExtinguishable
{
    public enum CombustibleLevel { L1, L2, L3, Sober }

    [Header("燃烧物设置")]
    public CombustibleLevel level;
    public bool isBurning = false;
    public float burnInterval = 1f;

    [Header("烟雾设置")]
    public SmokeSystem.SmokeLevel smokeLevel;
    private Coroutine burnCoroutine;

    [Header("烟雾生成控制")]
    public int maxSmokeCount = 10;     //最大烟雾数量


    [Header("视觉表现")]
    public SpriteRenderer targetRenderer;
    public Sprite visualSprite;

    [Header("火焰特效")]
    public GameObject Flame;
    public Vector3 flameOffset = new Vector3(0.15f, 1f, -0.1f);
    public Vector3 smokeOffest = new Vector3(0f, 0.9f, -0.9f);

    private Rigidbody2D rb;
    public BoxCollider2D itemCollider;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        itemCollider = rb.GetComponent<BoxCollider2D>();
        rb.isKinematic = true;

        // 应用贴图
        if (targetRenderer != null && visualSprite != null)
        {
            targetRenderer.sprite = visualSprite;
        }
        smokeLevel = ConvertToSmokeLevel(level);
    }

    public override void OnInteract(PlayerController player)
    {
        if (isBurning)
        {
            Debug.Log("燃烧物正在燃烧，无法拾取！");
            return;
        }
        base.OnInteract(player);
    }
    protected override void HandleUse(PlayerController player)
    {
        ThrowAndIgnite(player);
    }
    private void ThrowAndIgnite(PlayerController player)
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        transform.position = player.transform.position;

        rb.AddForce(player.transform.right * 10f, ForceMode2D.Impulse);
        StartCoroutine(DelayIgnite(0.2f));
    }

    private IEnumerator DelayIgnite(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        Ignite();
    }

    public void Ignite()
    {
        if (!isBurning)
        {
            isBurning = true;
            if (itemCollider != null)
            {
                itemCollider.isTrigger = false;
            }
            if (Flame != null)
            {
                Flame.transform.localPosition = flameOffset;
                Flame.SetActive(true);
            }
            burnCoroutine = StartCoroutine(GenerateSmoke());
            if (AudioManager.Instance != null)
            {
                //AudioManager.Instance.PlayLoopSFX(AudioManager.Instance.fireClip);
            }
            //Debug.Log($"{level} 级燃烧物开始燃烧！");
        }
    }


    public void Extinguish()
    {
        if (isBurning)
        {
            isBurning = false;
            if (itemCollider != null)
            {
                itemCollider.isTrigger = true;
            }
            if (Flame != null)
                Flame.SetActive(false);

            if (burnCoroutine != null)
                StopCoroutine(burnCoroutine);
            if (AudioManager.Instance != null)
            {
                //AudioManager.Instance.StopLoopSFX();
            }
            Debug.Log("燃烧物已扑灭！");
        }
    }

    private IEnumerator GenerateSmoke()
    {
        for (int i = 0; i < maxSmokeCount; i++)
        {
            if (!isBurning)
                break;

            SmokeSystem.Instance?.AddSmoke(transform.position + smokeOffest, smokeLevel, 1, Quaternion.Euler(-135f, 0f, 0f));

            yield return new WaitForSeconds(burnInterval);
        }

        Debug.Log("燃烧结束，物体销毁。");
        if (AudioManager.Instance != null)
        {
            //AudioManager.Instance.StopLoopSFX();
        }
        Destroy(gameObject);
    }

    private SmokeSystem.SmokeLevel ConvertToSmokeLevel(CombustibleLevel level)
    {
        switch (level)
        {
            case CombustibleLevel.L1: return SmokeSystem.SmokeLevel.Level1;
            case CombustibleLevel.L2: return SmokeSystem.SmokeLevel.Level2;
            case CombustibleLevel.L3: return SmokeSystem.SmokeLevel.Level3;
            case CombustibleLevel.Sober: return SmokeSystem.SmokeLevel.Sober;
            default: return SmokeSystem.SmokeLevel.Level1;
        }
    }

    public override void OnEquip(Transform parent)
    {
        if (isBurning)
        {
            //Debug.Log("燃烧物正在燃烧，无法装备！");
            return;
        }
        base.OnEquip(parent);
    }
}