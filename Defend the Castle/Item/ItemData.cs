using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	Resource,
	Equipable,
	Consumable
}

public enum ConsumableType
{
	HP,
	MP,
	Speed
}

[Serializable]
public class ConsumableData
{
	public ConsumableType type;
	public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
	[Header("ItemInformation")]
	public string ItemName;
	public string ItemDescription;
	public ItemType type;
	public ConsumableType Ctype;
	public Sprite icon;

	[Header("Stat")]
	public float AttackPower;

	[Header("Stacking")]
	public bool canStack;
	public int maxStackAmount;

	[Header("Consumable")]
	public ConsumableData[] consumableDatas;

	[Header("Equip")]
	public GameObject equipPrefab;

	[Header("Audio")]
	public AudioClip[] audio;
}
