using System;
using RYU.Skill;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RYU.UI
{
    /// <summary>
    /// 스킬 하나를 보여주는 버튼. 아이콘, 이름, 개수를 채운다.
    /// 마우스를 올리면 밝아지면서 살짝 커진다.
    /// </summary>
    public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button button;

        [Header("Hover")]
        [Tooltip("마우스를 올리지 않았을 때의 밝기.")]
        [SerializeField] private Color normalTint = new Color(0.62f, 0.62f, 0.62f);
        [Tooltip("마우스를 올렸을 때의 밝기.")]
        [SerializeField] private Color hoverTint = Color.white;
        [SerializeField, Min(1f)] private float hoverScale = 1.1f;
        [Tooltip("밝아지고 커지는 속도. 클수록 즉각적이다.")]
        [SerializeField, Min(0.1f)] private float transitionSpeed = 12f;

        private Image _background;
        private SkillDefinition _skill;
        private Action<SkillDefinition> _onClick;

        /// <summary>0이면 평소, 1이면 마우스가 올라간 상태.</summary>
        private float _hover;
        private bool _pointerInside;

        public SkillDefinition Skill => _skill;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Awake()
        {
            _background = GetComponent<Image>();

            if (button == null)
                button = GetComponent<Button>();

            if (button != null)
            {
                // 색 연출을 직접 하므로 버튼 기본 전환은 끈다. 안 그러면 서로 덮어쓴다.
                button.transition = Selectable.Transition.None;
                button.onClick.AddListener(HandleClick);
            }

            ApplyHover();
        }

        private void OnDisable()
        {
            _pointerInside = false;
            _hover = 0f;
        }

        private void Update()
        {
            float target = _pointerInside ? 1f : 0f;
            if (Mathf.Approximately(_hover, target))
                return;

            _hover = Mathf.MoveTowards(_hover, target, Time.unscaledDeltaTime * transitionSpeed);
            ApplyHover();
        }

        public void Bind(SkillDefinition skill, int count, Action<SkillDefinition> onClick)
        {
            _skill = skill;
            _onClick = onClick;

            if (icon != null)
            {
                icon.sprite = skill.Icon;
                // 아이콘을 아직 안 넣었으면 빈 네모가 남지 않게 숨긴다.
                icon.enabled = skill.Icon != null;
            }

            if (nameText != null)
                nameText.text = skill.DisplayName;

            SetCount(count);
            ApplyHover();
        }

        public void SetCount(int count)
        {
            if (countText != null)
                countText.text = count.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData) => _pointerInside = true;

        public void OnPointerExit(PointerEventData eventData) => _pointerInside = false;

        private void ApplyHover()
        {
            Color tint = Color.Lerp(normalTint, hoverTint, _hover);

            if (_background != null)
                _background.color = tint;

            if (icon != null)
                icon.color = tint;

            transform.localScale = Vector3.one * Mathf.Lerp(1f, hoverScale, _hover);
        }

        private void HandleClick()
        {
            if (_skill != null)
                _onClick?.Invoke(_skill);
        }
    }
}
