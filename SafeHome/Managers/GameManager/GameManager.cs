﻿using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public UIManager _uiManager;
    public BuildManager _buildManager;
    public PrefabManager prefabManager;
    public DayManager _dayManager;
    public MonsterSpawnManager _monsterSpawnManager;
    public SoundManager _soundManager;
    public ItemManager _itemManager;
    public EquipManager _equipManager;
    public InteractionManager _interactionManager;
    public CraftManager _craftManager;
    public ConditionManager _conditionManager;

	private GameObject player;
	public ResourceDisplayUI resourceDisplayUI;
	public Inventory inventory;

	public static GameManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = GameObject.FindWithTag("GameManager");
				if (obj != null)
					_instance = obj.GetComponent<GameManager>();
				if (_instance == null)
				{
					obj = Instantiate(Resources.Load<GameObject>("GameManager"));
					_instance = obj.GetComponent<GameManager>();
				}
			}

			return _instance;
		}
		private set
		{
			_instance = value;
		}
	}

	private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public GameObject GetPlayer()
    {
        if (!player)
            player = GameObject.FindWithTag("Player");
        return player;
    }
}
