using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.RYU._01.Script.FeedBack
{
    /// <summary>
    /// 피격 순간 화면이 붉게 확 물들었다가 서서히 걷힌다.
    /// 덮개는 시작할 때 코드로 만들기 때문에 씬 세팅이 필요 없다.
    /// </summary>
    public class ScreenEffect : AbstractFeedBack
    {
        [Header("Look")]
        [SerializeField] private Color flashColor = new Color(0.75f, 0.05f, 0.05f);

        [Tooltip("확 물든 순간의 진하기. 1이면 화면이 완전히 가려진다.")]
        [SerializeField, Range(0f, 1f)] private float peakAlpha = 0.4f;

        [Tooltip("걷히는 데 걸리는 시간(초).")]
        [SerializeField, Min(0.01f)] private float fadeDuration = 0.35f;

        private Image _overlay;
        private Tween _fade;

        private void Awake()
        {
            Build();
        }

        public override void CreateFeedBack()
        {
            if (_overlay == null)
                return;

            _fade?.Kill();

            // 즉시 확 물들이고, 서서히 걷는다. 갈수록 빨리 빠지는 곡선이라 여운이 남는다.
            Color peak = flashColor;
            peak.a = peakAlpha;
            _overlay.color = peak;

            _fade = _overlay.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad);
        }

        public override void StopFeedBack()
        {
            if (_overlay == null)
                return;

            _fade?.Kill();

            Color clear = flashColor;
            clear.a = 0f;
            _overlay.color = clear;
        }

        /// <summary>화면 전체를 덮는 캔버스와 이미지를 만든다. 평소에는 완전히 투명하다.</summary>
        private void Build()
        {
            var canvasObject = new GameObject("ScreenEffectCanvas", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // 게임 화면과 UI 위에 깔리도록 앞쪽으로 뺀다.
            canvas.sortingOrder = 50;

            var imageObject = new GameObject("Overlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(canvasObject.transform, false);

            var rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            _overlay = imageObject.GetComponent<Image>();
            _overlay.raycastTarget = false;

            Color clear = flashColor;
            clear.a = 0f;
            _overlay.color = clear;
        }
    }
}
