using UnityEngine;

namespace _Work.RYU._01.Script.Item
{
    [CreateAssetMenu(fileName = "RYU", menuName = "RYU/Item/TeleportItem", order = 0)]
    public class TeleportItem : AbstractItem
    {
        // 남길 표식 프리팹
        [SerializeField] private GameObject markPrefab;

        // 표식으로 돌아가기까지 시간(초)
        [SerializeField] private float cooldown = 3f;

        // 지금 살아있는 표식. 슬롯 진행도를 여기서 읽는다
        private TeleportMark activeMark;

        // 사용하면 플레이어 자리에 표식을 남긴다. 월드 고정이라 부모 안 붙임
        public override void Use(GameObject target)
        {
            if (markPrefab == null)
                return;

            GameObject mark = Instantiate(markPrefab, target.transform.position, Quaternion.identity);
            if (mark.TryGetComponent(out activeMark))
                activeMark.Init(target, cooldown);
        }

        // 쓴 뒤 귀환할 때까지 슬롯에 남는다
        public override bool KeepAfterUse(GameObject user) => true;

        // 남은 시간 비율. 처음 1(꽉 참)에서 0(귀환)으로 줄어든다
        public override float CooldownRatio01(GameObject user)
            => activeMark != null ? 1f - activeMark.Progress : 0f;

        // 표식이 사라졌으면(귀환 완료) 효과 끝. 그때 슬롯 비운다
        public override bool IsFinished(GameObject user)
            => activeMark == null;
    }
}
