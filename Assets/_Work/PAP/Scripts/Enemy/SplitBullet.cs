using _Work.RYU._01.Script.FeedBack;
using RYU.Combat;
using Systems;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    public class SplitBullet : Bullet
    {
        [SerializeField] FeedBackPlayer feedbackPlayer;
        [SerializeField] private PoolItemSO bullet;
        protected override void Bounce(Vector2 normal, Collider2D wall, Vector2 contactPoint)
        {
            feedbackPlayer.PlayAllFeedBack();
            _impulseSource.GenerateImpulseWithVelocity(velocity * speed/2500f);
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

            Vector2 baseDir = normal;
            
            float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

            int count = 3;
            float degree = 45;

            float step = count > 1 ? degree / (count - 1) : 0f;
            float startAngle = baseAngle - degree * 0.5f;
            for (int i = 0; i < count; i++)
            {
                float angle = startAngle + step * i;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                
                IPoolable bullet = PoolManager.Instance.Pop(this.bullet.ItemName);
                bullet.GameObject.transform.position = transform.position + (Vector3)(dir * 2f);
                bullet.GameObject.GetComponent<Bullet>().Init(dir);
            }
            PoolManager.Instance.Push(this);
        }
    }
}