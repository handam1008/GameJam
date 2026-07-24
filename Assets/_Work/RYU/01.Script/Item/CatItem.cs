using Systems;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    // 고양이 아이템. 사용하면 플레이어 주변 여러 방향으로 총알을 뿌린다.
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/CatItem", order = 0)]
    public class CatItem : AbstractItem
    {
        // 발사할 총알 (풀에 등록된 것)
        [SerializeField] private PoolItemSO bullet;

        // 몇 방향으로 쏠지
        [SerializeField] private int bulletCount = 10;

        // 플레이어에서 총알이 나오는 거리
        [SerializeField] private float spawnRadius = 1f;

        public override void Use(GameObject target)
        {
            if (bullet == null)
                return;

            float step = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = step * i * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                IPoolable pooled = PoolManager.Instance.Pop(bullet.ItemName);
                pooled.GameObject.transform.position = target.transform.position + (Vector3)(dir * spawnRadius);
                pooled.GameObject.GetComponent<Bullet>().Init(dir);
            }
        }
    }
}
