using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableManager : Singleton<AddressableManager>
{
	private Dictionary<string, Sprite> _itemSpriteDic = new Dictionary<string, Sprite>();
	private Dictionary<string, GameObject> _itemObjectDic = new Dictionary<string, GameObject>();

	public async Task<Sprite> LoadSpriteAsync(string path)
	{
		if(_itemSpriteDic.ContainsKey(path))
		{
			return _itemSpriteDic[path];
		}

		AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(path);
		await handle.Task;

		if(handle.Status == AsyncOperationStatus.Succeeded)
		{
			Sprite loadedSprite = handle.Result;
			_itemSpriteDic[path] = loadedSprite;
			return loadedSprite;
		}
		else
		{
			Debug.LogError($"Fail to load sprite");
			return null;
		}
	}

	public async Task<GameObject> LoadObjectAsync(string path, Transform transform = null)
	{
		if (_itemObjectDic.ContainsKey(path))
		{
			return _itemObjectDic[path];
		}

		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(path, transform);
		await handle.Task;
		
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			GameObject loadedObject = handle.Result;
			_itemObjectDic[path] = loadedObject;
			return loadedObject;
		}
		else
		{
			Debug.LogError("Fail to load object");
			return null;
		}
	}
}


