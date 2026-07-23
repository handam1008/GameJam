using RYU.Memory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RYU.UI
{
    /// <summary>
    /// 메모리 칸을 세로로 쌓고, 칸 상태와 스캐너 위치를 보여준다. 0번 칸이 맨 위다.
    /// 칸은 Capacity만큼 자동 생성하며, 재생하지 않아도 에디터에서 미리 보인다.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class MemorySlotBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MemoryController memory;
        [SerializeField] private MemoryScanner scanner;

        [Header("Layout")]
        [SerializeField] private Vector2 slotSize = new Vector2(72f, 72f);
        [SerializeField, Min(0f)] private float framePadding = 6f;

        [Header("Colors")]
        [SerializeField] private Color frameColor = new Color(0.15f, 0.15f, 0.18f);
        [SerializeField] private Color scanFrameColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private Color freeColor = new Color(0.25f, 0.28f, 0.35f);
        [Tooltip("아직 실행하지 않은 데이터.")]
        [SerializeField] private Color dataColor = new Color(0.3f, 0.75f, 0.95f);
        [Tooltip("실행이 끝났거나 피격으로 망가진 칸.")]
        [SerializeField] private Color garbageColor = new Color(0.85f, 0.25f, 0.3f);

        [Header("Scan Line")]
        [SerializeField, Min(1f)] private float beamWidth = 6f;
        [SerializeField] private Color beamColor = new Color(1f, 0.95f, 0.4f);

        [Header("Execute Flash")]
        [Tooltip("실행되는 순간 칸이 이 색으로 번쩍인다.")]
        [SerializeField] private Color flashColor = new Color(1f, 0.75f, 0.15f);
        [Tooltip("번쩍임이 사라지는 데 걸리는 시간(초).")]
        [SerializeField, Min(0.01f)] private float flashDuration = 0.25f;
        [Tooltip("실행 순간 칸이 커지는 배율.")]
        [SerializeField, Min(1f)] private float flashScale = 1.25f;

        [Header("Insert / Remove")]
        [Tooltip("스택에 들어올 때 칸이 이 색으로 번쩍인다.")]
        [SerializeField] private Color appearFlash = Color.white;
        [SerializeField, Min(0.01f)] private float appearDuration = 0.22f;
        [Tooltip("들어오는 순간 부풀었다가 줄어드는 배율.")]
        [SerializeField, Min(1f)] private float appearScale = 1.45f;
        [SerializeField, Min(0.01f)] private float vanishDuration = 0.3f;
        [Tooltip("빠질 때 쪼그라드는 배율.")]
        [SerializeField, Range(0f, 1f)] private float vanishScale = 0.55f;

        [Header("Smoothing")]
        [Tooltip("색이 바뀔 때 스며드는 속도. 낮을수록 부드럽고 높을수록 즉각적이다.")]
        [SerializeField, Min(0.5f)] private float colorSpeed = 8f;

        [Header("Icon / Text")]
        [Tooltip("칸에 담긴 스택의 아이콘/이름을 보여줄지.")]
        [SerializeField] private bool showItemInfo = true;
        [SerializeField, Min(1f)] private float labelFontSize = 14f;
        [SerializeField] private Color labelColor = Color.white;

        private Image[] _frames;
        private Image[] _fills;
        private Image[] _icons;
        private TextMeshProUGUI[] _labels;
        private Image _beam;

        /// <summary>칸별 번쩍임 잔여 시간.</summary>
        private float[] _flashTimers;

        /// <summary>들어오고 빠지는 연출의 잔여 시간.</summary>
        private float[] _appearTimers;
        private float[] _vanishTimers;

        /// <summary>지금 화면에 보이는 색. 목표 색으로 서서히 스며든다.</summary>
        private Color[] _fillColors;
        private Color[] _frameColors;

        /// <summary>지난 프레임의 칸 상태. 바뀐 순간을 잡아 연출을 건다.</summary>
        private SlotState[] _lastStates;

        private void OnEnable()
        {
            ResolveReferences();
            Rebuild();

            if (scanner != null)
                scanner.OnSlotExecuted += StartFlash;
        }

        private void OnDisable()
        {
            if (scanner != null)
                scanner.OnSlotExecuted -= StartFlash;
        }

        private void StartFlash(int index)
        {
            if (_flashTimers == null || index < 0 || index >= _flashTimers.Length)
                return;

            _flashTimers[index] = flashDuration;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            // OnValidate 도중에는 오브젝트를 만들거나 지울 수 없어 한 프레임 미룬다.
            UnityEditor.EditorApplication.delayCall += RebuildFromEditor;
        }

        private void RebuildFromEditor()
        {
            if (this == null || Application.isPlaying)
                return;

            ResolveReferences();
            Rebuild();
        }
#endif

        private void LateUpdate()
        {
            DetectChanges();
            TickTimers();
            Refresh();
            UpdateBeam();
        }

        /// <summary>칸이 차거나 비는 순간을 잡아 연출을 건다.</summary>
        private void DetectChanges()
        {
            if (_lastStates == null || memory == null)
                return;

            for (int i = 0; i < _lastStates.Length; i++)
            {
                SlotState now = memory.GetSlot(i);
                SlotState before = _lastStates[i];
                if (now == before)
                    continue;

                _lastStates[i] = now;

                if (before == SlotState.Free)
                {
                    _appearTimers[i] = appearDuration;
                }
                else if (now == SlotState.Free)
                {
                    _vanishTimers[i] = vanishDuration;
                }
            }
        }

        private void TickTimers()
        {
            if (_flashTimers == null)
                return;

            for (int i = 0; i < _flashTimers.Length; i++)
            {
                if (_flashTimers[i] > 0f)
                    _flashTimers[i] -= Time.deltaTime;
                if (_appearTimers[i] > 0f)
                    _appearTimers[i] -= Time.deltaTime;
                if (_vanishTimers[i] > 0f)
                    _vanishTimers[i] -= Time.deltaTime;
            }
        }

        /// <summary>끝으로 갈수록 느려진다. 갑자기 멈추지 않게.</summary>
        private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);

        /// <summary>목표를 살짝 넘겼다 돌아온다. 튕기는 느낌을 준다.</summary>
        private static float EaseOutBack(float t)
        {
            const float overshoot = 1.70158f;
            float p = t - 1f;
            return 1f + (overshoot + 1f) * p * p * p + overshoot * p * p;
        }

        private Color ColorOf(SlotState state) => state switch
        {
            SlotState.Data => dataColor,
            SlotState.Garbage => garbageColor,
            _ => freeColor
        };

        /// <summary>인스펙터에서 연결하지 않았으면 씬에서 찾아 쓴다.</summary>
        private void ResolveReferences()
        {
            if (memory == null)
                memory = FindAnyObjectByType<MemoryController>();

            if (scanner == null)
                scanner = FindAnyObjectByType<MemoryScanner>();
        }

        private void Rebuild()
        {
            ClearSlots();

            if (memory == null)
                return;

            int count = memory.Capacity;
            _frames = new Image[count];
            _fills = new Image[count];
            _icons = new Image[count];
            _labels = new TextMeshProUGUI[count];
            _flashTimers = new float[count];
            _appearTimers = new float[count];
            _vanishTimers = new float[count];
            _fillColors = new Color[count];
            _frameColors = new Color[count];

            // 시작하자마자 없던 연출이 터지지 않도록 지금 상태를 기준으로 삼는다.
            _lastStates = new SlotState[count];
            for (int i = 0; i < count; i++)
            {
                _lastStates[i] = memory.GetSlot(i);
                _fillColors[i] = ColorOf(_lastStates[i]);
                _frameColors[i] = frameColor;
            }

            for (int i = 0; i < count; i++)
            {
                _frames[i] = CreateFrame(i);
                _fills[i] = CreateFill(_frames[i].rectTransform);
                _icons[i] = CreateIcon(_frames[i].rectTransform);
                _labels[i] = CreateLabel(_frames[i].rectTransform);
            }

            _beam = CreateBeam();

            Refresh();
            UpdateBeam();
        }

        private void ClearSlots()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
            }

            _frames = null;
            _fills = null;
            _icons = null;
            _labels = null;
            _beam = null;
        }

        private Image CreateFrame(int index)
        {
            var frame = new GameObject($"Slot_{index}", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            frame.transform.SetParent(transform, false);
            MarkEditorOnly(frame);

            var layout = frame.GetComponent<LayoutElement>();
            layout.preferredWidth = slotSize.x;
            layout.preferredHeight = slotSize.y;

            // 레이아웃 그룹이 크기를 안 잡는 설정이어도 칸이 0x0으로 사라지지 않게 직접 지정한다.
            frame.GetComponent<RectTransform>().sizeDelta = slotSize;

            return frame.GetComponent<Image>();
        }

        private Image CreateFill(RectTransform parent)
        {
            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(parent, false);
            MarkEditorOnly(fill);

            var rect = fill.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(framePadding, framePadding);
            rect.offsetMax = new Vector2(-framePadding, -framePadding);

            return fill.GetComponent<Image>();
        }

        private Image CreateIcon(RectTransform parent)
        {
            var icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(parent, false);
            MarkEditorOnly(icon);

            var rect = icon.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(framePadding, framePadding);
            rect.offsetMax = new Vector2(-framePadding, -framePadding);

            var image = icon.GetComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            image.enabled = false;
            return image;
        }

        private TextMeshProUGUI CreateLabel(RectTransform parent)
        {
            var label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(parent, false);
            MarkEditorOnly(label);

            var rect = label.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var text = label.GetComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Bottom;
            text.fontSize = labelFontSize;
            text.color = labelColor;
            text.raycastTarget = false;
            text.text = string.Empty;
            return text;
        }

        private Image CreateBeam()
        {
            var beam = new GameObject("ScanLine", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            beam.transform.SetParent(transform, false);
            MarkEditorOnly(beam);

            // 스캔선은 칸이 아니므로 세로 배치에서 빼야 한다.
            beam.GetComponent<LayoutElement>().ignoreLayout = true;

            var rect = beam.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(slotSize.x + framePadding * 2f, beamWidth);

            var image = beam.GetComponent<Image>();
            image.color = beamColor;
            image.raycastTarget = false;
            return image;
        }

        /// <summary>에디터에서 만든 미리보기 오브젝트는 씬 파일에 저장하지 않는다.</summary>
        private static void MarkEditorOnly(GameObject target)
        {
            if (!Application.isPlaying)
                target.hideFlags = HideFlags.DontSave;
        }

        private void Refresh()
        {
            if (_fills == null || memory == null)
                return;

            int scanIndex = scanner != null ? scanner.CurrentIndex : -1;

            // 에디터 미리보기에서는 deltaTime이 0일 수 있어 색이 멈춰버린다.
            float smoothDelta = Application.isPlaying ? Time.deltaTime : 1f / 60f;

            for (int i = 0; i < _fills.Length; i++)
            {
                // 0에서 1로 흐르는 진행도. 남은 시간이 아니라 지나온 정도를 쓴다.
                float flash = 1f - Mathf.Clamp01(_flashTimers[i] / flashDuration);
                float appear = 1f - Mathf.Clamp01(_appearTimers[i] / appearDuration);
                float vanish = 1f - Mathf.Clamp01(_vanishTimers[i] / vanishDuration);

                bool appearing = _appearTimers[i] > 0f;
                bool vanishing = _vanishTimers[i] > 0f;

                Color targetFill = ColorOf(memory.GetSlot(i));

                // 들어오는 순간에만 하얗게 물들이고, 그 뒤로는 제 색으로 풀린다.
                if (appearing)
                    targetFill = Color.Lerp(targetFill, appearFlash, 1f - EaseOutCubic(appear));

                Color targetFrame = i == scanIndex ? scanFrameColor : frameColor;
                if (_flashTimers[i] > 0f)
                    targetFrame = Color.Lerp(targetFrame, flashColor, 1f - EaseOutCubic(flash));

                // 색은 목표를 향해 계속 스며든다. 상태가 바뀌어도 툭 끊기지 않는다.
                float blend = 1f - Mathf.Exp(-colorSpeed * smoothDelta);
                _fillColors[i] = Color.Lerp(_fillColors[i], targetFill, blend);
                _frameColors[i] = Color.Lerp(_frameColors[i], targetFrame, blend);

                _fills[i].color = _fillColors[i];
                _frames[i].color = _frameColors[i];

                RefreshItemInfo(i);

                // 크기는 세 연출이 겹칠 수 있어 배율을 곱해서 함께 반영한다.
                float scale = 1f;

                if (_flashTimers[i] > 0f)
                    scale *= Mathf.Lerp(flashScale, 1f, EaseOutCubic(flash));

                // 부풀었다가 살짝 넘겼다 돌아오게 해서 툭 튀는 느낌을 준다.
                if (appearing)
                    scale *= Mathf.LerpUnclamped(appearScale, 1f, EaseOutBack(appear));

                // 쪼그라들었다 제자리로. 양 끝이 0이라 매끄럽게 이어진다.
                if (vanishing)
                    scale *= Mathf.Lerp(1f, vanishScale, Mathf.Sin(vanish * Mathf.PI));

                _frames[i].rectTransform.localScale = Vector3.one * scale;
            }
        }

        /// <summary>칸에 담긴 스택의 아이콘/이름을 채운다. 비었거나 가비지면 지운다.</summary>
        private void RefreshItemInfo(int index)
        {
            AbstractStack item = showItemInfo && memory.GetSlot(index) == SlotState.Data
                ? memory.GetItem(index)
                : null;

            if (_icons != null && _icons[index] != null)
            {
                Sprite sprite = item?.Icon;
                _icons[index].sprite = sprite;
                _icons[index].enabled = sprite != null;
            }

            if (_labels != null && _labels[index] != null)
                _labels[index].text = item != null ? item.DisplayName : string.Empty;
        }

        private void UpdateBeam()
        {
            if (_beam == null || _frames == null || _frames.Length == 0 || scanner == null)
                return;

            int index = scanner.CurrentIndex;
            if (!scanner.IsActive || index < 0)
            {
                _beam.enabled = false;
                return;
            }

            _beam.enabled = true;
            index = Mathf.Clamp(index, 0, _frames.Length - 1);
            float fraction = Mathf.Clamp01(scanner.Fraction);

            Vector3 center = _frames[index].rectTransform.position;

            // 인덱스가 오를수록 위로 가는 간격. 스캔선은 그 반대로 내려온다.
            float stride = _frames.Length > 1
                ? _frames[1].rectTransform.position.y - _frames[0].rectTransform.position.y
                : slotSize.y;

            Vector3 beamPosition = center;
            beamPosition.y = center.y + (0.5f - fraction) * stride;
            _beam.rectTransform.position = beamPosition;
        }
    }
}
