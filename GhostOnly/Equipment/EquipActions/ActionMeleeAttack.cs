using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMeleeAttack : EquipAction
{
    [field: Header("Extra Info")]
    [field: SerializeField] public PoolType MeleeObject { get; private set; }

    private void Reset()
    {
        Range = 1.5f;
        Cooltime = 2;
        AppliedStat = StatType.Strength;
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

        GameObject attack = ObjectPoolManager.Instance.GetGo(MeleeObject);     
        attack.transform.position = spawnPoint;
   
        if (attack.TryGetComponent(out MeleeAttack data))
        {
            data.Initialize(rotZ - 90, CalcDamage(Stat.Stats[AppliedStat].Value), Target, HitCallback);
        }

        Managers.Sound.PlaySound(SoundType, spawnPoint, true);

        RecentTime = GetCooltime();
        Able = false;

        return true;
    }
}
