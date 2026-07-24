using UnityEngine;

// 바닥에 떨어진 무기. 플레이어가 닿으면 장착된다.
public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private AbstractWeapon weapon;

    
    [SerializeField] private float lifeTime = 0f;

    private void Start()
    {
        if (lifeTime > 0f)
            Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        WeaponHolder holder = other.GetComponentInParent<WeaponHolder>();
        if (holder == null && other.attachedRigidbody != null)
            holder = other.attachedRigidbody.GetComponentInChildren<WeaponHolder>();

        if (holder == null)
            return;

        holder.Equip(weapon);
        Destroy(gameObject);
    }
}
