using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBuff : EquipAction
{
    [field: Header("Extra Info")]
    [field: SerializeField]
    public float BuffValue { get; private set; }

    [field: SerializeField] public float BuffTime { get; private set; }

    private void Reset()
    {
        Range = 0f;
        Cooltime = 7;
    }

    public override bool Action(Vector2 dir, Vector2 spawnPoint, float rotZ)
    {
        if (!Able)
            return false;

        Equip.Owner.BuffSystem.AddBuff(new BuffModel(
            new[] { AppliedStat },
            BuffTime,
            BuffValue,
            BuffModel.StatTypeToBuffType(AppliedStat)));

        Managers.Sound.PlaySound(SoundType);

        RecentTime = GetCooltime();
        Able = false;

        return true;
    }
}