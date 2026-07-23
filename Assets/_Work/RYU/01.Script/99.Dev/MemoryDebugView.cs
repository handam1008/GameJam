using System.Text;
using RYU.Memory;
using UnityEngine;

namespace RYU.Dev
{
    /// <summary>
    /// 메모리 칸 상태를 화면 좌상단에 그린다. 정식 UI가 나오면 지운다.
    /// </summary>
    [RequireComponent(typeof(MemoryController))]
    public class MemoryDebugView : MonoBehaviour
    {
        [SerializeField, Min(1)] private int fontSize = 48;
        [SerializeField] private Font font;
        [SerializeField] private Color textColor = Color.white;

        [SerializeField] private MemoryScanner scanner;

        private MemoryController _memory;
        private readonly StringBuilder _builder = new StringBuilder();
        private GUIStyle _style;

        private void Awake()
        {
            _memory = GetComponent<MemoryController>();
            Debug.Log($"[Memory] 시작. 칸 {_memory.Capacity}개", this);
        }

        private void OnEnable()
        {
            _memory.OnMemoryChanged += LogMemoryChanged;
            _memory.OnGarbageCollected += LogGarbageCollected;
            _memory.OnOverflow += LogOverflow;

            if (scanner == null)
                scanner = FindAnyObjectByType<MemoryScanner>();
            if (scanner != null)
                scanner.OnSlotExecuted += LogSlotExecuted;
        }

        private void OnDisable()
        {
            _memory.OnMemoryChanged -= LogMemoryChanged;
            _memory.OnGarbageCollected -= LogGarbageCollected;
            _memory.OnOverflow -= LogOverflow;

            if (scanner != null)
                scanner.OnSlotExecuted -= LogSlotExecuted;
        }

        private void LogSlotExecuted(int index)
        {
            Debug.Log($"[Scan] {index}번 칸 실행", this);
        }

        private void LogMemoryChanged()
        {
            Debug.Log($"[Memory] 메모리 사용. 가비지 {_memory.GarbageCount}/{_memory.Capacity}", this);
        }

        private void LogGarbageCollected(int cleared)
        {
            Debug.Log($"[Memory] GC 실행. {cleared}칸 정리", this);
        }

        private void LogOverflow()
        {
            Debug.Log("[Memory] 오버플로 발생", this);
        }

        private void OnGUI()
        {
            _style ??= new GUIStyle(GUI.skin.label);
            _style.fontSize = fontSize;
            _style.normal.textColor = textColor;
            if (font != null)
                _style.font = font;

            _builder.Clear();
            for (int i = 0; i < _memory.Capacity; i++)
                _builder.Append(_memory.GetSlot(i) == SlotState.Garbage ? "[X]" : "[ ]");

            _builder.Append($"   가비지 {_memory.GarbageCount}/{_memory.Capacity}");

            if (_memory.IsStunned)
                _builder.Append("   << OVERFLOW >>");

            GUI.Label(new Rect(20f, 20f, Screen.width - 40f, fontSize * 2f), _builder.ToString(), _style);
        }
    }
}
