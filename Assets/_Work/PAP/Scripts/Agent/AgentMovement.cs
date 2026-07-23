using System;
using UnityEngine;
using UnityEngine.AI;

namespace _Work.PAP.Scripts.Agent
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AgentMovement : MonoBehaviour
    {
        private Vector2 _moveDir;
        private Rigidbody2D _rb;

        [SerializeField] private float speed = 5f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _moveDir * speed;
        }
        
        public void SetMovementDir(Vector2 dir)
        {
            _moveDir = dir;
        }
    }
}