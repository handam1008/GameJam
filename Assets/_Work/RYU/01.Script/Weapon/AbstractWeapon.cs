using UnityEngine;


public abstract class AbstractWeapon : ScriptableObject
{
    public Sprite icon;
    public string weaponName;

   
    public float cooldown = 0.5f;

    
    public int maxUses = 10;

   
    // 클릭형 무기가 이걸로 발동한다. 총·카타나처럼 클릭 한 번 = 한 방.
    // 방패 같은 홀드형은 이걸 비워두고 OnEquip/OnUnequip을 쓴다.
    public virtual void Fire(GameObject user, Vector3 origin, Vector2 direction) { }

    // 장착될 때 한 번. 방패는 여기서 ShieldBlock을 켠다
    public virtual void OnEquip(GameObject user) { }

    // 벗겨질 때 한 번. 방패는 여기서 ShieldBlock을 끈다
    public virtual void OnUnequip(GameObject user) { }

    // true면 WeaponHolder가 좌클릭 발사를 안 한다. 방패처럼 스스로 입력받는 무기용
    public virtual bool ManagesOwnInput => false;

    // 남은 사용량 0~1. UI 네모박스 크기용. 기본은 남은횟수/최대횟수
    public virtual float GetRemaining01(GameObject user, int usesLeft)
        => maxUses > 0 ? (float)usesLeft / maxUses : 0f;

    // 다 썼는지. 기본은 횟수 0. 방패는 게이지 0
    public virtual bool IsDepleted(GameObject user, int usesLeft)
        => usesLeft <= 0;
}
