using System.Collections;
using _Work.PAP.Scripts.Player;
using RYU.Combat;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    public class CombatEnemyAI : MonoBehaviour
    {
        [SerializeField] private float lerpScale = 0.1f;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float attackDistance = 1f;
        [SerializeField] private float attackOrigin = 2f;
        [SerializeField] private float attackRadius = 3f;
        [SerializeField] private float knockbackPower = 2f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private ContactFilter2D whatIsTarget;

        private float lastAttackTime;
        private PlayerController player;
        private Rigidbody2D _rb;
        private bool canSee;

        private void Awake()
        {

            _rb = GetComponent<Rigidbody2D>();
            StartCoroutine(CanSeeCoroutine());
        }

        private IEnumerator CanSeeCoroutine()
        {
            yield return new WaitForSeconds(2f);
            canSee = true;
        }

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            SeePlayer();
            AttackForward();
        }

        private void AttackForward()
        {
            if ((player.transform.position - transform.position).magnitude < attackDistance && lastAttackTime < Time.time && canSee)
            {
                lastAttackTime = Time.time + attackCooldown;
                RaycastHit2D[] hits = new RaycastHit2D[1];
                Physics2D.CircleCast(transform.position + transform.right * attackOrigin,
                    attackRadius, Vector2.zero, whatIsTarget, hits);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null && hit.collider.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage((hit.collider.transform.position - transform.position).normalized * knockbackPower);
                    }
                }
            }
        }

        private void SeePlayer()
        {
            if (!canSee) return;
            Vector2 direction = player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), lerpScale);
            if (!(direction.magnitude < attackDistance))
            {
                Vector2 dir = (player.transform.position - transform.position).normalized;
                _rb.linearVelocity = dir * speed;
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.right * attackOrigin,
                attackRadius);
        }
    }
}