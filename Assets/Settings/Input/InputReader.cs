using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Core/Input Reader")]
public class InputReader : SerializableScriptableObject, GameInput.IGameplayActions, GameInput.IDialoguesActions, GameInput.IMenusActions
{
	// Assign delegate{} to events to initialise them with an empty delegate
	// so we can skip the null check when we use them

	// Gameplay
	public event Action JumpEvent = delegate { };
	public event Action JumpCanceledEvent = delegate { };
	public event Action AttackEvent = delegate { };
	public event Action AttackCanceledEvent = delegate { };
	public event Action InteractEvent = delegate { }; // Used to talk, pickup objects, interact with tools like the cooking cauldron
	public event Action InventoryActionButtonEvent = delegate { };
	public event Action SaveActionButtonEvent = delegate { };
	public event Action ResetActionButtonEvent = delegate { };
	public event Action<Vector2> MoveEvent = delegate { };
	public event Action<Vector2, bool> CameraMoveEvent = delegate { };
	public event Action EnableMouseControlCameraEvent = delegate { };
	public event Action DisableMouseControlCameraEvent = delegate { };
	public event Action StartedRunning = delegate { };
	public event Action StoppedRunning = delegate { };

	// Shared between menus and dialogues
	public event Action MoveSelectionEvent = delegate { };

	// Dialogues
	public event Action AdvanceDialogueEvent = delegate { };

	// Menus
	public event Action MenuMouseMoveEvent = delegate { };
	public event Action MenuClickButtonEvent = delegate { };
	public event Action MenuUnpauseEvent = delegate { };
	public event Action MenuPauseEvent = delegate { };
	public event Action MenuCloseEvent = delegate { };
	public event Action OpenInventoryEvent = delegate { }; // Used to bring up the inventory
	public event Action CloseInventoryEvent = delegate { }; // Used to bring up the inventory
	public event Action<float> TabSwitched = delegate { };


	private GameInput _gameInput;

	private void OnEnable()
	{
		if (_gameInput == null)
		{
			_gameInput = new GameInput();

			_gameInput.Menus.SetCallbacks(this);
			_gameInput.Gameplay.SetCallbacks(this);
			_gameInput.Dialogues.SetCallbacks(this);
		}
	}

	private void OnDisable()
	{
		DisableAllInput();
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				AttackEvent.Invoke();
				break;
			case InputActionPhase.Canceled:
				AttackCanceledEvent.Invoke();
				break;
		}
	}

	public void OnOpenInventory(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			OpenInventoryEvent.Invoke();
	}
	public void OnCancel(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			MenuCloseEvent.Invoke();
	}

	public void OnInventoryActionButton(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			InventoryActionButtonEvent.Invoke();
	}

	public void OnSaveActionButton(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			SaveActionButtonEvent.Invoke();
	}

	public void OnResetActionButton(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			ResetActionButtonEvent.Invoke();
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed) 
			InteractEvent.Invoke();
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			JumpEvent.Invoke();

		if (context.phase == InputActionPhase.Canceled)
			JumpCanceledEvent.Invoke();
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		MoveEvent.Invoke(context.ReadValue<Vector2>());
	}

	public void OnRun(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				StartedRunning.Invoke();
				break;
			case InputActionPhase.Canceled:
				StoppedRunning.Invoke();
				break;
		}
	}

	public void OnPause(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			MenuPauseEvent.Invoke();
	}

	public void OnRotateCamera(InputAction.CallbackContext context)
	{
		CameraMoveEvent.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
	}

	public void OnMouseControlCamera(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			EnableMouseControlCameraEvent.Invoke();

		if (context.phase == InputActionPhase.Canceled)
			DisableMouseControlCameraEvent.Invoke();
	}

	private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

	public void OnMoveSelection(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			MoveSelectionEvent.Invoke();
	}

	public void OnAdvanceDialogue(InputAction.CallbackContext context)
	{

		if (context.phase == InputActionPhase.Performed)
			AdvanceDialogueEvent.Invoke();
	}

	public void OnConfirm(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			MenuClickButtonEvent.Invoke();
	}


	public void OnMouseMove(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			MenuMouseMoveEvent.Invoke();
	}

	public void OnUnpause(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			MenuUnpauseEvent.Invoke();
	}

	public void EnableDialogueInput()
	{
		_gameInput.Menus.Enable();
		_gameInput.Gameplay.Disable();
		_gameInput.Dialogues.Enable();
	}

	public void EnableGameplayInput()
	{
		_gameInput.Menus.Disable();
		_gameInput.Dialogues.Disable();
		_gameInput.Gameplay.Enable();
	}

	public void EnableMenuInput()
	{
		_gameInput.Dialogues.Disable();
		_gameInput.Gameplay.Disable();

		_gameInput.Menus.Enable();
	}

	public void DisableAllInput()
	{
		_gameInput.Gameplay.Disable();
		_gameInput.Menus.Disable();
		_gameInput.Dialogues.Disable();
	}

	public void OnChangeTab(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			TabSwitched.Invoke(context.ReadValue<float>());
	}

	public bool LeftMouseDown() => Mouse.current.leftButton.isPressed;

	public void OnClick(InputAction.CallbackContext context)
	{

	}

	public void OnSubmit(InputAction.CallbackContext context)
	{

	}

	public void OnPoint(InputAction.CallbackContext context)
	{

	}
	
	public void OnRightClick(InputAction.CallbackContext context)
	{

	}

	public void OnNavigate(InputAction.CallbackContext context)
	{

	}

	public void OnCloseInventory(InputAction.CallbackContext context)
	{
		CloseInventoryEvent.Invoke();
	}
}
