using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Popup : UI_Base
{
	public override bool Init()
	{
		if (!base.Init()) return false;

		GameManager.Instance.ui_Manager.SetCanvas(gameObject);
		return true;
	}
}
