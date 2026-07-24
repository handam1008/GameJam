using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _Work.PAP.Scripts.Agent
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AgentMovement : MonoBehaviour
    {
        public Vector2 MoveDirection { get; private set; }
        private Rigidbody2D _rb;
        private Vector2 _currentDirection;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float slipping = 1f;
        
        public bool CanMove { get; set; } = true;
        

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _currentDirection = Vector3.Lerp(_currentDirection, MoveDirection * speed, slipping * 0.1f);
            RotateCharacter();
            if (!CanMove) return;
            _rb.linearVelocity = _currentDirection;
        }
        public void ApplyVelocity(Vector3 velocity, ForceMode2D forceMode = ForceMode2D.Impulse)
        {
            _rb.AddForce(velocity, forceMode);
        }
        public void StopImmediately()
        {
            _rb.linearVelocity = Vector3.zero;
        }
        
        public void SetMovementDir(Vector2 dir)
        {
            MoveDirection = dir;
        }

        public void PlusSpeed(float amount)
        {
            speed += amount;
            StartCoroutine(SpeedTimer());
            speed -= amount;
        }

        public IEnumerator SpeedTimer()
        {
            yield return new WaitForSeconds(0.5f);
        }

        private void RotateCharacter()
        {
            float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0, 0, angle),slipping * 0.1f);
            _rb.rotation = angle;
        }
    }
}