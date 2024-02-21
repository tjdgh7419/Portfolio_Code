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
	public Item1 item;
	public string name;
	public int stacks;

	public ItemSlotInfo(Item1 newItem, int newStacks)
	{
		item = newItem;
		stacks = newStacks;
	}
}

public class InventoryModel
{
	private List<ItemSlotInfo> _items;
	private List<QuickSlot> _quickSlots;

	private ItemPanel _curSelectedPanel;
	private int _inventorySize;
	private Dictionary<string, Item1> _allItemsDictionary = new Dictionary<string, Item1>();

	public event Action OnUpdateInventory;


	public InventoryModel(int inventorySize, List<ItemSlotInfo> itemsList, List<QuickSlot> quickSlotsList)
	{
		_items = itemsList;
		_quickSlots = quickSlotsList;
		_inventorySize = inventorySize;	
	}
	
	public void InitializeInventoryModel()
	{
		for (int i = 0; i < _inventorySize; i++)
		{
			_items.Add(new ItemSlotInfo(null, 0));
		}

		List<Item1> allItems = GetAllItems().ToList();
		string itemInDictionary = "Items in Dictionary: ";
		foreach (Item1 i in allItems)
		{
			if (!_allItemsDictionary.ContainsKey(i.GiveName()))
			{
				_allItemsDictionary.Add(i.GiveName(), i);
				itemInDictionary += ", " + i.GiveName();

			}
			else
			{
				Debug.Log("" + i + "already exists in Dictionary " + _allItemsDictionary[i.GiveName()]);
			}
		}

		itemInDictionary += ".";
		Debug.Log(itemInDictionary);
	}

	public int AddItem(string itemName, int amount)
	{
		Item1 item = null;
		_allItemsDictionary.TryGetValue(itemName, out item);

		if (item == null)
		{
			Debug.Log("Could not find Item in Dictionary");
			return amount;
		}
		foreach (ItemSlotInfo i in _items)
		{
			if (i.item != null)
			{
				if (i.item.GiveName() == item.GiveName())
				{
					if (amount > i.item.MaxStacks() - i.stacks)
					{
						amount -= i.item.MaxStacks() - i.stacks;
						i.stacks = i.item.MaxStacks();
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

				if (amount > item.MaxStacks())
				{
					i.item = item;
					i.stacks = item.MaxStacks();
					amount -= item.MaxStacks();
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

	public void EquipItem(ItemPanel seletedPanel)
	{
		if (_curSelectedPanel != null)
		{
			_curSelectedPanel.equipOutline.enabled = false;
		}
		if (_curSelectedPanel == seletedPanel)
		{
			UnEquipItem(seletedPanel);
			return;
		}

		_curSelectedPanel = seletedPanel;
		_curSelectedPanel.equipOutline.enabled = true;

		OnUpdateInventory?.Invoke();
	}

	public void UnEquipItem(ItemPanel equippedPanel)
	{
		_curSelectedPanel = null;
		equippedPanel.equipOutline.enabled = false;
	}

	public IEnumerable<Item1> GetAllItems()
	{
		return System.AppDomain.CurrentDomain.GetAssemblies()
		 .SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Item1)))
		 .Select(type => System.Activator.CreateInstance(type) as Item1);
	}
}
