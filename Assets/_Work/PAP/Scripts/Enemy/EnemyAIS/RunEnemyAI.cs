using System.Collections.Generic;
using _Work.PAP.Scripts.Player;
using Systems;
using UnityEngine;
using UnityEngine.Events;

namespace _Work.PAP.Scripts.Enemy.EnemyAIS
{
    public class RunEnemyAI : MonoBehaviour
    {
        [SerializeField] private float lerpScale = 0.1f;
        [SerializeField] private UnityEvent flareEvent;
        
        private float reachTime;
        private PlayerController player;
        private EnemyAttackPatternSO currentPattern;
        private Rigidbody2D _rb;
        private float lastPatrolTime;
        private Vector3 _originalLocalPosition;
        private float _originalLocalAngleZ;
        private bool canSee;
        private bool canUseFlare = true;

        private void Awake()
        {

            _rb = GetComponent<Rigidbody2D>();
            lastPatrolTime = Time.time + 1.5f;
            reachTime = Time.time + 10f;
        }

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();
            
        }

        private void Update()
        {
            SeePlayer();
            if (Time.time > lastPatrolTime)
            {
                canSee = true;
                lastPatrolTime = Time.time + Random.Range(0, 1.5f);
                _rb.linearVelocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            }

            if (Time.time > reachTime && canUseFlare)
            {
                canUseFlare = false;
                flareEvent?.Invoke();
            }
        }

        private void SeePlayer()
        {
            if (!canSee) return;
            Vector2 direction = player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), lerpScale);
        }
        
    }
}