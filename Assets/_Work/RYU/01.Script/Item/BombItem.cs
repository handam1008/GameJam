using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    // 폭격 아이템. 사용하면 씬의 폭격 비행기가 랜덤 지점들을 연속 폭격한다.
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/BombItem", order = 0)]
    public class BombItem : AbstractItem
    {
        public override void Use(GameObject target)
        {
            // 씬에 미리 배치된 폭격 비행기를 찾아 폭격을 시작시킨다
            BombPlaneMarker bombPlane = Object.FindAnyObjectByType<BombPlaneMarker>();
            if (bombPlane != null)
                bombPlane.StartBombing();
        }
    }
}
