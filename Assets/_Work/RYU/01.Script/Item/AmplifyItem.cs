using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    // 증폭 아이템. 슬롯에 있으면 반대편 슬롯 아이템을 쓸 때 2번 발동시킨다.
    // 자기 자신은 QE로 발동 안 되고, 반대편을 증폭하면 사라진다.
    // 실제 증폭 처리는 ItemInventory가 한다. 이건 표식 역할.
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/AmplifyItem", order = 0)]
    public class AmplifyItem : AbstractItem
    {
        public override void Use(GameObject target)
        {
            // 스스로는 아무것도 안 한다 (증폭은 ItemInventory가 처리)
        }
    }
}
