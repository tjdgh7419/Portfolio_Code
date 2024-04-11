using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class StorageView 
{
	private List<ItemPanel> _existingPanels = new List<ItemPanel>();
	private GameObject _storageMenu;
	private GameObject _itemPanel;
	private GameObject _itemPanelGrid;

	protected List<ItemSlotInfo> items;

	public Mouse mouse;
	public bool isPickUp;

	public StorageView(List<ItemSlotInfo> items, Mouse mouse, 
		GameObject storageMenu, GameObject itemPanel, GameObject itemPanelGrid)
	{
		this.items = items;
		this.mouse = mouse;
		_storageMenu = storageMenu;
		_itemPanel = itemPanel;
		_itemPanelGrid = itemPanelGrid;
	}

	public void ShowStorage()
	{
		if (_storageMenu.activeSelf)
		{
			_storageMenu.SetActive(false);
			mouse.EmptySlot();
			RefreshStorage();
		}
		else
		{
			_storageMenu.SetActive(true);
			RefreshStorage();
		}
	}

	public async void RefreshStorage()
	{
		_existingPanels = _itemPanelGrid.GetComponentsInChildren<ItemPanel>().ToList();
		Debug.Log(_existingPanels.Count);
		List<ItemSlotInfo> itemsCopy = new List<ItemSlotInfo>(items);
		int index = 0;

		foreach (ItemSlotInfo i in itemsCopy)
		{
			i.name = "" + (index + 1);
			if (i.item != null) i.name += ": " + i.item.Name;
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
					panel.itemImage.sprite = await AddressableManager.Instance.LoadSpriteAsync(i.item.UIImagePath);
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
	}

	public virtual void Refresh()
	{
		if (_storageMenu.activeSelf) RefreshStorage();
	}

	public void ClearSlot(ItemSlotInfo slot)
	{
		slot.item = null;
		slot.stacks = 0;
	}
}
