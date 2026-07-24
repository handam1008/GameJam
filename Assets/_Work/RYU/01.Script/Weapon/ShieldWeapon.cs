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

    // 방패는 좌클릭을 스스로 받는다. Holder가 발사 처리하면 안 됨
    public override bool ManagesOwnInput => true;

    // 남은 양은 게이지 비율
    public override float GetRemaining01(GameObject user, int usesLeft)
        => user.TryGetComponent(out ShieldBlock shield) ? shield.GaugeRatio : 0f;

    public override bool IsDepleted(GameObject user, int usesLeft)
        => user.TryGetComponent(out ShieldBlock shield) && shield.IsGaugeEmpty;
}
