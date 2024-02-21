using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;


public enum PoolType
{
    MeleeAttack,
    Slow,
    Arrow,
    HeadShot, 
    Magic,
    Meteor,
    Lamping,
    HealSpell,
    BigMelee,
    ChainLightning,
    HitEffect,
    SwordMan = 1 << 8, 
    BowMan,
    WandMan,
    SwordHero,
    BowHero,
    WandHero,
    Guardian,
    Assassin,
    Ranger,
    Lancer,
    Wizard,
    Mage,
    Jack,
    Queen,
    King,
    GoblinSoul,

    EffectSound = 1 << 9,
    DamageText = 1 << 10,
    RecoveryEffect,
    ElectricExplosionVfx = 1 << 11,
    LifeStealVfx,
    RageVfx,
    FireExplosionVfx,
    FrozenVfx,
    HorrorVfx,
}

public class ObjectPoolManager : MonoBehaviour
{
    private const int HeroStartOffset = (int)PoolType.SwordMan;
    private const int SubItemOffset = (int)PoolType.DamageText;
    private const int VfxOffset = (int)PoolType.ElectricExplosionVfx;

    [System.Serializable]
    private class ObjectInfo
    {
        public PoolType type;

        public GameObject perfab;

        public int count;
    }

    private static ObjectPoolManager _instance;

    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance != null) { return _instance; }

            _instance = FindObjectOfType<ObjectPoolManager>();
            if (_instance != null) { return _instance; }

            GameObject go = new GameObject(nameof(ObjectPoolManager));
            return _instance = go.AddComponent<ObjectPoolManager>();
        }
    }

    public bool IsLoading { get; private set; }
    private bool _isInit = false;

    [SerializeField] private GameObject locate;
    [SerializeField] private ObjectInfo[] objectInfos;

    private PoolType objectType;

    private Dictionary<PoolType, IObjectPool<GameObject>> objectPoolDic =
        new Dictionary<PoolType, IObjectPool<GameObject>>();

    private Dictionary<PoolType, GameObject> goDic = new Dictionary<PoolType, GameObject>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Managers.Data.LoadAllDataSetEvent += Init;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            ClearPool();
            Init();
        };
    }

    private void ClearPool()
    {
        goDic.Clear();
        objectPoolDic.Clear();

        _isInit = false;
    }

    public void Init()
    {
        if (_isInit) { return; }

        if (!Managers.Data.IsLoaded) { return; }

        IsLoading = true;

        for (int idx = 0; objectInfos != null && idx < objectInfos.Length; idx++)
        {
            if (goDic.ContainsKey(objectInfos[idx].type))
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objectInfos[idx].type);
                continue;
            }

            InsertPoolableToPool(objectInfos[idx]);
        }

        IsLoading = false;
        _isInit = true;
    }

    private void InsertPoolableToPool(ObjectInfo info)
    {
        IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: info.count);

        goDic.Add(info.type, info.perfab);
        objectPoolDic.Add(info.type, pool);

        for (int i = 0; i < info.count; i++)
        {
            objectType = info.type;
            PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>();
            poolAbleGo.Pool.Release(poolAbleGo.gameObject);
        }
    }

    private GameObject CreatePooledItem()
    {
        if (locate == null)
        {
            locate = new GameObject("Poolables");
        }

        GameObject poolGo = Instantiate(goDic[objectType], locate.transform);
        poolGo.GetComponent<PoolAble>().Pool = objectPoolDic[objectType];
        return poolGo;
    }

    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    public GameObject GetGo(PoolType goType)
    {
        objectType = goType;

        if (!goDic.ContainsKey(goType))
        {
            GameObject prefab = (GameObject)Managers.Resource.LoadByName(goType.ToString());
            ObjectInfo info = new ObjectInfo() { count = 5, type = goType, perfab = prefab };
            InsertPoolableToPool(info);
        }

        return objectPoolDic[goType].Get();
    }
}