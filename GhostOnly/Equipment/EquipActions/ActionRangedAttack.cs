using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRangedAttack : EquipAction
{
    [field: Header("Extra Info")]
    [field: SerializeField]
    public PoolType RangedObject { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }

    private void Reset()
    {
        Range = 5;
        Cooltime = 3;
        AppliedStat = StatType.Dexterity;
        Speed = 10;
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

        GameObject attack = ObjectPoolManager.Instance.GetGo(RangedObject);
        attack.transform.position = spawnPoint;

        Managers.Sound.PlaySound(SoundType, spawnPoint, true);

        if (attack.TryGetComponent(out RangeAttack data))
        {
            data.Initialize(dir, rotZ, Range, CalcDamage(Stat.Stats[AppliedStat].Value), Speed, Target, HitCallback);
        }

        RecentTime = GetCooltime();
        Able = false;

        return true;
    }
}