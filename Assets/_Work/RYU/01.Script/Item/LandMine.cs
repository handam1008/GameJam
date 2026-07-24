using RYU.Combat;
using UnityEngine;


public class LandMine : MonoBehaviour
{
    [Header("페이드")]
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private float minAlpha = 0.1f;

    [Header("폭발")]
    [SerializeField] private float triggerRadius = 0.5f;
    [SerializeField] private float explodeRadius = 2.5f;
    // 밟거나 피해받는 대상 레이어 (Agent = 플레이어+적)
    [SerializeField] private LayerMask targetLayer;

    // 터질 때 그 자리에 소환할 폭발 연출 프리팹 (파티클+사운드 등). 지뢰가 사라져도 얘는 남는다
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private float armDelay = 0.5f;
    private float armTimer;

    private SpriteRenderer sprite;
    private float t;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Fade();
        
        if (armTimer < armDelay)
        {
            armTimer += Time.deltaTime;
            return;
        }

        CheckTrigger();
    }

    private void Fade()
    {
        if (t >= 1f)
            return;

        t += Time.deltaTime / fadeTime;
        float alpha = Mathf.Lerp(1f, minAlpha, Mathf.Clamp01(t));

        Color c = sprite.color;
        c.a = alpha;
        sprite.color = c;
    }

    private void CheckTrigger()
    {
        // targetLayer(Agent)에 있는 것만 감지한다. 플레이어든 적이든 밟으면 터진다
        Collider2D hit = Physics2D.OverlapCircle(transform.position, triggerRadius, targetLayer);

        if (hit != null)
            Explode();
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius, targetLayer);

        Debug.Log($"[지뢰] 폭발! 반경 안 대상 {hits.Length}개", this);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable victim))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                victim.TakeDamage(dir);
                Debug.Log($"[지뢰] {hit.name} 체력 깎음", hit);
            }
            else
            {
                Debug.Log($"[지뢰] {hit.name} 은 IDamageable 없음 (체력 안 깎임)", hit);
            }
        }

        // 폭발 연출을 그 자리에 따로 소환한다. 지뢰는 사라져도 연출은 남아 재생된다
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
