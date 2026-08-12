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
        [SerializeField] private Rigidbody2D parentRb;

        // 여러 곳(방패/분노/신발)이 각자 배율을 등록하면 전부 곱해진다.
        // 소스별로 저장해서 서로 안 덮어쓴다
        private readonly System.Collections.Generic.Dictionary<UnityEngine.Object, float> _speedMultipliers
            = new System.Collections.Generic.Dictionary<UnityEngine.Object, float>();

        // source가 배율을 등록/갱신한다
        public void SetSpeedMultiplier(UnityEngine.Object source, float multiplier)
        {
            _speedMultipliers[source] = multiplier;
        }

        // source의 배율을 뗀다 (효과 끝날 때)
        public void ClearSpeedMultiplier(UnityEngine.Object source)
        {
            _speedMultipliers.Remove(source);
        }

        // 등록된 배율을 다 곱한 값
        private float SpeedMultiplier
        {
            get
            {
                float m = 1f;
                foreach (float v in _speedMultipliers.Values)
                    m *= v;
                return m;
            }
        }

        public bool CanMove { get; set; } = true;

        float lastPlatformRotation;
        Vector2 lastPlatformPosition;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (parentRb == null) return;
            lastPlatformRotation = parentRb.rotation;
            lastPlatformPosition = parentRb.position;
        }

        private void FixedUpdate()
        {
            _currentDirection = Vector3.Lerp(_currentDirection, MoveDirection * speed * SpeedMultiplier, slipping * 0.1f);
            RotateCharacter();
            if (!CanMove) return;

            Vector2 carriedVelocity = Vector2.zero;
            if (parentRb != null)
            {
                float deltaRotation = parentRb.rotation - lastPlatformRotation;
                Vector2 deltaPosition = parentRb.position - lastPlatformPosition;
                lastPlatformRotation = parentRb.rotation;
                lastPlatformPosition = parentRb.position;

                Vector2 relativePosition = (Vector2)transform.position - parentRb.position;
                float angularVelocityInRadians = (deltaRotation * Mathf.Deg2Rad) / Time.fixedDeltaTime;
                Vector2 rotationVelocity = new Vector2(-relativePosition.y, relativePosition.x) * angularVelocityInRadians;
                Vector2 platformLinearVelocity = deltaPosition / Time.fixedDeltaTime;

                carriedVelocity = rotationVelocity + platformLinearVelocity;
            }

            _rb.linearVelocity = _currentDirection + carriedVelocity;
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
            StartCoroutine(SpeedTimer(amount));
            
        }

        public IEnumerator SpeedTimer(float amount)
        {
            yield return new WaitForSeconds(2f);
            speed -= amount;
        }

        private void RotateCharacter()
        {
            float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0, 0, angle-90f),slipping * 0.1f);
            _rb.rotation = angle;
        }
    }
}