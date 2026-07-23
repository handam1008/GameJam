using RYU.Memory;
using UnityEngine;

namespace RYU.Combat
{
    /// <summary>
    /// GC로 비운 가비지 양에 비례해 주변 적에게 광역 피해를 준다.
    /// </summary>
    [RequireComponent(typeof(MemoryController))]
    public class GarbageBlast : MonoBehaviour
    {
        [Header("Radius")]
        [SerializeField, Min(0f)] private float baseRadius = 1.5f;
        [SerializeField, Min(0f)] private float radiusPerGarbage = 0.3f;
        [SerializeField, Min(0f)] private float maxRadius = 8f;

        [Header("Damage")]
        [SerializeField, Min(0f)] private float damagePerGarbage = 5f;

        [Header("Target")]
        [SerializeField] private LayerMask targetLayers = ~0;

        private MemoryController _memory;
        private float _lastRadius;

        private void Awake()
        {
            _memory = GetComponent<MemoryController>();
        }

        private void OnEnable()
        {
            _memory.OnGarbageCollected += Explode;
        }

        private void OnDisable()
        {
            _memory.OnGarbageCollected -= Explode;
        }

        private void Explode(int garbageCount)
        {
            float radius = Mathf.Min(baseRadius + radiusPerGarbage * garbageCount, maxRadius);
            float damage = damagePerGarbage * garbageCount;
            _lastRadius = radius;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                // if (hits[i].TryGetComponent(out IDamageable target))
                    // target.TakeDamage(damage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _lastRadius > 0f ? _lastRadius : baseRadius);
        }
    }
}
