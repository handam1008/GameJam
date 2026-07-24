using System.Collections;
using _Work.PAP.Scripts.Agent;
using _Work.PAP.Scripts.Player;
using csiimnida.CSILib.SoundManager.RunTime;
using RYU.Combat;
using Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private AgentAnimator animator;
        [SerializeField] private SoundSo targetSound;

        private float lastAttackTime;
        private PlayerController player;
        private Rigidbody2D _rb;
        private Rigidbody2D parentRb;
        private PlatformCarrier platformCarrier;
        private Vector2 moveVelocity;
        private bool canSee;
        private bool attacking = false;
        private bool initAttack = true;
        RaycastHit2D[] hits = new RaycastHit2D[1];

        public UnityEvent OnEffectEvent;
        public UnityEvent OnStartEvent;
        public UnityEvent OnEndEvent;


        private readonly int ATTACKING_HASHDATA = Animator.StringToHash("Attacking");

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
            parentRb = GameObject.FindGameObjectsWithTag("RotatePlatform")[0].GetComponent<Rigidbody2D>();
            platformCarrier = new PlatformCarrier(parentRb);
        }

        private void Update()
        {
            SeePlayer();
            AttackForward();
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = moveVelocity + platformCarrier.GetCarriedVelocity(transform.position);
        }

        private void AttackForward()
        {
            if ((player.transform.position - transform.position).magnitude < attackDistance && lastAttackTime < Time.time && canSee)
            {
                attacking = true;
                lastAttackTime = Time.time + attackCooldown;
                if (targetSound != null)
                    SoundManager.Instance.PlaySound(targetSound.soundName);
                animator.SetBool(ATTACKING_HASHDATA, true);
                animator.OnAnimationEvent += HandleDamageCast;
                animator.OnAnimationEndEvent += HandleAnimationEnd;
            }
        }

        private void HandleAnimationEnd()
        {
            animator.SetBool(ATTACKING_HASHDATA, false);
            attacking = false;
            animator.OnAnimationEndEvent -= HandleAnimationEnd;
        }

        private void HandleDamageCast()
        {
            OnEffectEvent?.Invoke();
            hits = new RaycastHit2D[1];
            animator.OnAnimationEvent -= HandleDamageCast;
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

        private void SeePlayer()
        {
            if (!canSee) return;
            Vector2 direction = player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), lerpScale);
            if (!(direction.magnitude < attackDistance) && !attacking)
            {
                if (!initAttack)
                {
                    initAttack = true;
                    Debug.Log("A");
                    OnEndEvent?.Invoke();
                }
                OnEndEvent?.Invoke();
                Vector2 dir = (player.transform.position - transform.position).normalized;
                moveVelocity = dir * speed;
            }
            else
            {
                if (initAttack)
                {
                    initAttack = false;
                    OnStartEvent?.Invoke();
                }
                moveVelocity = Vector2.zero;
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