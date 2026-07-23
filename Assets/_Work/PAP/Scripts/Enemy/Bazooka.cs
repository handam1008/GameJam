using _Work.RYU._01.Script.FeedBack;
using RYU.Combat;
using Systems;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    public class Bazooka : Bullet
    {
        private RaycastHit2D[] hits;
       [SerializeField] private ContactFilter2D whatIsTarget;
       [SerializeField] private FeedBackPlayer feedBackPlayer;

       protected override void Awake()
       {
           base.Awake();
           hits = new RaycastHit2D[1];
       }
        protected override void Bounce(Vector2 normal, Collider2D wall, Vector2 contactPoint)
        {
            feedBackPlayer.PlayAllFeedBack();
            _impulseSource.GenerateImpulseWithVelocity(velocity * speed/50f);
            WallHitEffect effect = wall.GetComponentInParent<WallHitEffect>();
            if (effect != null)
            {
                effect.ShowHit(transform.position);
            }
            if (wall.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(velocity);
                PoolManager.Instance.Push(this);
                return;
            }
            hits = new RaycastHit2D[1];
            Physics2D.CircleCast(contactPoint, 2, Vector2.zero,whatIsTarget, hits,0f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.TryGetComponent(out IDamageable damageable1))
                {
                    damageable1.TakeDamage(velocity);
                }
            }
            PoolManager.Instance.Push(this);
            return;
        }
    }
}