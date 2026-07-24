using UnityEngine;

[CreateAssetMenu(fileName = "RYU", menuName = "RYU/Weapon/Shield")]
public class ShieldWeapon : AbstractWeapon
{
    // 홀드형이라 Fire는 안 쓴다. 장착되면 방패를 켜고, 벗겨지면 끈다
    public override void OnEquip(GameObject user)
    {
        if (user.TryGetComponent(out ShieldBlock shield))
            shield.SetEquipped(true);
    }

    public override void OnUnequip(GameObject user)
    {
        if (user.TryGetComponent(out ShieldBlock shield))
            shield.SetEquipped(false);
    }
}
