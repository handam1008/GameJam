using RYU.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace RYU.UI
{
    /// <summary>
    /// 메모리가 찰수록 화면 테두리에서 안쪽으로 금이 뻗는다.
    /// 꽉 찬 뒤에는 사망까지 남은 시간에 맞춰 금이 화면을 덮는다.
    /// 이미지 없이 선을 메시로 직접 그린다.
    /// </summary>
    public class CrackOverlay : Graphic
    {
        [SerializeField] private MemoryController memory;

        [Header("Shape")]
        [Tooltip("가장 많이 찼을 때의 금 개수.")]
        [SerializeField, Range(4, 400)] private int maxLines = 140;
        [SerializeField, Min(0.5f)] private float lineWidth = 3f;
        [Tooltip("처음 생기는 짧은 금의 길이. 화면 크기 대비 비율.")]
        [SerializeField, Range(0.02f, 1.5f)] private float minLength = 0.06f;
        [SerializeField, Range(0.05f, 1.5f)] private float maxLength = 0.75f;
        [Tooltip("꺾이는 각도. 클수록 어지럽게 갈라진다.")]
        [SerializeField, Range(0f, 90f)] private float jitterAngle = 40f;
        [Tooltip("모양을 바꾸고 싶을 때 숫자를 바꾼다.")]
        [SerializeField] private int seed = 12345;

        [Header("Intensity")]
        [Tooltip("메모리가 꽉 찼을 때(아직 안 죽었을 때) 금이 차오르는 정도.")]
        [SerializeField, Range(0f, 1f)] private float fullMemoryIntensity = 0.55f;

        private Crack[] _cracks;
        private float _intensity;

        private struct Crack
        {
            public Vector2[] Points;
        }

        protected override void Awake()
        {
            base.Awake();

            if (memory == null)
                memory = FindAnyObjectByType<MemoryController>();

            raycastTarget = false;
        }

        private void LateUpdate()
        {
            if (memory == null)
                return;

            float fill = memory.Capacity > 0
                ? (float)memory.UsedCount / memory.Capacity
                : 0f;

            // 채운 만큼 금이 늘고, 꽉 찬 뒤에는 사망까지 남은 시간이 화면을 마저 덮는다.
            float death = memory.IsDead ? 1f : memory.OverflowProgress;
            float target = Mathf.Lerp(fill * fullMemoryIntensity, 1f, death);

            if (Mathf.Abs(target - _intensity) < 0.001f)
                return;

            _intensity = target;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (_cracks == null || _cracks.Length != maxLines)
                BuildCracks();

            if (_intensity <= 0f)
                return;

            Rect area = rectTransform.rect;
            float shown = _intensity * _cracks.Length;

            for (int i = 0; i < _cracks.Length; i++)
            {
                float grown = Mathf.Clamp01(shown - i);
                if (grown <= 0f)
                    continue;

                DrawCrack(vh, _cracks[i], grown, area);
            }
        }

        /// <summary>
        /// 금의 모양을 미리 정해 둔다. 매 프레임 새로 뽑으면 화면이 떨린다.
        /// </summary>
        private void BuildCracks()
        {
            var random = new System.Random(seed);
            _cracks = new Crack[maxLines];

            for (int i = 0; i < maxLines; i++)
            {
                float along = (float)random.NextDouble();
                Vector2 start;
                Vector2 direction;

                switch (random.Next(4))
                {
                    case 0:
                        start = new Vector2(-0.5f + along, -0.5f);
                        direction = Vector2.up;
                        break;
                    case 1:
                        start = new Vector2(-0.5f + along, 0.5f);
                        direction = Vector2.down;
                        break;
                    case 2:
                        start = new Vector2(-0.5f, -0.5f + along);
                        direction = Vector2.right;
                        break;
                    default:
                        start = new Vector2(0.5f, -0.5f + along);
                        direction = Vector2.left;
                        break;
                }

                int segments = random.Next(3, 7);

                // 나중에 생기는 금일수록 길게. 처음에는 테두리 근처에만 짧게 생긴다.
                float order = maxLines > 1 ? (float)i / (maxLines - 1) : 1f;
                float lengthT = Mathf.Clamp01(order * 0.75f + (float)random.NextDouble() * 0.25f);
                float length = Mathf.Lerp(minLength, maxLength, lengthT);
                float step = length / segments;

                var points = new Vector2[segments + 1];
                points[0] = start;

                for (int s = 1; s <= segments; s++)
                {
                    float angle = ((float)random.NextDouble() - 0.5f) * jitterAngle * Mathf.Deg2Rad;
                    direction = Rotate(direction, angle).normalized;
                    points[s] = points[s - 1] + direction * step;
                }

                _cracks[i] = new Crack { Points = points };
            }
        }

        private void DrawCrack(VertexHelper vh, Crack crack, float grown, Rect area)
        {
            int segments = crack.Points.Length - 1;
            float visible = grown * segments;

            for (int s = 0; s < segments; s++)
            {
                float portion = Mathf.Clamp01(visible - s);
                if (portion <= 0f)
                    return;

                Vector2 from = crack.Points[s];
                Vector2 to = Vector2.Lerp(from, crack.Points[s + 1], portion);

                AddSegment(vh, ToLocal(from, area), ToLocal(to, area));
            }
        }

        private static Vector2 ToLocal(Vector2 normalized, Rect area)
        {
            return new Vector2(normalized.x * area.width, normalized.y * area.height);
        }

        private void AddSegment(VertexHelper vh, Vector2 from, Vector2 to)
        {
            Vector2 delta = to - from;
            if (delta.sqrMagnitude < 0.0001f)
                return;

            delta.Normalize();
            Vector2 offset = new Vector2(-delta.y, delta.x) * (lineWidth * 0.5f);

            int start = vh.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = from - offset;
            vh.AddVert(vertex);
            vertex.position = from + offset;
            vh.AddVert(vertex);
            vertex.position = to + offset;
            vh.AddVert(vertex);
            vertex.position = to - offset;
            vh.AddVert(vertex);

            vh.AddTriangle(start, start + 1, start + 2);
            vh.AddTriangle(start + 2, start + 3, start);
        }

        private static Vector2 Rotate(Vector2 value, float radians)
        {
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2(value.x * cos - value.y * sin, value.x * sin + value.y * cos);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _cracks = null;
            SetVerticesDirty();
        }
#endif
    }
}
