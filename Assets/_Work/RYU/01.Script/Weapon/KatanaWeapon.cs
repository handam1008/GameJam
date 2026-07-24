using UnityEngine;

// 카타나 제로 스타일 무기. 좌클릭하면 마우스 방향 앞의 총알을 온 반대로 튕겨낸다.
// 클릭형이라 쿨타임·횟수는 WeaponHolder가 관리한다.
[CreateAssetMenu(fileName = "RYU", menuName = "RYU/Weapon/Katana")]
public class KatanaWeapon : AbstractWeapon
{
    // 튕기는 감지 반경 (플레이어 앞 이 범위 안의 총알)
    [SerializeField] private float slashRange = 2f;
    // 플레이어 앞으로 얼마나 떨어진 곳을 벨지
    [SerializeField] private float slashOffset = 1f;
    // 정면 각도(도). 180이면 앞쪽 절반. 좁힐수록 마우스 방향만
    [SerializeField] private float slashAngle = 120f;

    public override void Fire(GameObject user, Vector3 origin, Vector2 direction)
    {
        // TODO(애니메이션): 여기서 카타나 휘두르는 애니메이션 재생 (친구가 붙일 예정)
        // 예) user.GetComponentInChildren<Animator>()?.SetTrigger("Slash");

        // 마우스 방향 앞을 중심으로 총알을 찾는다
        Vector2 center = (Vector2)origin + direction * slashOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, slashRange);

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out Bullet bullet))
                continue;

            // 총알이 정면 각도 안에 있을 때만 튕긴다
            Vector2 toBullet = ((Vector2)hit.transform.position - (Vector2)origin).normalized;
            if (Vector2.Angle(direction, toBullet) > slashAngle * 0.5f)
                continue;

            // 온 방향 반대로 되돌려보낸다
            bullet.Reflect();

            // TODO(애니메이션): 총알 하나 튕길 때마다 이펙트 재생 (베기 이펙트 등)
        }
    }
}
