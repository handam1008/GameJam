using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    private Vector2 _moveDir;
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDir * _speed;
    }

    private void OnMove(InputValue value)
    {
        _moveDir = value.Get<Vector2>();
    }
}
