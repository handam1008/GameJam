using Systems;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/RemoveBulletItem", order = 0)]
    public class RemoveBulletItem : AbstractItem
    {
        public override void Use(GameObject target)
        {
            // 화면에 있는 모든 총알을 찾아 풀에 반납한다. Destroy가 아니라 Push여야 풀이 안 깨진다
            Bullet[] bullets = Object.FindObjectsByType<Bullet>(FindObjectsSortMode.None);

            foreach (Bullet bullet in bullets)
                PoolManager.Instance.Push(bullet);
        }
    }
}
