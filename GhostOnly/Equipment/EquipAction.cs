using Data;
using System;
using UnityEngine;
using Util;

public enum ActionType
{
    SwordAction,
    SwordSkill1,
    SwordSkill2,
    BowAction,
    BowSkill1,
    BowSkill2,
    WandAction,
    WandSkill1,
    WandSkill2,
    LampAction,
    LampSkill1,
    LampSkill2
}

public abstract class EquipAction : MonoBehaviour
{
    public StatController Stat { get; private set; }

    protected NewEquip Equip;
    [field: SerializeField] public StatType AppliedStat { get; protected set; }

    [field: Header("Action Info")]
    [field: SerializeField]
    public LayerMask Target { get; protected set; }

    [field: SerializeField] public float Range { get; protected set; }
    [field: SerializeField] public float Cooltime { get; protected set; }
    [field: SerializeField] public Data.SoundType SoundType { get; protected set; }

    [field:Header("UI")]

    [field: SerializeField] public ActionType UIInfo { get; private set; }

    public bool Able { get; protected set; }
    public float RecentTime { get; protected set; }

    public Action<Collider2D> HitCallback;

    private void Awake()
    {
        Equip = GetComponentInParent<NewEquip>();
    }

    private void Update()
    {
        if (Able)
            return;

        if (RecentTime > 0)
        {
            RecentTime -= Time.deltaTime;
        }
        else
        {
            Able = true;
        }
    }

    public void SetStat(StatController _stat)
    {
        Stat = _stat;
    }

    public float GetCooltime() => Mathf.Max(MinStat.ACTION_DELAY, Cooltime - Stat.Stats[StatType.ActionDelay].Value);

    public float GetCooltimePercentage() => (GetCooltime() - RecentTime) / GetCooltime();

    public abstract bool Action(Vector2 dir, Vector2 spawnPoint, float rotZ);

    public Sprite GetIcon()
    {
        return UIInfo switch
        {
            ActionType.SwordAction => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.SwordActionIconIndex],
            ActionType.SwordSkill1 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.SwordSkill1IconIndex],
            ActionType.SwordSkill2 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.SwordSkill2IconIndex],
            ActionType.BowAction => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.BowActionIconIndex],
            ActionType.BowSkill1 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.BowSkill1IconIndex],
            ActionType.BowSkill2 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.BowSkill2IconIndex],
            ActionType.WandAction => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.WandActionIconIndex],
            ActionType.WandSkill1 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.WandSkill1IconIndex],
            ActionType.WandSkill2 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.WandSkill2IconIndex],
            ActionType.LampAction => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.LampActionIconIndex],
            ActionType.LampSkill1 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.LampSkill1IconIndex],
            ActionType.LampSkill2 => Resources.LoadAll<Sprite>(Constants.Sprites.Icon)[Constants.EquipAction.LampSkill2IconIndex],
            _ => null,
        };
    }
}