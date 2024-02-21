using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
	[HideInInspector] public ItemPanel sourceItemPanel;
	[HideInInspector] public int splitSize;

	public GameObject mouseItemUI;
	public Image mouseCursor;
	public ItemSlotInfo itemSlot;
	public Image itemImage;
	public TextMeshProUGUI stackText;

	private void Update()
	{
		transform.position = Input.mousePosition;
		if(Cursor.lockState == CursorLockMode.Locked)
		{
			mouseCursor.enabled = false;	
			mouseItemUI.SetActive(false);
		}
		else
		{
			mouseCursor.enabled = true;

			if (itemSlot.item != null)
			{
				mouseItemUI.SetActive(true);
			}
			else
			{
				mouseItemUI.SetActive(false);
			}
		}

		if(itemSlot.item != null)
		{
			if(/*Input.GetAxis("Mouse ScrollWheel") > 0*/ Input.GetKeyDown(KeyCode.R) && splitSize < itemSlot.stacks)
			{
				splitSize++;
			}

			if (/*Input.GetAxis("Mouse ScrollWheel") < 0*/ Input.GetKeyDown(KeyCode.T) && splitSize > 1)
			{
				splitSize--;
			}
			stackText.text = "" + splitSize;

			if (splitSize == itemSlot.stacks) sourceItemPanel.stackText.gameObject.SetActive(false);
			else
			{
				sourceItemPanel.stackText.gameObject.SetActive(true);
				sourceItemPanel.stackText.text = "" + (itemSlot.stacks - splitSize);
			}
		}
	}

	public void SetUI()
	{
		stackText.text = "" + splitSize;
		itemImage.sprite = itemSlot.item.GiveItemImage();
	}

	public void EmptySlot()
	{
		itemSlot = new ItemSlotInfo(null, 0);
	}
}
