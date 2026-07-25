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

    // UI 네모박스 크기용. 남은 사용량 0~1 (방패=게이지, 카타나=횟수)
    public float RemainingRatio => weapon != null ? weapon.GetRemaining01(gameObject, usesLeft) : 0f;

    // 지금 무기를 사용 중인지(방패 홀드 등). UI 지속 흔들기용
    public bool IsWeaponInUse => weapon != null && weapon.IsInUse(gameObject);

    public event Action OnChanged;

    // 클릭형 무기가 발사되는 순간 발행. UI 1회 흔들기용
    public event Action OnFired;

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

        // 클릭형 무기만 좌클릭으로 발사. 방패는 스스로 입력받으니 건너뛴다
        if (!weapon.ManagesOwnInput &&
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TryFire();

        // 다 쓴 무기는 벗겨진다 (횟수 0 또는 방패 게이지 0)
        if (weapon.IsDepleted(gameObject, usesLeft))
            Unequip();
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
        OnFired?.Invoke();
        OnChanged?.Invoke();
    }

    private void Unequip()
    {
        weapon.OnUnequip(gameObject);
        weapon = null;
        OnChanged?.Invoke();
    }
}
