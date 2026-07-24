using System.Collections.Generic;
using CoreLib;
using DG.Tweening;
using RYU.Combat;
using UnityEngine;

namespace _Work.RYU._01.Script.Player
{
    public class GiantMode : TimedMode
    {
        [Header("커지기")]
        // 커질 대상 (캐릭터 그림). 비우면 이 오브젝트 자신
        [SerializeField] private Transform visual;
        [SerializeField] private float scale = 2.5f;
        [SerializeField] private float growTime = 0.3f;

        [Header("밟기")]
        [SerializeField] private float stompRadius = 1f;
        // 몇 초마다 한 번씩 쿵 밟을지
        [SerializeField] private float stompInterval = 0.5f;
        // 한 번 밟을 때 데미지. 0.5면 2번 밟아야 체력 1이 깎인다
        [SerializeField] private float stompDamage = 0.5f;
        [SerializeField] private string enemyTag = "Enemy";

        [Header("연출")]
        // 밟을 때 눌리는 세기 (0.15 = 15% 찌그러짐)
        [SerializeField] private float stompSquash = 0.15f;
        [SerializeField] private float shakeStrength = 0.3f;

        private Vector3 normalScale;
        private float stompTimer;

        // 적마다 쌓인 데미지. 1이 넘으면 그만큼 실제로 체력을 깎는다
        private readonly Dictionary<GameObject, float> damageDealt = new Dictionary<GameObject, float>();

        private void Awake()
        {
            if (visual == null)
                visual = transform;

            normalScale = visual.localScale;
        }

        protected override void OnEnter()
        {
            stompTimer = 0f;
            damageDealt.Clear();

            visual.DOKill();
            visual.localScale = normalScale;
            visual.DOScale(normalScale * scale, growTime).SetEase(Ease.OutBack).SetLink(visual.gameObject);
        }

        protected override void OnExit()
        {
            visual.DOKill();
            visual.DOScale(normalScale, growTime).SetEase(Ease.InBack).SetLink(visual.gameObject);
        }

        protected override void OnTick()
        {
            // 간격마다 한 번씩 밟는다
            stompTimer -= Time.deltaTime;
            if (stompTimer > 0f)
                return;

            stompTimer = stompInterval;
            Stomp();
        }

        private void Stomp()
        {
            // 물리 충돌이 꺼져 있어도 OverlapCircle은 반경 안의 콜라이더를 다 찾는다
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stompRadius);

            bool hitAny = false;
            foreach (Collider2D hit in hits)
            {
                if (DamageEnemy(hit.gameObject))
                    hitAny = true;
            }

            if (hitAny)
                PlayStompEffect();
        }

        // 적이면 데미지를 주고 true. 0.5씩 쌓다가 1이 넘으면 실제로 체력을 깎는다
        private bool DamageEnemy(GameObject target)
        {
            if (!target.CompareTag(enemyTag))
                return false;

            if (!target.TryGetComponent(out IDamageable enemy))
                return false;

            float total = (damageDealt.TryGetValue(target, out float prev) ? prev : 0f) + stompDamage;
            Vector3 dir = (target.transform.position - transform.position).normalized;

            while (total >= 1f)
            {
                enemy.TakeDamage(dir);
                total -= 1f;
            }

            damageDealt[target] = total;
            return true;
        }

        private void PlayStompEffect()
        {
            visual.DOComplete();
            visual.DOPunchScale(normalScale * scale * -stompSquash, stompInterval * 0.8f, 4, 0.5f)
                .SetLink(visual.gameObject);

            if (CameraEffectManager.Instance != null)
                CameraEffectManager.Instance.Shake(shakeStrength);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stompRadius);
        }
    }
}
