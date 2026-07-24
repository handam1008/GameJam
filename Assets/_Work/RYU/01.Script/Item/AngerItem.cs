using _Work.RYU._01.Script.Player;
using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/AngerItem", order = 0)]
    public class AngerItem : AbstractItem
    {
        // 발동 중(체력 1)일 때 보여줄 빨간 아이콘
        public Sprite rageIcon;

        // 패시브라 QE로 발동되면 안 된다. 주우면 바로 장착만 된다
        public override bool IsPassive => true;

        // 줍는 즉시 LastStand를 켠다. 체력 1이 되면 자동으로 속도 증가
        public override void Use(GameObject target)
        {
            if (target.TryGetComponent(out LastStand lastStand))
                lastStand.Equip();
        }
    }
}
