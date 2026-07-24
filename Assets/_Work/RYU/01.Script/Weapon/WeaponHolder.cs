using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Transform firePoint;

    private AbstractWeapon weapon;
    private float cooldownTimer;
    private int usesLeft;

    public AbstractWeapon Weapon => weapon;
    public int UsesLeft => usesLeft;
    public bool HasWeapon => weapon != null;
    
    public event Action OnChanged;

    private void Awake()
    {
        if (firePoint == null)
            firePoint = transform;
    }
    
    public void Equip(AbstractWeapon newWeapon)
    {
        // 기존 무기를 벗긴다 (방패면 ShieldBlock 꺼짐)
        if (weapon != null)
            weapon.OnUnequip(gameObject);

        weapon = newWeapon;
        usesLeft = newWeapon.maxUses;
        cooldownTimer = 0f;

        // 새 무기 장착 (방패면 ShieldBlock 켜짐)
        weapon.OnEquip(gameObject);
        OnChanged?.Invoke();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (weapon == null)
            return;
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TryFire();
    }

    private void TryFire()
    {
      
        if (cooldownTimer > 0f)
            return;
        
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = ((Vector2)(mouse - firePoint.position)).normalized;

        // 이 Holder가 붙은 오브젝트(플레이어)를 user로 넘긴다
        weapon.Fire(gameObject, firePoint.position, dir);

        cooldownTimer = weapon.cooldown;
        usesLeft--;
        OnChanged?.Invoke();
        
        if (usesLeft <= 0)
        {
            weapon = null;
            OnChanged?.Invoke();
        }
    }
}
