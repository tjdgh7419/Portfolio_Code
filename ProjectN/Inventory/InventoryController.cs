using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
	[SerializeReference] public List<ItemSlotInfo> items = new List<ItemSlotInfo>();

	private int _inventorySize = 10;
	private int _testWheel = 0;

	private InventoryModel _inventoryModel;
	private InventoryView _inventoryView;

	[Header("Quick Slot")]
	[SerializeField] private List<QuickSlot> quickSlots = new List<QuickSlot>();

	public void Start()
	{
		_inventoryModel = new InventoryModel(_inventorySize, items, quickSlots);
		InitializedInventoryView();

		_inventoryModel.OnUpdateInventory += _inventoryView.Refresh;

		_inventoryModel.InitializeInventoryModel();

		_inventoryModel.AddItem("Axe", 40);
		_inventoryModel.AddItem("Carrot", 5);
		_inventoryModel.EquipItem(quickSlots[0].quickSlotPanel);

		_inventoryView.RefreshInventory();
	}

	private void OnDestroy()
	{
		_inventoryModel.OnUpdateInventory -= _inventoryView.Refresh;
	}

	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			_inventoryModel.EquipItem(quickSlots[0].quickSlotPanel);
			_testWheel = 0;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			_inventoryModel.EquipItem(quickSlots[1].quickSlotPanel);
			_testWheel = 1;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			_inventoryModel.EquipItem(quickSlots[2].quickSlotPanel);
			_testWheel = 2;
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			_inventoryModel.EquipItem(quickSlots[3].quickSlotPanel);
			_testWheel = 3;
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			_inventoryModel.EquipItem(quickSlots[4].quickSlotPanel);
			_testWheel = 4;
		}

		if (Input.GetAxis("Mouse ScrollWheel") > 0 && _testWheel < quickSlots.Count - 1)
		{
			_testWheel++;
			_inventoryModel.EquipItem(quickSlots[_testWheel].quickSlotPanel);
		}

		if (Input.GetAxis("Mouse ScrollWheel") < 0 && _testWheel > 0)
		{
			_testWheel--;
			_inventoryModel.EquipItem(quickSlots[_testWheel].quickSlotPanel);
		}
	}

	private void InitializedInventoryView()
	{
		_inventoryView = GetComponent<InventoryView>();
		_inventoryView.items = items;
		_inventoryView.quickSlots = quickSlots;
	}
}
