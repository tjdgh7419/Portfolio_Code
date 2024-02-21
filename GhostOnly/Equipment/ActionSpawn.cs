using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpawn : EquipAction
{
    [field: Header("Extra Info")]
    [field: SerializeField] public PoolType SpawnObject { get; private set; }

    private void Reset()
    {
        Range = 0;
        Cooltime = 3;
    }

    public override bool Action(Vector2 dir, Vector2 spawnPoint, float rotZ)
    {
        if (!Able)
            return false;

        GameObject obj = ObjectPoolManager.Instance.GetGo(SpawnObject);
        obj.transform.position = spawnPoint;

        if (obj.TryGetComponent(out IActionSpawnable actionSpawn))
        {
            actionSpawn.Initialize(Target);
        }

        Managers.Sound.PlaySound(SoundType);

        RecentTime = GetCooltime();
        Able = false;

        return true;
    }
}