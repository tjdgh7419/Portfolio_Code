using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	public Player player;

	public ItemManager itemManager;
	public EquipManager equipManager;
	public CraftManager craftManager;
	public UIManager uiManager;
	public InteractionManager interactionManager;
	public RoundManager roundManager;
	public MonsterManager monsterManager;
	public QuestManager questManager;
	public Inventory inventory;
	public CraftPanel craft;
	public PlayerConditionManager playerConditionManager;
	public GraphicManager garphicManager;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		uiManager.OpenUI<PlayerUI>();
		UIManager.Instance.IsOnUI = false;
		SoundManager.Instance.BackMusic.WaveOff();
	}
}
