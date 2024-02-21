using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ItemSlot
{
	public ItemData item;
	public int quantity;
}
public class Inventory : MonoBehaviour
{
	public ItemSlotUI[] uiSlot;
	public ItemSlot[] slots;
	public GameObject inventoryWindow;

	[Header("Selected Item")]
	private ItemSlot _selectedItem;
	private int selectedItemIndex;
	public TextMeshProUGUI selectedItemName;
	public TextMeshProUGUI selectedItemDescription;
	public GameObject useButton;
	public GameObject equipButton;
	public GameObject unEquipButton;
	public GameObject dropButton;

	private ItemData _pickaxe;
	private ItemData _hammer;
	private int _curEquipIndex;
	private PlayerMovement _controller;
	[Header("Events")]
	public UnityEvent onOpenInventory;
	public UnityEvent onCloseInventory;
	// Start is called before the first frame update

	private Dictionary<ItemData, int> _ItemTotalCount;
	private void Awake()
	{
		_controller = GetComponent<PlayerMovement>();
		_ItemTotalCount = new Dictionary<ItemData, int>();
	}

	private void Start()
	{
		GameManager.Instance.inventory = this;
		_pickaxe = GameManager.Instance._itemManager.Pickax;
		_hammer = GameManager.Instance._itemManager.Hammer;
		inventoryWindow.SetActive(false);
		slots = new ItemSlot[uiSlot.Length];

		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = new ItemSlot();
			uiSlot[i].index = i;
			uiSlot[i].Clear();

		}
		ClearSelectedItemWindow();
		AddItem(_pickaxe);
		AddItem(_hammer);
	}

	public void Toggle()
	{
		if (inventoryWindow.activeInHierarchy)
		{
			GameManager.Instance._uiManager.RemoveUICount(gameObject);
			inventoryWindow.SetActive(false);
			onCloseInventory?.Invoke();
		}
		else
		{
			GameManager.Instance._uiManager.AddUICount(gameObject);
			inventoryWindow.SetActive(true);
			onOpenInventory?.Invoke();
		}
	}

	public void OnInventoryButton(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.phase == InputActionPhase.Started)
		{
			Toggle();
		}
	}

	public bool IsOpen()
	{
		return inventoryWindow.activeInHierarchy;
	}

	public void AddItem(ItemData item) 
	{
		if (item.canStack)
		{
			ItemSlot slotToStakTo = GetItemStack(item);
			if (slotToStakTo != null)
			{
				slotToStakTo.quantity++;
				_ItemTotalCount[item] += 1;
				UpdateUI();
				GameManager.Instance.resourceDisplayUI.ShowGetResource(item);
				return;
			}
		}

		ItemSlot emptySlot = GetEmptySlot();

		if (emptySlot != null)
		{
			emptySlot.item = item;
			emptySlot.quantity = 1;
			if (_ItemTotalCount.ContainsKey(item))
				_ItemTotalCount[item] += 1;
			else
				_ItemTotalCount[item] = 1;
			UpdateUI();
			GameManager.Instance.resourceDisplayUI.ShowGetResource(item);
			return;
		}

		return;
	}

	void UpdateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item != null)
				uiSlot[i].Set(slots[i]);
			else
				uiSlot[i].Clear();
		}
	}

	ItemSlot GetItemStack(ItemData item) 
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)
				return slots[i];
		}
		return null;
	}

	ItemSlot GetEmptySlot() 
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item == null)
				return slots[i];
		}
		return null;
	}

	public void SelectItem(int index)
	{
		if (slots[index].item == null)
			return;

		_selectedItem = slots[index];
		selectedItemIndex = index;

		selectedItemName.text = _selectedItem.item.itemName;
		selectedItemDescription.text = _selectedItem.item.description;

		useButton.SetActive(_selectedItem.item.type == ItemType.Consumable);
		equipButton.SetActive(_selectedItem.item.type == ItemType.Equipable && !uiSlot[index].equipped);
		unEquipButton.SetActive(_selectedItem.item.type == ItemType.Equipable && uiSlot[index].equipped);
		dropButton.SetActive(_selectedItem.item.type != ItemType.Equipable);
	}

	private void ClearSelectedItemWindow()
	{
		_selectedItem = null;
		selectedItemName.text = string.Empty;
		selectedItemDescription.text = string.Empty;

		equipButton.SetActive(false);
		dropButton.SetActive(false);
		useButton.SetActive(false);
		unEquipButton.SetActive(false);
	}

	public void OnUseButton()
	{
		if (_selectedItem.item.type == ItemType.Consumable)
		{
			for (int i = 0; i < _selectedItem.item.consumables.Length; i++)
			{
				switch (_selectedItem.item.consumables[i].type)
				{
					case ConsumableType.Hunger:
						GameManager.Instance._conditionManager.hunger.Change(_selectedItem.item.consumables[i].value);
						break;

					case ConsumableType.Thirsty:
						GameManager.Instance._conditionManager.thirsty.Change(_selectedItem.item.consumables[i].value);
						break;

					case ConsumableType.Mental:
						GameManager.Instance._conditionManager.mental.Change(_selectedItem.item.consumables[i].value);
						break;
				}
				SoundManager.PlayRandomClip(_selectedItem.item.UseAudio,transform.position);
			}
		}
		RemoveSelectedItem();
	}

	public void OnPickaxEquipButton()
	{
		if (uiSlot[0].equipped)
		{
			UnEquip(0);
		}

		uiSlot[0].equipped = true;
		uiSlot[1].equipped = false;
		_curEquipIndex = 0;
		GameManager.Instance._equipManager.EquipNew(_pickaxe);
		UpdateUI();

		SelectItem(0);
		SoundManager.PlayRandomClip(_pickaxe.UseAudio,transform.position);
	}
	public void OnHammerEquipButton()
	{
		if (uiSlot[1].equipped)
		{
			UnEquip(1);
		}
		uiSlot[0].equipped = false;
		uiSlot[1].equipped = true;
		_curEquipIndex = 1;
		GameManager.Instance._equipManager.EquipNew(_hammer);
		UpdateUI();

		SoundManager.PlayRandomClip(_hammer.UseAudio,transform.position);
		SelectItem(1);
	}
	public void OnEquipButton()
	{
		if (uiSlot[_curEquipIndex].equipped)
		{
			UnEquip(_curEquipIndex);
		}

		uiSlot[selectedItemIndex].equipped = true;
		
		GameManager.Instance._equipManager.EquipNew(_selectedItem.item);
		UpdateUI();
		SoundManager.PlayRandomClip(_selectedItem.item.UseAudio,transform.position);
		SelectItem(selectedItemIndex);
	}

	void UnEquip(int index)
	{
		uiSlot[index].equipped = false;
		GameManager.Instance._equipManager.UnEquip();
		UpdateUI();

		if(selectedItemIndex == index)
		{
			SelectItem(index);
		}
	}

	public void OnUnEquipButton()
	{
		UnEquip(selectedItemIndex);
	}
	public void OnDropButton()
	{
		RemoveSelectedItem();
	}

	private void RemoveSelectedItem()
	{
		_selectedItem.quantity--;

		if (_selectedItem.quantity <= 0)
		{
			if (uiSlot[selectedItemIndex].equipped)
			{
				UnEquip(selectedItemIndex);
			}

			_selectedItem.item = null;
			ClearSelectedItemWindow();
		}
		UpdateUI();
	}
	

	public void ComsumeItem(ItemData item, int quantity)
	{
		int currentConsumeQuantity = 0;
		List<ItemSlot> removeSlots = new List<ItemSlot>();
		List<ItemSlot> slots = GetAllItemStack(item);
		foreach (var slot in slots)
		{
			if (currentConsumeQuantity >= quantity)
				break;
			removeSlots.Add(slot);
			currentConsumeQuantity += slot.quantity;
		}
			

		foreach (var slot in removeSlots)
		{
			int remain = slot.quantity - quantity;
			if (remain <= 0)
			{
				_ItemTotalCount[item] -= slot.quantity;
				slot.quantity = 0;
				slot.item = null;
				quantity = -remain;
			}
			else
			{
				_ItemTotalCount[item] -= quantity;
				slot.quantity -= quantity;
			}
		}

		slots = null;
		removeSlots = null;
		
		UpdateUI();
		return;
	}

	public bool HasItems(ItemData item, int quantity)
	{
		if (_ItemTotalCount.TryGetValue(item, out int count))
		{
			if (quantity > count)
				return false;
			else
				return true;
		}
		return false;
	}

	private List<ItemSlot> GetAllItemStack(ItemData item)
	{
		List<ItemSlot> stacks = new List<ItemSlot>();
		for (int i = slots.Length-1; i >=0; --i)
		{
			if (slots[i].item == item)
				stacks.Add(slots[i]);
		}

		return stacks;
	}
}