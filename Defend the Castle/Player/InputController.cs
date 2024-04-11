using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
	public event Action<Vector2> OnMoved;
	public event Action<Vector2> OnLookRotation;
	public event Action OnJump;

	private void Awake()
	{
		if (TryGetComponent(out PlayerInput playerInput))
		{
			InputAction action = playerInput.actions.FindAction("Move");
			action.performed += CallOnMove;
			action.canceled += CallOnMove;
			action = playerInput.actions.FindAction("Look");
			action.performed += CallOnLookRotation;
			action.canceled += CallOnLookRotation;
			action = playerInput.actions.FindAction("Jump");
			action.started += CallOnJump;
			action = playerInput.actions.FindAction("Interaction");
			action.started += CallOnInteraction;
			action = playerInput.actions.FindAction("Pause");
			action.started += CallOnPause;
            action = playerInput.actions.FindAction("Quest");
            action.started += CallOnQuest;
        }
	}

	public void CallOnMove(InputAction.CallbackContext callbackContext)
	{
		Vector2 direction = callbackContext.ReadValue<Vector2>();
		direction.Normalize();
		OnMoved?.Invoke(direction);
	}

	public void CallOnLookRotation(InputAction.CallbackContext callbackContext)
	{
		if (!UIManager.Instance.IsOnUI)
		{
            Vector2 mouseDelta = callbackContext.ReadValue<Vector2>();
            OnLookRotation?.Invoke(mouseDelta);
        }
		else
		{
            Vector2 mouseDelta = Vector2.zero;
            OnLookRotation?.Invoke(mouseDelta);
        }
	}

	public void CallOnJump(InputAction.CallbackContext callbackContext)
	{
		OnJump?.Invoke();
	}

	public void CallOnInteraction(InputAction.CallbackContext callbackContext)
	{
		if (!UIManager.Instance.IsOnUI)
        {
            GameManager.Instance.interactionManager.CallShowWindow();
        }
	}

	public void CallOnPause(InputAction.CallbackContext callbackContext)
	{
		if (Time.timeScale != 0 && !UIManager.Instance.IsOnUI)
		{
			UIManager.Instance.OpenUI<PausePanel>();
			Time.timeScale = 0;			
		}	
	}
    public void CallOnQuest(InputAction.CallbackContext callbackContext)
    {
		if (!UIManager.Instance.IsOnUI)
		{
            UIManager.Instance.OpenUI<QuestListPanel>();
        }
    }
}
