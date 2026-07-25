using DG.Tweening;
using UnityEngine;

// UI를 흔드는 재사용 컴포넌트. 흔들 RectTransform에 붙인다.
// Pulse() = 아이템 사용처럼 1회, SetShaking(true) = 방패처럼 계속.
[RequireComponent(typeof(RectTransform))]
public class UIShake : MonoBehaviour
{
    // 흔들 세기(픽셀)
    [SerializeField] private float strength = 8f;
    // 1회 흔들기 지속 시간
    [SerializeField] private float duration = 0.25f;
    // 떨림 횟수. 클수록 촘촘하게 떤다
    [SerializeField] private int vibrato = 12;

    private RectTransform rect;
    private Vector2 basePos;
    private Tween tween;
    private bool shaking;   // 지속 흔들기 중인지

    private void Awake()
    {
        rect = (RectTransform)transform;
        basePos = rect.anchoredPosition;
    }

    // 1회 흔들기 (아이템 사용). 지속 흔들기 중이면 무시한다
    public void Pulse()
    {
        if (shaking)
            return;

        tween?.Kill();
        rect.anchoredPosition = basePos;
        // 끝에서 서서히 잦아들며 제자리로 (fadeOut = true)
        tween = rect.DOShakeAnchorPos(duration, strength, vibrato, 90f, false, true)
            .SetLink(gameObject)
            .OnComplete(() => rect.anchoredPosition = basePos);
    }

    // 지속 흔들기 on/off (방패 등 누르는 동안)
    public void SetShaking(bool on)
    {
        if (on == shaking)
            return;

        shaking = on;

        if (on)
        {
            tween?.Kill();
            rect.anchoredPosition = basePos;
            // 일정한 세기로 계속 떤다 (fadeOut = false + 무한 루프)
            tween = rect.DOShakeAnchorPos(duration, strength, vibrato, 90f, false, false)
                .SetLoops(-1)
                .SetLink(gameObject);
        }
        else
        {
            tween?.Kill();
            tween = null;
            rect.anchoredPosition = basePos;
        }
    }
}
