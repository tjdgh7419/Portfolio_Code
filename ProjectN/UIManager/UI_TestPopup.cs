using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TestPopup : UI_Popup
{
	enum Images
	{
		image1,
	}

	enum Texts
	{
		Text1,
	}
	public Image[] image;
	public TextMeshProUGUI[] text;

	public override bool Init()
	{
		if (!base.Init())
			return false;

		_init = true;
		Bind(typeof(Images), image);
		Bind(typeof(Texts), text);
		UIUpdate();
		return true;
	}

	private void UIUpdate()
	{
		Get<TextMeshProUGUI>((int)Texts.Text1).text = "hi";
	}
}
