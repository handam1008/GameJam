using DG.Tweening;
using UnityEngine;

namespace _Work.RYU._01.Script.Player
{
    public class ShieldMode : TimedMode
    {
        // 내가 만든 실드 오브젝트 (파란 막). 여기에 끌어다 넣는다
        [SerializeField] private GameObject shieldVisual;

        // 막을 때 반짝이는 세기 (0.2 = 20% 커졌다 돌아옴)
        [SerializeField] private float blinkScale = 0.2f;

        // 실드가 켜져 있으면 피해를 막는다
        public bool IsShielded => IsActive;

        private Vector3 normalScale;

        private void Awake()
        {
            if (shieldVisual == null)
                return;

            normalScale = shieldVisual.transform.localScale;
            shieldVisual.SetActive(false);
        }

        protected override void OnEnter()
        {
            if (shieldVisual != null)
                shieldVisual.SetActive(true);
        }

        protected override void OnExit()
        {
            if (shieldVisual != null)
                shieldVisual.SetActive(false);
        }

        // 피해를 막은 순간 실드가 반짝한다. PlayerHealth가 불러준다
        public void Blink()
        {
            if (shieldVisual == null)
                return;

            shieldVisual.transform.DOComplete();
            shieldVisual.transform.DOPunchScale(normalScale * blinkScale, 0.25f, 6, 0.5f)
                .SetLink(shieldVisual);
        }
    }
}
