using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class UI_Manager : Singleton<UI_Manager>
{
	private List<UI_Popup> _popupList = new List<UI_Popup>();

	public UI_Scene curSceneUI { get; private set; }
	public GameObject Root
	{
		get
		{
			GameObject root = GameObject.Find("UI_Root");
			if (root == null) root = new GameObject { name = "UI_Root" };
			return root;
		}
	}
	public void SetCanvas(GameObject go)
	{
		Canvas canvas = go.GetOrAddComponent<Canvas>();
		canvas.sortingOrder = go.GetComponent<Canvas>().sortingOrder;
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.overrideSorting = true;
	}

	public T MakeSubItem<T> (string name = null, Transform parent = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

		GameObject loadPrefab = Resources.Load<GameObject>($"Prefabs/UI/SubItem/{name}");

		if (loadPrefab == null)
		{
			Debug.Log($"Failed to load : Prefabs/UI/SubItem/{name}");
			return null;
		}

		GameObject subItemPrefab = Instantiate(loadPrefab, parent);

		T subItemUI = subItemPrefab.GetComponent<T>();
		if (subItemUI == null) subItemUI = subItemPrefab.AddComponent<T>();

		if (parent != null) subItemPrefab.transform.SetParent(parent);
		subItemPrefab.transform.localScale = Vector3.one;

		return subItemUI;
	}

	public T ShowScene<T> () where T : UI_Scene
	{
		GameObject loadPrefab = Resources.Load<GameObject>($"Prefabs/UI/Scene/{name}");

		if (loadPrefab == null)
		{
			Debug.Log($"Failed to load : Prefabs/UI/Scene/{name}");
			return null;
		}

		GameObject scenePrefab = Instantiate(loadPrefab);

		T sceneUI = scenePrefab.GetComponent<T>();
		if (sceneUI == null) sceneUI = scenePrefab.AddComponent<T>();

		curSceneUI = sceneUI;
		scenePrefab.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopup<T>(string name = null, Transform parent = null) where T : UI_Popup
	{
		if(string.IsNullOrEmpty(name)) name = typeof(T).Name;

		foreach (UI_Popup uiPopup in _popupList)
		{
			if(uiPopup.name == name) return uiPopup as T;
		}

		GameObject loadPrefab = Resources.Load<GameObject>($"Prefabs/UI/Popup/{name}");

		if (loadPrefab == null)
		{
			Debug.Log($"Failed to load : Prefabs/UI/Popup/{name}");
			return null;
		}

		GameObject popupPrefab = Instantiate(loadPrefab, parent);

		T popupUI = popupPrefab.GetComponent<T>();
		if(popupUI == null) popupUI = popupPrefab.AddComponent<T>();
		
		_popupList.Add(popupUI);

		if (parent != null) popupPrefab.transform.SetParent(parent);
		else popupPrefab.transform.SetParent(Root.transform);

		popupPrefab.transform.localScale = Vector3.one;

		return popupUI;
	}

	public void ClosePopup(UI_Popup popup)
	{
		if (_popupList.Count == 0)
		{
			return;
		}

		if (_popupList.Contains(popup))
		{
			_popupList.Remove(popup);
			Destroy(popup.gameObject);
			return;
		}
		else
		{
			Debug.Log("Popup information is not included");
			return;
		}
	}

	public void CloseAllPopup()
	{
		while (_popupList.Count > 0) _popupList.Clear(); ;
	}

	public void Clear()
	{
		CloseAllPopup();
		curSceneUI = null;
	}
}
