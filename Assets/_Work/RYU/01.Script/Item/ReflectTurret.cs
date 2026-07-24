using System.Collections.Generic;
using RYU.Combat;
using UnityEngine;

// 소환된 자리에 고정되는 반사 포탑.
// 반경 안에 들어온 총알을 온 반대로 되돌리고, 정해진 횟수를 채우면 광역 폭발하며 사라진다.
public class ReflectTurret : MonoBehaviour
{
    [Header("반사")]
    // 이 반경 안의 총알을 되돌린다
    [SerializeField] private float reflectRadius = 1f;
    // 몇 번 되돌리면 터질지
    [SerializeField] private int maxReflects = 20;
    // 같은 총알을 이 시간 안에는 다시 튕기지 않는다. 벽에 맞고 돌아와도 재카운트 방지
    [SerializeField] private float sameBulletCooldown = 0.3f;

    [Header("폭발")]
    [SerializeField] private float explodeRadius = 3f;
    [SerializeField] private string enemyTag = "Enemy";

    private int reflectCount;

    // 최근에 튕긴 총알과 그 시각. 쿨다운 안이면 다시 안 튕긴다
    private readonly Dictionary<Bullet, float> lastReflectTime = new Dictionary<Bullet, float>();

    private void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, reflectRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out Bullet bullet))
                continue;

            // 방금 튕긴 총알이면 쿨다운 동안 건너뛴다
            if (lastReflectTime.TryGetValue(bullet, out float t) && Time.time - t < sameBulletCooldown)
                continue;

            bullet.Reflect();
            lastReflectTime[bullet] = Time.time;
            reflectCount++;
            Debug.Log($"[Turret] 튕김 {reflectCount}/{maxReflects} ({bullet.name})", this);

            if (reflectCount >= maxReflects)
            {
                Explode();
                return;
            }
        }
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(enemyTag))
                continue;

            if (hit.TryGetComponent(out IDamageable enemy))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                enemy.TakeDamage(dir);
            }
        }

        // 폭발 연출은 나중에 여기 붙이면 된다
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, reflectRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
