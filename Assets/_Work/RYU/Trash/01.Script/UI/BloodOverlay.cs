using RYU.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace RYU.UI
{
    /// <summary>
    /// 오버플로가 이어지는 동안 화면 테두리부터 피가 차오른다.
    /// 죽음이 가까워질수록 시야 가운데까지 덮여 눈이 가려지는 느낌을 준다.
    /// 텍스처는 코드로 만들기 때문에 따로 이미지 리소스가 필요 없다.
    /// </summary>
    public class BloodOverlay : MonoBehaviour
    {
        [SerializeField] private MemoryController memory;

        [Header("Look")]
        [SerializeField] private Color bloodColor = new Color(0.55f, 0.02f, 0.03f);
        [Tooltip("죽기 직전 화면이 가려지는 최대 진하기.")]
        [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.95f;
        [Tooltip("가장자리에서 안쪽으로 번지는 정도. 클수록 빨리 시야를 덮는다.")]
        [SerializeField, Range(0f, 1f)] private float maxCoverage = 0.9f;

        [Header("Heartbeat")]
        [Tooltip("피가 두근거리는 속도.")]
        [SerializeField, Min(0f)] private float pulseSpeed = 6f;
        [Tooltip("두근거림의 폭.")]
        [SerializeField, Range(0f, 0.5f)] private float pulseAmount = 0.12f;

        [Header("Quality")]
        [Tooltip("그라데이션 해상도. 늘려도 부드러워질 뿐 성능 차이는 거의 없다.")]
        [SerializeField, Range(32, 256)] private int resolution = 128;

        private Image _image;
        private Texture2D _texture;
        private Color32[] _pixels;

        /// <summary>텍스처를 다시 만든 시점의 진행도. 조금씩 바뀔 때마다 새로 그리지 않으려고 둔다.</summary>
        private int _bakedStep = -1;

        private const int StepCount = 24;

        private void Awake()
        {
            if (memory == null)
                memory = FindAnyObjectByType<MemoryController>();

            BuildImage();
        }

        private void OnDestroy()
        {
            if (_texture != null)
                Destroy(_texture);
        }

        private void LateUpdate()
        {
            if (memory == null || _image == null)
                return;

            float progress = memory.IsDead ? 1f : memory.OverflowProgress;

            if (progress <= 0f)
            {
                _image.enabled = false;
                return;
            }

            _image.enabled = true;
            Bake(progress);

            // 진행도가 오를수록 두근거림이 강해진다.
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount * progress;

            Color color = bloodColor;
            color.a = Mathf.Clamp01(progress * maxAlpha * pulse);
            _image.color = color;
        }

        private void BuildImage()
        {
            var rect = gameObject.GetComponent<RectTransform>();
            if (rect == null)
                rect = gameObject.AddComponent<RectTransform>();

            // 화면 전체를 덮는다.
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            _texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            _pixels = new Color32[resolution * resolution];

            _image = gameObject.GetComponent<Image>();
            if (_image == null)
                _image = gameObject.AddComponent<Image>();

            _image.sprite = Sprite.Create(
                _texture,
                new Rect(0f, 0f, resolution, resolution),
                new Vector2(0.5f, 0.5f));
            _image.raycastTarget = false;
            _image.enabled = false;
        }

        /// <summary>
        /// 가운데는 비어 있고 가장자리로 갈수록 진해지는 그라데이션을 굽는다.
        /// 진행도가 오르면 비어 있는 가운데가 좁아져 시야가 덮인다.
        /// </summary>
        private void Bake(float progress)
        {
            int step = Mathf.Clamp(Mathf.RoundToInt(progress * StepCount), 0, StepCount);
            if (step == _bakedStep)
                return;

            _bakedStep = step;

            float coverage = (float)step / StepCount * maxCoverage;
            float innerRadius = Mathf.Lerp(1f, 0f, coverage);
            float center = (resolution - 1) * 0.5f;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float dx = (x - center) / center;
                    float dy = (y - center) / center;

                    // 모서리가 1이 되도록 정규화한다.
                    float distance = Mathf.Sqrt(dx * dx + dy * dy) / Mathf.Sqrt(2f);
                    float alpha = Mathf.SmoothStep(0f, 1f,
                        Mathf.InverseLerp(innerRadius, 1f, distance));

                    _pixels[y * resolution + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }

            _texture.SetPixels32(_pixels);
            _texture.Apply(false);
        }
    }
}
