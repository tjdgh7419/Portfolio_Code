using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftData", menuName = "Craft Recipe")]
public class CraftData : ScriptableObject
{
    public Sprite Image;
    public ItemData[] resources;
    public int[] resourceCount;
    public ItemData Result;
}
