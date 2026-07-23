using RYU.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace RYU.UI
{
    /// <summary>
    /// 틱이 도는 걸 눈으로 보게 해주는 막대.
    /// 한 틱 동안 왼쪽에서 오른쪽으로 차고, 틱이 넘어가는 순간 번쩍인다.
    /// 스택이 비어 스캔선이 숨어 있을 때도 계속 돈다. 시계가 안 멈춘다는 걸 보여준다.
    /// </summary>
    public class TickBar : MonoBehaviour
    {
        [SerializeField] private MemoryScanner scanner;

        [Header("Layout")]
        [SerializeField] private Vector2 size = new Vector2(420f, 18f);
        [Tooltip("화면 위쪽 가운데에서 얼마나 떨어질지.")]
        [SerializeField] private Vector2 offset = new Vector2(0f, -40f);

        [Header("Colors")]
        [SerializeField] private Color backgroundColor = new Color(0.12f, 0.12f, 0.15f, 0.85f);
        [SerializeField] private Color fillColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private Color beatColor = Color.white;
        [Tooltip("틱이 넘어가는 순간 번쩍이는 시간(초).")]
        [SerializeField, Min(0.01f)] private float beatFlash = 0.12f;

        private RectTransform _fillRect;
        private Image _fill;
        private Image _background;

        private float _lastFraction;
        private float _flashTimer;

        private void Awake()
        {
            if (scanner == null)
                scanner = FindAnyObjectByType<MemoryScanner>();

            Build();
        }

        private void LateUpdate()
        {
            if (scanner == null || _fillRect == null)
                return;

            float fraction = Mathf.Clamp01(scanner.Fraction);

            // 되감겼으면 틱이 하나 넘어간 것이다.
            if (fraction < _lastFraction)
                _flashTimer = beatFlash;

            _lastFraction = fraction;

            if (_flashTimer > 0f)
                _flashTimer -= Time.deltaTime;

            _fillRect.sizeDelta = new Vector2(size.x * fraction, size.y);

            float flash = Mathf.Clamp01(_flashTimer / beatFlash);
            _fill.color = Color.Lerp(fillColor, beatColor, flash);
        }

        private void Build()
        {
            // 이 오브젝트가 RectTransform이 아닐 수도 있어서 캔버스에 직접 붙인다.
            Canvas canvas = GetComponentInParent<Canvas>();
            Transform parent = canvas != null ? canvas.transform : transform;

            var background = new GameObject("TickBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            background.transform.SetParent(parent, false);

            var rect = background.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            _background = background.GetComponent<Image>();
            _background.color = backgroundColor;
            _background.raycastTarget = false;

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            fill.transform.SetParent(rect, false);

            _fillRect = fill.GetComponent<RectTransform>();

            // 왼쪽에 붙여두고 가로 길이만 늘려서 채운다. 스프라이트가 없어도 확실히 동작한다.
            _fillRect.anchorMin = new Vector2(0f, 0.5f);
            _fillRect.anchorMax = new Vector2(0f, 0.5f);
            _fillRect.pivot = new Vector2(0f, 0.5f);
            _fillRect.anchoredPosition = Vector2.zero;
            _fillRect.sizeDelta = new Vector2(0f, size.y);

            _fill = fill.GetComponent<Image>();
            _fill.color = fillColor;
            _fill.raycastTarget = false;
        }
    }
}
