using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    [SerializeField] private AbstractItem item;
    [SerializeField] private float lifeTime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemInventory inventory = other.GetComponentInParent<ItemInventory>();
        if (inventory == null && other.attachedRigidbody != null)
            inventory = other.attachedRigidbody.GetComponentInChildren<ItemInventory>();

        if (inventory == null)
            return;

       
        if (!inventory.TryPickUp(item))
            return;

        Destroy(gameObject);
    }
}
