using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour
{
    protected bool _init = false;
	private Dictionary<Type, Dictionary<string, UnityEngine.Object>> _objects = 
        new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();

	public virtual bool Init()
    {
        if (_init) return false;
        return _init = true;
    }

    private void Start()
    {
        if (_init) return;
        Init();
    }

    protected void Bind<T>(Type type, T[] uiData) where T : UnityEngine.Object
    {
        if(!type.IsEnum)
        {
            Debug.Log("The Type class is not an Enum");
            return;
        }

        string[] names = Enum.GetNames(type);
        Dictionary<string, UnityEngine.Object> objects = new Dictionary<string, UnityEngine.Object>();

		_objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            objects[names[i]] = uiData[i];

			if (objects[names[i]] == null)
			{
				Debug.Log($"Bind failed, The Object`s value is null");
			}
		}
    }

    protected T Get<T>(Enum uiName) where T : UnityEngine.Object
    {
		Dictionary<string, UnityEngine.Object> objects = null;
        if (!_objects.TryGetValue(typeof(T), out objects)) return null;

        return objects[uiName.ToString()] as T;
    }

	#region GetMethod
	protected TextMeshProUGUI GetText(Enum uiName) { return Get<TextMeshProUGUI>(uiName); }
    protected GameObject GetObject(Enum uiName) { return Get<GameObject>(uiName); }
    protected Image GetImage(Enum uiName) { return Get<Image>(uiName); }
	#endregion
}
