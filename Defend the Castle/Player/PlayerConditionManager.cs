using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class condition
{
	public float curValue;
	public float maxValue;
	public float startValue;

	public Image uiBar;
	public Image reduceUiBar;

	public void Initialize()
	{
		curValue = startValue;
	}

	public bool IsZero()
	{
		return curValue <= 0f;
	}
}

public class PlayerConditionManager : MonoBehaviour
{
	[Header("Player Stat")]
	public condition hp;
	public condition mp;

	private void Start()
	{		
		hp.Initialize();
		mp.Initialize();
	}

	private void Update()
	{
		if (hp.IsZero())
		{
			Debug.Log("Game Over");
			GameEnd("GameOverScene");
		}
	}

	private void GameEnd(string sceneName)
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		SceneManager.LoadScene(sceneName);
	}
}
