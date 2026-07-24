using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    // 이 아이템이 담고 있는 효과
    [SerializeField] private AbstractItem item;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Pickup] 트리거 진입: {other.name}", this);

        // 부딪힌 콜라이더 기준으로 위(부모)와 아래(자식) 양쪽에서 인벤토리를 찾는다.
        // 콜라이더와 ItemInventory가 다른 오브젝트에 있어도 걸린다
        ItemInventory inventory = other.GetComponentInParent<ItemInventory>();
        if (inventory == null && other.attachedRigidbody != null)
            inventory = other.attachedRigidbody.GetComponentInChildren<ItemInventory>();

        if (inventory == null)
        {
            Debug.Log("[Pickup] 인벤토리 못 찾음 (플레이어 아님)", this);
            return;
        }

        // 슬롯이 꽉 차 있으면 줍지 않고 바닥에 남는다
        if (!inventory.TryPickUp(item))
        {
            Debug.Log("[Pickup] 슬롯 꽉 참", this);
            return;
        }

        Debug.Log("[Pickup] 주움 성공", this);
        Destroy(gameObject);
    }
}
