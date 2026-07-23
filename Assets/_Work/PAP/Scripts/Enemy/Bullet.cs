using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private LayerMask wallLayer;

        [SerializeField] private int bounceCount = 2;
        private int currentBounces = 0;
        Vector2 dir;

        public void Init(Vector2 direction)
        {
            dir = direction.normalized;
            SetRotation();
        }

        void Update()
        {
            float move = speed * Time.deltaTime;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, move + 0.1f, wallLayer);
            if (hit.collider != null)
            {
                dir = Vector2.Reflect(dir, hit.normal);
                SetRotation();
                if (++currentBounces > bounceCount)
                {
                    Destroy(gameObject);
                    //잘하면 풀까지
                }
            }

            transform.position += (Vector3)(dir * move);
        }

        void SetRotation()
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}