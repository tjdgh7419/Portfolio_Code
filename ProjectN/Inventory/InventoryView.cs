using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Assertions;

public class InventoryView : StorageView
{
	public List<QuickSlot> quickSlots;

	private int _updateQuickSlotSize = 0;
	private int _inventorySize = 10;

	public InventoryView(List<ItemSlotInfo> items, List<QuickSlot> quickSlots,
		Mouse mouse, GameObject inventoryMenu, GameObject itemPanel, GameObject itemPanelGrid)
		: base(items, mouse, inventoryMenu, itemPanel, itemPanelGrid)
	{
		this.quickSlots = quickSlots;
	}

	public override void Refresh()
	{
		base.Refresh();
		RefreshQuickSlots();
	}


	public async void RefreshQuickSlots()
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
				quickSlotPanel.itemImage.sprite = await AddressableManager.Instance.LoadSpriteAsync(inventorySlot.item.UIImagePath);
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

		RefreshStorage();
	}
}
