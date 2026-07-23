using RYU.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace RYU.UI
{
    /// <summary>
    /// 죽는 순간 아무 연출 없이 곧바로 뜨는 블루스크린.
    /// 화면은 코드로 만들기 때문에 따로 프리팹이 필요 없다.
    /// </summary>
    public class BlueScreen : MonoBehaviour
    {
        [SerializeField] private MemoryController memory;
        [SerializeField] private Font font;

        [Header("Look")]
        [SerializeField] private Color backgroundColor = new Color(0f, 0.32f, 0.63f);
        [SerializeField] private Color textColor = Color.white;

        [Header("Progress")]
        [Tooltip("0%에서 100%까지 차는 데 걸리는 시간(초).")]
        [SerializeField, Min(0.1f)] private float progressDuration = 8f;

        [Header("Text")]
        [SerializeField, TextArea(2, 4)]
        private string message =
            "장치에 문제가 발생하여 다시 시작해야 합니다. 일부 오류 정보를 수집하고 있습니다. 그런 다음 자동으로 다시 시작합니다.";

        [SerializeField] private string stopCode = "중지 코드: MEMORY_OVERFLOW";

        private GameObject _root;
        private Text _progressText;
        private float _elapsed;
        private bool _shown;

        private void Awake()
        {
            if (memory == null)
                memory = FindAnyObjectByType<MemoryController>();

            Build();
            _root.SetActive(false);
        }

        private void OnEnable()
        {
            if (memory != null)
                memory.OnDeath += Show;
        }

        private void OnDisable()
        {
            if (memory != null)
                memory.OnDeath -= Show;
        }

        private void Update()
        {
            if (!_shown)
                return;

            _elapsed += Time.deltaTime;
            int percent = Mathf.FloorToInt(Mathf.Clamp01(_elapsed / progressDuration) * 100f);
            _progressText.text = $"{percent}% 완료";
        }

        /// <summary>페이드 없이 그대로 띄운다.</summary>
        private void Show()
        {
            if (_shown)
                return;

            _shown = true;
            _elapsed = 0f;
            _root.SetActive(true);

            // 금 연출 위에 확실히 덮이도록 맨 앞으로 보낸다.
            _root.transform.SetAsLastSibling();
        }

        private void Build()
        {
            _root = CreatePanel();

            CreateText(":(", 170, new Vector2(220f, -150f), new Vector2(500f, 260f));
            CreateText(message, 46, new Vector2(220f, -430f), new Vector2(1520f, 160f));
            _progressText = CreateText("0% 완료", 46, new Vector2(220f, -610f), new Vector2(600f, 70f));

            CreateText(stopCode, 26, new Vector2(420f, -760f), new Vector2(1200f, 50f));
        }

        private GameObject CreatePanel()
        {
            var panel = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            // 이 오브젝트가 RectTransform이 아닐 수도 있어서 캔버스에 직접 붙인다.
            // 부모가 RectTransform이 아니면 화면 전체로 늘어나지 못한다.
            Canvas canvas = GetComponentInParent<Canvas>();
            panel.transform.SetParent(canvas != null ? canvas.transform : transform, false);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = panel.GetComponent<Image>();
            image.color = backgroundColor;
            image.raycastTarget = false;

            return panel;
        }

        /// <summary>왼쪽 위를 기준으로 글자를 놓는다. position의 y는 아래로 내려갈수록 음수.</summary>
        private Text CreateText(string content, int size, Vector2 position, Vector2 boxSize)
        {
            var item = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            item.transform.SetParent(_root.transform, false);

            var rect = item.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = boxSize;

            var text = item.GetComponent<Text>();
            text.text = content;
            text.font = font;
            text.fontSize = size;
            text.color = textColor;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            return text;
        }
    }
}
