using _Work.RYU._01.Script.FeedBack;
using RYU.Combat;
using UnityEngine;

// 낙하산으로 내려온 보급 상자. 총알을 한 발 맞으면 아이템을 떨구고 부서진다.
// 총알이 벽처럼 인식하도록 wallLayer에 두고, IDamageable이라 맞으면 TakeDamage가 불린다.
public class SupplyBox : MonoBehaviour, IDamageable
{
    // 부서지면 나올 아이템 후보들 (ItemPickup 붙은 것). 이 중 하나가 랜덤으로 나온다
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private FeedBackPlayer feedBackPlayer;

    private bool broken;

    public void TakeDamage(Vector3 dir)
    {
        // 한 발이면 부서진다. 여러 번 안 터지게 잠금
        if (broken)
            return;

        broken = true;

        // 후보 중 하나를 랜덤으로 뽑아 떨군다
        if (itemPrefabs != null && itemPrefabs.Length > 0)
        {
            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            if (prefab != null)
                Instantiate(prefab, transform.position, Quaternion.identity);
        }

        feedBackPlayer.PlayAllFeedBack();
        // 부서지는 연출은 나중에 여기 붙이면 된다
        Destroy(transform.parent.gameObject);
    }
}
