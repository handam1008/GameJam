using UnityEngine;

public abstract class AbstractItem : ScriptableObject
{
    public Sprite icon;
    public string description;
    
    
    // true면 QE로 발동 안 되고, 줍는 즉시 Use가 불린다 (분노 같은 패시브)
    public virtual bool IsPassive => false;

    public abstract void Use(GameObject target);

    // 슬롯 UI에 채워질 효과 진행도 0~1 (1=효과 꽉 참, 0=효과 없음/끝).
    // 쿨타임/지속시간 있는 아이템이 오버라이드한다. 기본은 0 (표시 안 함)
    public virtual float CooldownRatio01(GameObject user) => 0f;

    // true면 Use해도 슬롯에서 안 사라진다. 순간이동처럼 효과 끝날 때까지 슬롯에 남을 때
    public virtual bool KeepAfterUse(GameObject user) => false;

    // KeepAfterUse 아이템의 효과가 완전히 끝났는지. true면 슬롯에서 사라진다.
    // 진행도 0으로 판단하면 시작 순간(0)에 바로 사라지니 이걸로 따로 판단한다
    public virtual bool IsFinished(GameObject user) => true;
}
