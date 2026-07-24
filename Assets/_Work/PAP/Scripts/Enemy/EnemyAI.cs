using System;
using System.Collections.Generic;
using _Work.PAP.Scripts.Player;
using csiimnida.CSILib.SoundManager.RunTime;
using Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.PAP.Scripts.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private List<EnemyAttackPatternSO> patterns;
        [SerializeField] private float lerpScale = 0.1f;
        [SerializeField] private Transform gunTrm;
        [SerializeField] private float kickDistance = 0.1f;
        [SerializeField] private float gunRecovery = 12f;
        
        private float lastFireTime;
        private PlayerController player;
        private EnemyAttackPatternSO currentPattern;
        private Rigidbody2D _rb;
        private Rigidbody2D parentRb;
        private PlatformCarrier platformCarrier;
        private Vector2 moveVelocity;
        private float lastPatrolTime;
        private Vector3 _originalLocalPosition;
        private float _originalLocalAngleZ;
        private bool canSee;

        private void Awake()
        {

            _rb = GetComponent<Rigidbody2D>();
            currentPattern = patterns[Random.Range(0, patterns.Count)];
            lastPatrolTime = Time.time + 1.5f;
            lastFireTime = lastPatrolTime + 0.5f;
            _originalLocalPosition = gunTrm.localPosition;
            _originalLocalAngleZ = gunTrm.localEulerAngles.z;
        }

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();
            parentRb = GameObject.FindGameObjectsWithTag("RotatePlatform")[0].GetComponent<Rigidbody2D>();
            platformCarrier = new PlatformCarrier(parentRb);
        }

        private void Update()
        {
            SeePlayer();
            GunKickBack();
            SpawnBullet();
            if (Time.time > lastPatrolTime)
            {
                canSee = true;
                lastPatrolTime = Time.time + Random.Range(0, 1.5f);
                moveVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = moveVelocity + platformCarrier.GetCarriedVelocity(transform.position);
        }

        private void SeePlayer()
        {
            if (!canSee) return;
            Vector2 direction = player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), lerpScale);
        }

        private void GunKickBack()
        {
            gunTrm.localPosition = Vector3.Lerp(
                gunTrm.localPosition, _originalLocalPosition,gunRecovery * Time.deltaTime);
            
            float currentZ = gunTrm.localEulerAngles.z;
            float targetZ = Mathf.LerpAngle(currentZ, _originalLocalAngleZ, gunRecovery * Time.deltaTime);
            gunTrm.localEulerAngles = new Vector3(0, 0, targetZ);
        }

        private void SpawnBullet()
        {
            if (Time.time < lastFireTime) return;
            FireAnimation();
            SoundManager.Instance.PlaySound(currentPattern.soundEffect.soundName);
            lastFireTime = Time.time + currentPattern.cooldown;
            Vector2 baseDir = (player.transform.position + new Vector3(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f),0) - transform.position).normalized;
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

        private void FireAnimation()
        {
            gunTrm.localPosition -= new Vector3(kickDistance, 0, 0);
        }
    }
}