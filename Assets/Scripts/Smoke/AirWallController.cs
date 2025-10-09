using Cysharp.Threading.Tasks;
using UnityEngine;

public class AirWallController : MonoBehaviour
{
    private Collider2D col2D;
    private PlayerController player;
    private ParticleSystem ps;

    public bool destroyAfterParticleGone = true;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        col2D = GetComponent<Collider2D>();
        ps = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!GameManager.Instance.isGameStarted) return;

        // 动态检测玩家是否佩戴防毒面具
        bool hasGasMask = PlayerEquipmentManager.Instance.EquippedItem is GasMask;
        col2D.enabled = !hasGasMask;

        // 粒子系统检测
        if (!ps.IsAlive(true))
        {
            col2D.enabled = false;

            if (destroyAfterParticleGone)
                Destroy(gameObject);

            enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && col2D.enabled)
        {
            // 提示信息根据是否有探测器进行区分
            if (PlayerEquipmentManager.Instance.EquippedItem is SmokeDetector)
            {
                UIManager.Instance.OpenPanel("MessagePanel", "危险烟雾，请佩戴防毒面具").Forget();
            }
            else
            {
                UIManager.Instance.OpenPanel("MessagePanel", "烟雾太浓，你无法前进").Forget();
            }
        }
    }
}
