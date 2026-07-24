using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/ReflectItem", order = 0)]
    public class ReflectItem : AbstractItem
    {
        // 소환할 반사 포탑 프리팹
        [SerializeField] private GameObject turretPrefab;

        public override void Use(GameObject target)
        {
            if (turretPrefab == null)
                return;
            
            Instantiate(turretPrefab, target.transform.position, Quaternion.identity);
        }
    }
}
