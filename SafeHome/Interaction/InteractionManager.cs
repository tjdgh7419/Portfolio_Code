using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
	string GetInteractPrompt();
	void OnInteract();
	void PlayInteractionSound();
}

public class InteractionManager : MonoBehaviour
{
	public float checkRate = 0.05f;
	public float maxCheckDistance;

	private float lastCheckTime;

	private Camera camera;
	private GameObject curInteractGameObject;
	private IInteractable curInteractable;

	public TextMeshProUGUI promptText;
	public LayerMask layerMask;

	void Start()
	{
		camera = Camera.main;
		GameManager.Instance._interactionManager = this;
	}

	void Update()
	{
		if (Time.time - lastCheckTime > checkRate)
		{
			lastCheckTime = Time.time;

			Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
			{
				if (hit.collider.gameObject != curInteractGameObject)
				{
					curInteractGameObject = hit.collider.gameObject;
					curInteractable = hit.collider.GetComponent<IInteractable>();
					SetPromptText();
				}
			}
			else
			{
				curInteractGameObject = null;
				curInteractable = null;
				promptText.gameObject.SetActive(false);
			}
		}
	}

	private void SetPromptText()
	{
		promptText.gameObject.SetActive(true);
		promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt());
	}

	public void OnInteractInput(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.phase == InputActionPhase.Started && curInteractable != null)
		{		
			curInteractable.OnInteract();
			curInteractGameObject = null;
			curInteractable = null;
			promptText.gameObject.SetActive(false);
		}
	}
}
