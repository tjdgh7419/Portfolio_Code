using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMeteor : EquipAction
{
    [field: Header("Extra Info")]
    [field: SerializeField] public float Speed { get; private set; }

    private void Reset()
    {
        Range = 5;
        Cooltime = 15;
        AppliedStat = StatType.Intelligence;
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

        Vector2 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));

        GameObject meteor = ObjectPoolManager.Instance.GetGo(PoolType.Meteor);
        meteor.transform.position = point;
        meteor.GetComponent<Meteor>().Initialize(dir, rotZ, Range, CalcDamage(Stat.Stats[AppliedStat].Value), Speed, Target);

        Managers.Sound.PlaySound(SoundType);

        RecentTime = GetCooltime();
        Able = false;

        return true;
    }
}
