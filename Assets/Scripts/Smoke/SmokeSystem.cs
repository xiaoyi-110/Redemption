using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SmokeSystem : MonoSingleton<SmokeSystem>
{
    public enum SmokeLevel { Sober, Level1, Level2, Level3 }
    public static SmokeSystem S { get; private set; }

    [Header("烟雾预制体")]
    public GameObject soberParticlePrefab;
    public GameObject level1ParticlePrefab;
    public GameObject level2ParticlePrefab;
    public GameObject level3ParticlePrefab;

    [Header("烟雾监测设置")]
    public float detectionRadius = 20f;
    public float spreadInterval = 1.0f;
    public int initialSmokeAmount = 100;
    public CameraEffectsController cameraEffects;

    [System.Serializable]
    public class SmokeArea
    {
        public GameObject smokeObject;
        public SmokeLevel level;
        public int amount = 100;
    }

    [System.Serializable]
    public class TagLevelPair
    {
        public string tag;
        public SmokeLevel level;
    }

    [Header("标签设置")]
    public List<TagLevelPair> tagLevelMap = new List<TagLevelPair>
    {
        new TagLevelPair { tag = "SoberSmoke", level = SmokeLevel.Sober },
        new TagLevelPair { tag = "Level1Smoke", level = SmokeLevel.Level1 },
        new TagLevelPair { tag = "Level2Smoke", level = SmokeLevel.Level2 },
        new TagLevelPair { tag = "Level3Smoke", level = SmokeLevel.Level3 }
    };

    //private Dictionary<Vector2, GameObject> smokeParticles = new Dictionary<Vector2, GameObject>();
    public List<SmokeArea> activeSmoke = new List<SmokeArea>();
    private Collider2D[] smokeCache = new Collider2D[20];
    private PlayerController player;


    void Start()
    {
        player=FindObjectOfType<PlayerController>();
        InitializeSceneSmoke();
        StartCoroutine(DetectSmokeRoutine());
    }

    private IEnumerator DetectSmokeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            HandleCharacterEnterSmoke(player, player.transform.position);
        }
    }

    private void InitializeSceneSmoke()
    {
        foreach (var pair in tagLevelMap)
        {
            GameObject[] sceneSmokes = GameObject.FindGameObjectsWithTag(pair.tag);
            foreach (GameObject smokeObj in sceneSmokes)
            {
                activeSmoke.Add(new SmokeArea
                {
                    smokeObject = smokeObj, 
                    level = pair.level,
                    amount = initialSmokeAmount
                });
            }
        }
    }

    #region 烟雾管理
    public void AddSmoke(Vector3 position, SmokeLevel level, int amount, Quaternion rotation = default)
    {
        SmokeArea existingSmoke = activeSmoke.Find(s => Vector3.Distance(s.smokeObject.transform.position, position) < 1.0f);
        if (existingSmoke != null)
        {
            //如果存在，更新其数据，而不是销毁和重建
            existingSmoke.amount += amount;
            if (level > existingSmoke.level)
            {
                existingSmoke.level = level;
                existingSmoke.smokeObject.tag = GetTagFromLevel(level);
            }
        }
        else
        {
            // 如果不存在，则创建新的
            GameObject prefab = GetParticlePrefab(level);
            GameObject particle = Instantiate(prefab, position, rotation);
            particle.transform.position = position;
            particle.tag = GetTagFromLevel(level);

            var ps = particle.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            activeSmoke.Add(new SmokeArea
            {
                smokeObject = particle,
                level = level,
                amount = amount
            });
        }


    }

  
    #endregion

    #region 监测系统
    public SmokeLevel DetectHighestSmokeLevel(Vector2 position)
    {
        SmokeLevel highestLevel = SmokeLevel.Level1;
        int count = Physics2D.OverlapCircleNonAlloc(position, detectionRadius, smokeCache, LayerMask.GetMask("Smoke"));
        //Collider2D[] smokeColliders = Physics2D.OverlapCircleAll(position, detectionRadius, LayerMask.GetMask("Smoke"));
        if (count== 0)
        {
            return SmokeLevel.Sober;
        }

        for (int i = 0; i < count; i++)
        {
            // 找到烟雾对应的等级
            var currentLevel = GetLevelFromTag(smokeCache[i].tag);
            if (currentLevel > highestLevel)
            {
                highestLevel = currentLevel;
                // 如果已经找到最高等级，可以提前退出循环
                if (highestLevel == SmokeLevel.Level3)
                {
                    break;
                }
            }
        }
        return highestLevel;
    }

    #endregion

    #region 工具方法
    private string GetTagFromLevel(SmokeLevel level)
    {
        var pair = tagLevelMap.FirstOrDefault(p => p.level == level);
        return pair != null ? pair.tag : "Level1Smoke";
    }

    private SmokeLevel GetLevelFromTag(string tag)
    {
        var pair = tagLevelMap.FirstOrDefault(p => p.tag == tag);
        return pair != null ? pair.level : SmokeLevel.Level1;
    }

    private GameObject GetParticlePrefab(SmokeLevel level)
    {
        return level switch
        {
            SmokeLevel.Sober => soberParticlePrefab,
            SmokeLevel.Level1 => level1ParticlePrefab,
            SmokeLevel.Level2 => level2ParticlePrefab,
            SmokeLevel.Level3 => level3ParticlePrefab,
            _ => level1ParticlePrefab
        };
    }
    #endregion

    #region 调试工具


    #endregion

    #region 交互设置
    private void ClearNearbyEffects(Vector2 position)
    {
        Collider2D[] npcs = Physics2D.OverlapCircleAll(position, 2f, LayerMask.GetMask("NPC"));
        foreach (var npc in npcs)
        {
            NPC npcController = npc.GetComponent<NPC>();
            if (npcController != null)
            {
                npcController.RecoverFromEffects();
            }
        }
    }
    public void HandleCharacterEnterSmoke(PlayerController player, Vector2 position)
    {
        if (player == null)
        {
            Debug.LogError("Can't find the player");
        }
        SmokeLevel level = DetectHighestSmokeLevel(position);

        switch (level)
        {
            case SmokeLevel.Level2:
                if (!PlayerEquipmentManager.Instance.EquippedMask)
                {
                    cameraEffects.EnterIllusionWorld();
                }
                break;
            case SmokeLevel.Level1:
                if (cameraEffects.isIllusion)
                {
                    cameraEffects.ReturnFromIllusionWorld();
                }
                break;
            case SmokeLevel.Sober:
                if (cameraEffects.isIllusion)
                {
                    cameraEffects.ReturnFromIllusionWorld();
                }
                break;
            default:
                break;
        }
    }
    #endregion
}