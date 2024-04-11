using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class QuickSlot
{
	public ItemSlotInfo inventorySlot;
	public ItemPanel quickSlotPanel;
}

[System.Serializable]
public class ItemSlotInfo
{
	public Item item;
	public string name;
	public int stacks;

	public ItemSlotInfo(Item newItem, int newStacks)
	{
		item = newItem;
		stacks = newStacks;
	}
}

public class InventoryModel : StorageModel
{
	private List<QuickSlot> _quickSlots;

	public InventoryModel(int inventorySize, List<ItemSlotInfo> itemsList,
		List<QuickSlot> quickSlots) : base(inventorySize, itemsList)
	{
		_quickSlots = quickSlots;
	}

	public void UseConsumableItem(ItemPanel selectedPanel)
	{
		// 아이템을 사용해도 액션이 계속 진행되고 다른 아이템의 슬롯을 갔다가 다시 오면 Null오류가 뜸
		if(selectedPanel != null && selectedPanel.itemSlot.item.ItemType == ItemType.Consumable)
		{
			if(!selectedPanel.UseItem()) selectedPanel = null;
			UpdateAction();
		}
	}
}
