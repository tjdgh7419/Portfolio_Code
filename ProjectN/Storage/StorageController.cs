using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class StorageController : MonoBehaviour
{
	[SerializeReference] protected List<ItemSlotInfo> items = new List<ItemSlotInfo>();

	protected int _storageSize = 10;

	[HideInInspector] protected StorageModel storageModel;
	[HideInInspector] protected StorageView storageView;

	[Space]
	[Header("Storage Menu Components")]
	[SerializeField] protected GameObject storageMenu;
	[SerializeField] protected GameObject itemPanel;
	[SerializeField] protected GameObject itemPanelGrid;

	[Header("Mouse")]
	[HideInInspector] protected Mouse mouse;

	[SerializeField] protected string[] AddItems;

	private void Start()
	{
		mouse = Mouse.Instance;
		InitializeStorageData();
	}

	public virtual void InitializeStorageData()
	{
		storageModel = new StorageModel(_storageSize, items);
		storageView = new StorageView(items, mouse, storageMenu, itemPanel, itemPanelGrid);

		storageModel.OnUpdateInventory += storageView.Refresh;

		Refresh();
	}

	public virtual void AddItem(string itemName, int amount)
	{
		storageModel.AddItem(itemName, amount);
	}

	private void Refresh()
	{
		if (storageView != null) storageView.Refresh();
	}

	public void CopyInventoryList(List<ItemSlotInfo> list)
	{
		items.Clear(); 
		items.AddRange(list);

		Refresh();
	}
}
