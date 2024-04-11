using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Storage : MonoBehaviour, IInteractable
{
	UI_Storage storage;

	private void Start()
	{
		Debug.Log("상자 인스턴스 생성");
		storage = UI_Manager.Instance.MakeSubItem<UI_Storage>();
	}

	public void OnInteract(GameObject interactor)
	{
		Character character = interactor.GetComponent<Character>();
		InventoryController inventoryController = character.GetInventory();

		storage.storageController.CopyInventoryList(inventoryController.GetItemList());

		if (storage.gameObject.activeSelf)
		{
			storage.gameObject.SetActive(false);
		}
		else
		{
			storage.gameObject.SetActive(true);
		}

		Debug.Log("Interacting with the Box");		
	}
}
