using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
	public override bool Init()
	{
		if (!base.Init()) return false;

		GameManager.Instance.ui_Manager.SetCanvas(gameObject);
		return true;
	}
}
