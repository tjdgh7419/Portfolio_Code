using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Base : MonoBehaviour
{
    protected bool _init = false;
	private Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

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
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {   
            objects[i] = uiData[i];
			
			if (objects[i] == null) Debug.Log($"Fail Bind {objects[i]}");
		}
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (!_objects.TryGetValue(typeof(T), out objects)) return null;

        Debug.Log(objects[idx].ToString());
        return objects[idx] as T;
    }
}
