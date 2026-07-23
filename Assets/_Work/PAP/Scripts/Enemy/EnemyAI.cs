using System;
using _Work.PAP.Scripts.Player;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private GameObject bullet;
        [SerializeField] private float cooldown = 1;
        private float lastFireTime;
        private PlayerController player;
        

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
            lastFireTime = Time.time + cooldown;
            Vector2 dir = (player.transform.position - transform.position).normalized;
            GameObject bulletInstance = Instantiate(bullet, transform.position, Quaternion.identity);
            bulletInstance.GetComponent<Bullet>().Init(dir);
        }
    }
}