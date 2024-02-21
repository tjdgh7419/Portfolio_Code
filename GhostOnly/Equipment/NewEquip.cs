using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewEquip : MonoBehaviour
{
    public enum EquipType
    {
        Sword,
        Bow,
        Wand,
        Lamp
    }

    [SerializeField] private SpriteRenderer equipRenderer;
    [SerializeField] private Collider2D equipCollider;
    [SerializeField] private GameObject minimapMarker;

    [field: Header("Equip Info")]
    [field: SerializeField] public EquipType Type;
    [field: SerializeField] public Sprite ArmedSprite;

    [field: SerializeField] public EquipAction Action { get; private set; }
    [field: SerializeField] public EquipAction Skill1 { get; private set; }
    [field: SerializeField] public EquipAction Skill2 { get; private set; }

    public SkullController Owner { get; private set; } = null;

    public void Equipped(SkullController newOwner)
    {
        equipRenderer.enabled = false;
        equipCollider.enabled = false;
        minimapMarker.SetActive(false);

        Action.SetStat(newOwner.Stat);
        Skill1.SetStat(newOwner.Stat);
        Skill2.SetStat(newOwner.Stat);

        Owner = newOwner;
    }

    public void UnEquipped(Vector2 dropPosition)
    {
        equipRenderer.enabled = true;
        equipCollider.enabled = true;
        minimapMarker.SetActive(true);

        transform.position = dropPosition;

        Owner = null;
    }
}