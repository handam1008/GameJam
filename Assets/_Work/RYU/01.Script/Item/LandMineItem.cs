using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/LandMineItem", order = 0)]
    public class LandMineItem : AbstractItem
    {
        // 설치할 지뢰 프리팹
        [SerializeField] private GameObject minePrefab;

        // 플레이어 앞으로 얼마나 떨어뜨려 설치할지
        [SerializeField] private float forwardDistance = 1.5f;

        public override void Use(GameObject target)
        {
            if (minePrefab == null)
                return;
            
            Vector3 spawnPos = target.transform.position + target.transform.right * forwardDistance;
            Instantiate(minePrefab, spawnPos, Quaternion.identity);
        }
    }
}