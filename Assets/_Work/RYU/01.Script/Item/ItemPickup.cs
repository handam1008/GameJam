using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    // 이 아이템이 담고 있는 효과
    [SerializeField] private AbstractItem item;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemInventory inventory = other.GetComponentInParent<ItemInventory>();
        if (inventory == null)
            return;

        // 슬롯이 꽉 차 있으면 줍지 않고 바닥에 남는다
        if (!inventory.TryPickUp(item))
            return;

        Destroy(gameObject);
    }
}
