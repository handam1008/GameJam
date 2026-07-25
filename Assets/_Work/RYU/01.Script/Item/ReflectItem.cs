using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/ReflectItem", order = 0)]
    public class ReflectItem : AbstractItem
    {
      
        [SerializeField] private GameObject turretPrefab;

      
        [SerializeField] private float forwardDistance = 1.5f;

        public override void Use(GameObject target)
        {
            if (turretPrefab == null)
                return;

            
            Vector3 spawnPos = target.transform.position + target.transform.right * forwardDistance;
            GameObject turret = Instantiate(turretPrefab, spawnPos, Quaternion.identity);

            // 맵(RotatePlatform)의 자식으로 붙여서 인형도 맵 따라 돌게 한다
            GameObject floor = GameObject.FindWithTag("RotatePlatform");
            if (floor != null)
                turret.transform.SetParent(floor.transform, true);
        }
    }
}
