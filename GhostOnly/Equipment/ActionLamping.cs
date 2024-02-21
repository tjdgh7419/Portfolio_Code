using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionLamping : EquipAction
{
    [field: Header("Extra Info")]
    [field: SerializeField] public bool IsSkill { get; private set; }

    private void Reset()
    {
        Range = 1.2f;
        Cooltime = 2;

        IsSkill = false;
    }

    public override bool Action(Vector2 dir, Vector2 spawnPoint, float rotZ)
    {
        if (!Able)
            return false;

        GameObject lamping = ObjectPoolManager.Instance.GetGo(PoolType.Lamping);      
        lamping.transform.position = spawnPoint;

        Managers.Sound.PlaySound(SoundType, spawnPoint, true);

        lamping.GetComponent<Lamping>().Initialize(rotZ, GetComponentInParent<NewEquip>().Owner.IsMastered, IsSkill);

        RecentTime = GetCooltime();
        Able = false;

        return true;
    }
}
