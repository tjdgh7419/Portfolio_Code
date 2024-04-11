using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHeadShot : EquipAction
{
    [field: Header("Extra Info")]
    [field: SerializeField] public float Speed { get; private set; }

    private void Reset()
    {
        Range = 5;
        Cooltime = 8;
        AppliedStat = StatType.Dexterity;
        Speed = 20;
    }

    private float CalcDamage(float attackPower)
    {
        if (Random.Range(0, 100) < Stat.Stats[StatType.CriticalRate].Value)
        {
            attackPower *= 2f;
        }

        return attackPower;
    }

    public override bool Action(Vector2 dir, Vector2 spawnPoint, float rotZ)
    {
        if (!Able)
            return false;

        GameObject attack = ObjectPoolManager.Instance.GetGo(PoolType.Arrow);
        attack.transform.position = spawnPoint;

        if (attack.TryGetComponent(out RangeAttack data))
        {
            data.Initialize(dir, rotZ, Range, CalcDamage(Stat.Stats[AppliedStat].Value) * 3, Speed, Target);
        }

        Managers.Sound.PlaySound(SoundType);

        RecentTime = GetCooltime();
        Able = false;

        return true;
    }
}
