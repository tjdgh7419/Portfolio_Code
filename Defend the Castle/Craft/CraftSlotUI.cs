using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftSlotUI : MonoBehaviour
{
	public Button button;
	public Image icon;
	private CraftSlot curSlot;

	public int index;

	public void Set(CraftSlot slot)
	{
		curSlot = slot;
		icon.gameObject.SetActive(true);
		icon.sprite = slot.item.Image;
	}

	public void Clear()
	{
		curSlot = null;
		icon.gameObject.SetActive(false);
	}

	public void OnCraftItemClick()
	{
		GameManager.Instance.craft.SelectCraftItem(index);
	}
}
