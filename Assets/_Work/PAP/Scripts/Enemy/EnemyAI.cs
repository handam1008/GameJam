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

        private void Awake()
        {
            currentPattern = patterns[Random.Range(0, patterns.Count)];
        }

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            transform.LookAt(player.transform);
            SpawnBullet();
        }

        private void SpawnBullet()
        {
            if (Time.time < lastFireTime) return;
            lastFireTime = Time.time + currentPattern.cooldown;
            Vector2 baseDir = (player.transform.position - transform.position).normalized;
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
                bullet.GameObject.transform.position = transform.position;
                bullet.GameObject.GetComponent<Bullet>().Init(dir);
            }
            currentPattern = patterns[Random.Range(0, patterns.Count)];
        }
    }
}