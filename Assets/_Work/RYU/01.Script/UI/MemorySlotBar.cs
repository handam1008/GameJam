using RYU.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace RYU.UI
{
    /// <summary>
    /// 메모리 칸을 가로로 늘어놓고, 칸 상태와 스캐너 위치를 보여준다.
    /// 칸은 Capacity만큼 자동 생성하며, 재생하지 않아도 에디터에서 미리 보인다.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(HorizontalLayoutGroup))]
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
        [SerializeField] private Color garbageColor = new Color(0.85f, 0.25f, 0.3f);

        [Header("Scan Line")]
        [SerializeField, Min(1f)] private float beamWidth = 6f;
        [SerializeField] private Color beamColor = new Color(1f, 0.95f, 0.4f);

        private Image[] _frames;
        private Image[] _fills;
        private Image _beam;

        private void OnEnable()
        {
            ResolveReferences();
            Rebuild();
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
            Refresh();
            UpdateBeam();
        }

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

            for (int i = 0; i < count; i++)
            {
                _frames[i] = CreateFrame(i);
                _fills[i] = CreateFill(_frames[i].rectTransform);
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

        private Image CreateBeam()
        {
            var beam = new GameObject("ScanLine", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            beam.transform.SetParent(transform, false);
            MarkEditorOnly(beam);

            // 스캔선은 칸이 아니므로 가로 배치에서 빼야 한다.
            beam.GetComponent<LayoutElement>().ignoreLayout = true;

            var rect = beam.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(beamWidth, slotSize.y + framePadding * 2f);

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

            for (int i = 0; i < _fills.Length; i++)
            {
                _fills[i].color = memory.GetSlot(i) == SlotState.Garbage ? garbageColor : freeColor;
                _frames[i].color = i == scanIndex ? scanFrameColor : frameColor;
            }
        }

        private void UpdateBeam()
        {
            if (_beam == null || _frames == null || _frames.Length == 0 || scanner == null)
                return;

            float position = scanner.Position;
            int index = Mathf.Clamp(Mathf.FloorToInt(position), 0, _frames.Length - 1);
            float fraction = Mathf.Clamp01(position - index);

            Vector3 center = _frames[index].rectTransform.position;
            float stride = _frames.Length > 1
                ? Mathf.Abs(_frames[1].rectTransform.position.x - _frames[0].rectTransform.position.x)
                : slotSize.x;

            Vector3 beamPosition = center;
            beamPosition.x = center.x + (fraction - 0.5f) * stride;
            _beam.rectTransform.position = beamPosition;
        }
    }
}
