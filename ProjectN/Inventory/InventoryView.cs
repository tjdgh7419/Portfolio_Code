using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class InventoryView : MonoBehaviour
{
	public List<ItemSlotInfo> items;
	public List<QuickSlot> quickSlots;
	public Mouse mouse;
	public bool isPickUp;

	private List<ItemPanel> _existingPanels = new List<ItemPanel>();
	private InventoryModel _inventoryModel;
	private int _updateQuickSlotSize = 0;
	private int _inventorySize = 10;

	[Space]
	[Header("Inventory Menu Components")]
	[SerializeField] public GameObject _inventoryMenu;
	[SerializeField] public GameObject _itemPanel;
	[SerializeField] public GameObject _itemPanelGrid;


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (_inventoryMenu.activeSelf)
			{
				_inventoryMenu.SetActive(false);
				mouse.EmptySlot();
				RefreshInventory();
				//Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				_inventoryMenu.SetActive(true);
				//Cursor.lockState = CursorLockMode.None;
				RefreshInventory();
			}
		}

		if (Input.GetKeyDown(KeyCode.F))
		{
			UpdateList();
		}
	}
	public void RefreshInventory()
	{
		_existingPanels = _itemPanelGrid.GetComponentsInChildren<ItemPanel>().ToList();

		if (_existingPanels.Count < _inventorySize)
		{
			int amountToCreate = _inventorySize - _existingPanels.Count;

			for (int i = 0; i < amountToCreate; i++)
			{
				GameObject newPanel = Instantiate(_itemPanel, _itemPanelGrid.transform);
				_existingPanels.Add(newPanel.GetComponent<ItemPanel>());
			}
		}

		int index = 0;
		foreach (ItemSlotInfo i in items)
		{
			i.name = "" + (index + 1);
			if (i.item != null) i.name += ": " + i.item.GiveName();
			else i.name += ": -";

			ItemPanel panel = _existingPanels[index];
			panel.name = i.name + "Panel";

			if (panel != null)
			{
				panel.inventory = this;
				panel.itemSlot = i;

				if (i.item != null)
				{
					panel.itemImage.gameObject.SetActive(true);
					panel.itemImage.sprite = i.item.GiveItemImage();
					panel.itemImage.CrossFadeAlpha(1f, 0.05f, true);
					panel.stackText.gameObject.SetActive(true);
					panel.stackText.text = "" + i.stacks;
				}
				else
				{
					panel.itemImage.gameObject.SetActive(false);
					panel.stackText.gameObject.SetActive(false);
				}
			}
			index++;
		}
		mouse.EmptySlot();
		RefreshQuickSlots();
	}

	public void RefreshQuickSlots()
	{
		for (int i = 0; i < quickSlots.Count; i++)
		{
			quickSlots[i].inventorySlot = items[i + _updateQuickSlotSize];
			quickSlots[i].quickSlotPanel.inventory = this;
			quickSlots[i].quickSlotPanel.itemSlot = quickSlots[i].inventorySlot;
			quickSlots[i].quickSlotPanel.isQuickSlot = true;

			QuickSlot quickslot = quickSlots[i];
			ItemSlotInfo inventorySlot = quickslot.inventorySlot;
			ItemPanel quickSlotPanel = quickslot.quickSlotPanel;

			if (inventorySlot.item != null)
			{
				quickSlotPanel.itemImage.gameObject.SetActive(true);
				quickSlotPanel.itemImage.sprite = inventorySlot.item.GiveItemImage();
				quickSlotPanel.itemImage.CrossFadeAlpha(1f, 0.05f, true);
				quickSlotPanel.stackText.gameObject.SetActive(true);
				quickSlotPanel.stackText.text = "" + inventorySlot.stacks;
			}
			else
			{
				quickSlotPanel.itemImage.gameObject.SetActive(false);
				quickSlotPanel.stackText.gameObject.SetActive(false);
			}
		}
	}

	public void UpdateList()
	{
		if (_updateQuickSlotSize + quickSlots.Count < _inventorySize) _updateQuickSlotSize = quickSlots.Count;
		else _updateQuickSlotSize = 0;

		RefreshInventory();
	}

	public void ClearSlot(ItemSlotInfo slot)
	{
		slot.item = null;
		slot.stacks = 0;
	}

	public void Refresh()
	{
		if (_inventoryMenu.activeSelf) RefreshInventory();
	}
}
