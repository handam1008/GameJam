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
}
