using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReaderHandler : MonoBehaviour
{
    private InputReader _input;
    [SerializeField] private float speed;

    [SerializeField] private float jumpSpeed;

    private Vector2 _moveDirection;
    private bool _isJumping;

    private void Start()
    {
        _input = new InputReader();
        _input.MoveEvent += InputOnMoveEvent;
        _input.Initialize();
        _input.SetGameplay();
    }

    private void InputOnMoveEvent(Vector2 value)
    {
        _moveDirection = value;
        Debug.Log($"move direction value {_moveDirection}");
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += new Vector3(_moveDirection.x, 0, _moveDirection.y) * speed * Time.deltaTime;
    }
}
