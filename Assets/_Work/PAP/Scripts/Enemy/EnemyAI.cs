using System;
using System.Collections.Generic;
using _Work.PAP.Scripts.Player;
using Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.PAP.Scripts.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private List<EnemyAttackPatternSO> patterns;
        
        private float lastFireTime;
        private PlayerController player;
        private EnemyAttackPatternSO currentPattern;
        private Rigidbody2D _rb;
        private float lastPatrolTime;

        private void Awake()
        {
            currentPattern = patterns[Random.Range(0, patterns.Count)];
            lastPatrolTime = Time.time + 1.5f;
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            SpawnBullet();
            if (Time.time > lastPatrolTime)
            {
                lastPatrolTime = Time.time + Random.Range(0, 1.5f);
                _rb.linearVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }

        private void SpawnBullet()
        {
            if (Time.time < lastFireTime) return;
            lastFireTime = Time.time + currentPattern.cooldown;
            Vector2 baseDir = (player.transform.position + new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),0) - transform.position).normalized;
            float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

            int count = currentPattern.bulletCount;
            float degree = currentPattern.degree;

            float step = count > 1 ? degree / (count - 1) : 0f;
            float startAngle = baseAngle - degree * 0.5f;
            for (int i = 0; i < currentPattern.bulletCount; i++)
            {
                float angle = startAngle + step * i;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                
                IPoolable bullet = PoolManager.Instance.Pop(currentPattern.bullet.ItemName);
                bullet.GameObject.transform.position = transform.position + (Vector3)(dir * 2f);
                bullet.GameObject.GetComponent<Bullet>().Init(dir);
            }
            currentPattern = patterns[Random.Range(0, patterns.Count)];
        }
    }
}