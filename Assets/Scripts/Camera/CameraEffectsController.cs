using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectsController : MonoBehaviour
{
    public Volume illusionVolume;
    public bool isIllusion;
    private PlayerController player;

    private void Start()
    {
        player= FindObjectOfType<PlayerController>();
        if (illusionVolume != null)
        {
            illusionVolume.gameObject.SetActive(false);
        }
    }

    public void EnterIllusionWorld()
    {
        if (illusionVolume != null)
        {
            illusionVolume.gameObject.SetActive(true);
        }
        isIllusion = true;
        if (!(PlayerEquipmentManager.Instance.EquippedItem is SmokeDetector smokeDetector)) { 
            UIManager.Instance.OpenPanel("MessagePanel","你进入了幻觉世界...").Forget();
        }
        else
        {
            UIManager.Instance.OpenPanel("MessagePanel", "危险烟雾，请佩戴防毒面具").Forget();
        }
    }

    public void ReturnFromIllusionWorld()
    {
        if (illusionVolume != null)
        {
            illusionVolume.gameObject.SetActive(false);
        }
        isIllusion = false;
        UIManager.Instance.OpenPanel("MessagePanel", "你恢复了意识。").Forget();
    }
}

