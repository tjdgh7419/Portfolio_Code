using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler, IEndDragHandler
{

	private Mouse _mouse;
	private bool _click;

	[HideInInspector] public InventoryView inventory;
	public ItemSlotInfo itemSlot;
	public Image itemImage;
	public TextMeshProUGUI stackText;
	public Outline equipOutline;
	public GameObject itemInfo;
	public TextMeshProUGUI itemName;

	public bool isQuickSlot;

	private void OnDisable()
	{
		OffItemInfo();
	}

	private void SwapItem(ItemSlotInfo slotA, ItemSlotInfo slotB)
	{
		ItemSlotInfo tempItem = new ItemSlotInfo(slotA.item, slotA.stacks);

		slotA.item = slotB.item;
		slotA.stacks = slotB.stacks;

		slotB.item = tempItem.item;
		slotB.stacks = tempItem.stacks;
	}

	private void StackItem(ItemSlotInfo source, ItemSlotInfo destination, int amount)
	{
		int slotAvailable = destination.item.MaxStacks() - destination.stacks;
		if (slotAvailable == 0) return;

		if (amount > slotAvailable)
		{
			source.stacks -= slotAvailable;
			destination.stacks = destination.item.MaxStacks();

		}

		if (amount <= slotAvailable)
		{
			destination.stacks += amount;
			if (source.stacks == amount) inventory.ClearSlot(source);
			else source.stacks -= amount;
		}
	}

	private void PickupItem()
	{
		_mouse.itemSlot = itemSlot;
		_mouse.sourceItemPanel = this;
		if (Input.GetKey(KeyCode.LeftShift) && itemSlot.stacks > 1) _mouse.splitSize = itemSlot.stacks / 2;
		else _mouse.splitSize = itemSlot.stacks;
		_mouse.SetUI();
	}

	private void FadeOut()
	{
		itemImage.CrossFadeAlpha(0.3f, 0.05f, true);
	}

	private void DropItem()
	{
		itemSlot.item = _mouse.itemSlot.item;
		if (_mouse.splitSize < _mouse.itemSlot.stacks)
		{
			itemSlot.stacks = _mouse.splitSize;
			_mouse.itemSlot.stacks -= _mouse.splitSize;
			_mouse.EmptySlot();
		}
		else
		{
			itemSlot.stacks = _mouse.itemSlot.stacks;
			inventory.ClearSlot(_mouse.itemSlot);
		}
	}

	private void TrashCan(PointerEventData eventData)
	{
		RaycastResult raycastResult = eventData.pointerCurrentRaycast;

		if (raycastResult.isValid && raycastResult.gameObject.CompareTag("Trash"))
		{
			if(itemSlot.stacks == _mouse.splitSize)
			{
				inventory.ClearSlot(itemSlot);
			}
			else 
			{
				itemSlot.stacks -= _mouse.splitSize;
			}
			
		}
	}

	private void OnClick()
	{
		if (inventory != null)
		{
			_mouse = inventory.mouse;

			if (_mouse.itemSlot.item == null)
			{
				if (itemSlot.item != null)
				{
					inventory.isPickUp = true;
					PickupItem();
					FadeOut();
				}
			}
			else
			{
				inventory.isPickUp = false;

				if (itemSlot == _mouse.itemSlot)
				{
					inventory.RefreshInventory();
					inventory.RefreshQuickSlots();
				}

				else if (itemSlot.item == null)
				{
					DropItem();
					inventory.RefreshInventory();
					inventory.RefreshQuickSlots();
				}

				else if (itemSlot.item.GiveName() != _mouse.itemSlot.item.GiveName())
				{
					SwapItem(itemSlot, _mouse.itemSlot);
					inventory.RefreshInventory();
					inventory.RefreshQuickSlots();
				}

				else if (itemSlot.stacks < itemSlot.item.MaxStacks())
				{
					StackItem(_mouse.itemSlot, itemSlot, _mouse.splitSize);
					inventory.RefreshInventory();
					inventory.RefreshQuickSlots();
				}
			}
		}
	}

	private void OnItemInfo()
	{		
		if(itemSlot.item != null && itemInfo != null && !isQuickSlot && !inventory.isPickUp)
		{
			itemName.text = itemSlot.item.GiveName();
			itemInfo.SetActive(true);
		}
	}

	private void OffItemInfo()
	{
		if (itemSlot.item != null && itemInfo != null && !isQuickSlot)
		{
			itemInfo.SetActive(false);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		UnityEngine.Debug.Log("OnPointerEnter");
		eventData.pointerPress = this.gameObject;
		OnItemInfo();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OffItemInfo();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		UnityEngine.Debug.Log("OnPointerDown");
		_click = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		UnityEngine.Debug.Log("OnPointerUp");
		if (_click)
		{
			OnClick();
			_click = false;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (_click)
		{
			OnClick();
			_click = false;
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		UnityEngine.Debug.Log("OnDrop");
		OnClick();
		_click = false;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		UnityEngine.Debug.Log("EndDrag");
		TrashCan(eventData);

		if (eventData.pointerPress == null)
		{
			inventory.RefreshInventory();
			inventory.RefreshQuickSlots();
		}
	}
}
