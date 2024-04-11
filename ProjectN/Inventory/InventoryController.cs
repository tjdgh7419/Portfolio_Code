using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Assertions;

public class InventoryController : StorageController
{
	[Header("Quick Slot")]
	[SerializeField] private List<QuickSlot> quickSlots = new List<QuickSlot>();

	private InventoryModel _inventoryModel;
	private InventoryView _inventoryView;
	private ItemPanel _curSelectedPanel;
	private Equipment _equipment;

	private int _testWheel = 0;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			_inventoryView.UpdateList();
		}
	}

	public void Init(Equipment equipment)
	{
		_equipment = equipment;
		SwitchSlot(0);
	}

	private void OnDestroy()
	{
		_inventoryModel.OnUpdateInventory -= _inventoryView.Refresh;
	}

	public override void AddItem(string itemName, int amount)
	{
		_inventoryModel.AddItem(itemName, amount);
	}

	public override void InitializeStorageData()
	{
		_inventoryModel = new InventoryModel(_storageSize, items, quickSlots);
		_inventoryView = new InventoryView(items, quickSlots, mouse, storageMenu, itemPanel, itemPanelGrid);

		_inventoryModel.OnUpdateInventory += _inventoryView.Refresh;

#if UNITY_EDITOR
		foreach (string itemName in AddItems)
		{
			AddItem(itemName, 1);
		}
#endif
		_inventoryView.RefreshStorage();
	}

	private async void SwitchSlot(int num)
	{
		EquipItem(quickSlots[num].quickSlotPanel);
		GameObject ItemObj = null;
		if (quickSlots[num].inventorySlot.item != null)
		{
			Item iteminfo = quickSlots[num].inventorySlot.item;
			var ItemObjTask = AddressableManager.Instance.LoadObjectAsync(iteminfo.PrefabPath);

			await ItemObjTask;

			ItemObj = ItemObjTask.Result;
			ItemObj.TryGetComponent(out ItemObject itemObject);
			Assert.IsNotNull(itemObject, $"{iteminfo.Name}의 prefab에 ItemObject 설정을 해주세요");
			itemObject.ItemData = iteminfo;
		}
		_equipment.Equip(ItemObj);
		_testWheel = num;
	}

	public void ScrollWhell(float input)
	{
		if (input > 0)
		{
			if (_testWheel < quickSlots.Count - 1)
			{
				_testWheel++;
			}
		}
		else
		{
			if (_testWheel > 0)
			{
				_testWheel--;
			}
		}
		SwitchSlot(_testWheel);
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

		_inventoryView.Refresh();
	}

	public void UnEquipItem(ItemPanel equippedPanel)
	{
		_curSelectedPanel = null;
		equippedPanel.equipOutline.enabled = false;
	}


	public Action GetInventoryShowFunc()
	{
		if (_inventoryView == null)
			InitializeStorageData();
		return _inventoryView.ShowStorage;
	}

	public Action<float> GetScrollFunc()
	{
		return ScrollWhell;
	}


	public Action<int> GetShortcutFunc()
	{
		return SwitchSlot;
	}

	//public Action<ItemPanel> GetItemUseFunc()
	//{
	//	return _inventoryModel.UseConsumableItem(_curSelectedPanel);
	//}

	public List<ItemSlotInfo> GetItemList() => items;
}
