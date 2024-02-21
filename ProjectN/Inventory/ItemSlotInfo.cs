
using UnityEditor.UIElements;

[System.Serializable]
public class ItemSlotInfo1 
{
	public Item item;
	public string name;
	public int stacks;

	public ItemSlotInfo1(Item newItem, int newStacks)
	{
		item = newItem;
		stacks = newStacks;
	}
}
