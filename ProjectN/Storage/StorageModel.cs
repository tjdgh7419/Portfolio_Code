using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class StorageModel
{
	private List<ItemSlotInfo> _items;
	private int _storageSize;

	public event Action OnUpdateInventory;

	public StorageModel(int storageSize, List<ItemSlotInfo> items)
	{
		_storageSize = storageSize;
		_items = items;
		InitializeStorageModel();
	}

	public void InitializeStorageModel()
	{
		if(_items.Count == 0)
		{
			for (int i = 0; i < _storageSize; i++)
			{
				_items.Add(new ItemSlotInfo(null, 0));
			}
		}	
	}

	public int AddItem(string itemName, int amount)
	{
		Item item = null;
		item = DataManager.Instance.GetData<Item>(itemName);

		if (item == null)
		{
			Debug.Log("Could not find Item in Dictionary");
			return amount;
		}
		foreach (ItemSlotInfo i in _items)
		{
			if (i.item != null)
			{
				if (i.item.Name == item.Name)
				{
					if (amount > i.item.GetMaxStack() - i.stacks)
					{
						amount -= i.item.GetMaxStack() - i.stacks;
						i.stacks = i.item.GetMaxStack();
					}
					else
					{
						i.stacks += amount;
						OnUpdateInventory?.Invoke();
						return 0;
					}
				}
			}
		}

		foreach (ItemSlotInfo i in _items)
		{
			if (i.item == null)
			{

				if (amount > item.GetMaxStack())
				{
					i.item = item;
					i.stacks = item.GetMaxStack();
					amount -= item.GetMaxStack();
				}
				else
				{
					i.item = item;
					i.stacks = amount;
					OnUpdateInventory?.Invoke();
					return 0;
				}
			}
		}

		OnUpdateInventory?.Invoke();
		return amount;
	}

	public void UpdateAction()
	{
		OnUpdateInventory?.Invoke();
	}
}