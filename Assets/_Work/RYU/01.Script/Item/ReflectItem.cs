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
            Instantiate(turretPrefab, spawnPos, Quaternion.identity);
        }
    }
}
