using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quantitiyTxt;

    private ItemSlot curSlot;
    private Outline outline;

    public int index;
    public bool equipped;

	private void Awake()
	{
		outline = GetComponent<Outline>();
	}

    public void Set(ItemSlot slot)
    {
        curSlot = slot;
        icon.gameObject.SetActive(true);
        
        icon.sprite = slot.item.icon;
        quantitiyTxt.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;

        if(outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void CraftSet(ItemSlot slot)
    {
		quantitiyTxt.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;
	}

    public void Clear()
    {
        curSlot = null;
        icon.gameObject.SetActive(false);
        quantitiyTxt.text = string.Empty;
    }

    public void OnButtonClick()
    {
        GameManager.Instance.inventory.SelectItem(index);    
	}
}
