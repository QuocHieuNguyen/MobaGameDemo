using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : GameInput.IGameplayActions, GameInput.IUIActions
{
    private GameInput _gameInput;

    public event Action<Vector2> MoveEvent;
    public event Action JumpEvent;
    public event Action PauseEvent;
    public event Action ResumeEvent;
    
    public void Initialize()
    {
        _gameInput = new GameInput();
        _gameInput.Gameplay.SetCallbacks(this);
        _gameInput.UI.SetCallbacks(this);
    }

    public void SetGameplay()
    {
        _gameInput.Gameplay.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log($"context {context.phase} and value {context.ReadValue<Vector2>()}");
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            
        }

        
    }

    public void OnPause(InputAction.CallbackContext context)
    {

    }

    public void OnResume(InputAction.CallbackContext context)
    {

    }
}
