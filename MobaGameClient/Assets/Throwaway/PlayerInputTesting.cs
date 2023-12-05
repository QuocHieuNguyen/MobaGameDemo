using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputTesting : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    // Start is called before the first frame update
    void Start()
    {
        _playerInput.actions["Movement"].performed += OnMovement;
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        Vector3 rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
        Debug.Log(rawInputMovement);
    }
}
