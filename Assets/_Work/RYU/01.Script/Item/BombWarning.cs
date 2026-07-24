using Combat.Effects;
using CoreLib;
using RYU.Combat;
using Systems;
using UnityEngine;

// 폭격 경고 지점. 잠깐 경고를 보여준 뒤 그 자리를 폭발시킨다.
// 비행기가 이걸 떨어뜨리면(SupplyPlane) 경고가 뜨고 warnTime 뒤 폭발한다.
public class BombWarning : MonoBehaviour
{
    // 경고 후 폭발까지 시간(초). 이 동안 피할 수 있다
    [SerializeField] private float warnTime = 1f;

    // 폭발 피해 반경
    [SerializeField] private float explodeRadius = 2.5f;

    // 피해 대상 레이어 (Agent = 적+플레이어)
    [SerializeField] private LayerMask targetLayer;

    // 폭발 순간 풀에서 꺼낼 연출 (친구 EnemyExplosion 등)
    [SerializeField] private PoolItemSO explosion;

    // 폭발 시 화면 흔들림 세기
    [SerializeField] private float shakeStrength = 0.4f;

    private float timer;

    private void Start()
    {
        timer = warnTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Explode();
    }

    private void Explode()
    {
        // 반경 안 대상(적+플레이어)에게 피해
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius, targetLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable victim))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                victim.TakeDamage(dir);
            }
        }

        // 폭발 연출을 풀에서 꺼내 이 자리에 놓는다
        if (explosion != null)
        {
            EffectPlayer fx = PoolManager.Instance.Pop(explosion.ItemName) as EffectPlayer;
            fx.SetPositionAndPlay(transform.position);
        }

        // 화면 흔들림
        if (CameraEffectManager.Instance != null)
            CameraEffectManager.Instance.Shake(shakeStrength);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
