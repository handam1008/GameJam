using System.Collections.Generic;
using RYU.Memory;
using RYU.Skill;
using UnityEngine;

namespace RYU.UI
{
    /// <summary>
    /// 가진 스킬만큼 버튼을 깔아준다.
    /// 버튼을 누르면 그 스킬이 메모리 스택 맨 아래로 들어간다.
    /// </summary>
    public class SkillPanel : MonoBehaviour
    {
        [SerializeField] private SkillInventory inventory;
        [SerializeField] private MemoryController memory;
        [Tooltip("복제할 버튼 프리팹.")]
        [SerializeField] private SkillButton buttonPrefab;
        [Tooltip("버튼을 붙일 곳. 비우면 이 오브젝트 밑에 붙는다.")]
        [SerializeField] private Transform buttonParent;

        private readonly List<SkillButton> _buttons = new List<SkillButton>();

        private void Awake()
        {
            if (inventory == null)
                inventory = FindAnyObjectByType<SkillInventory>();

            if (memory == null)
                memory = FindAnyObjectByType<MemoryController>();

            if (buttonParent == null)
                buttonParent = transform;
        }

        private void OnEnable()
        {
            if (inventory != null)
                inventory.OnChanged += Rebuild;

            Rebuild();
        }

        private void OnDisable()
        {
            if (inventory != null)
                inventory.OnChanged -= Rebuild;
        }

        private void Rebuild()
        {
            if (inventory == null || buttonPrefab == null)
                return;

            IReadOnlyList<SkillDefinition> skills = inventory.Skills;

            // 종류가 그대로면 개수만 고친다. 버튼을 새로 만들면 올려둔 마우스가 풀린다.
            if (MatchesCurrentButtons(skills))
            {
                for (int i = 0; i < _buttons.Count; i++)
                    _buttons[i].SetCount(inventory.GetCount(skills[i]));

                return;
            }

            ClearButtons();

            for (int i = 0; i < skills.Count; i++)
            {
                SkillButton button = Instantiate(buttonPrefab, buttonParent);
                button.gameObject.SetActive(true);
                button.Bind(skills[i], inventory.GetCount(skills[i]), Use);
                _buttons.Add(button);
            }
        }

        /// <summary>지금 깔린 버튼이 가진 스킬 목록과 순서까지 같은지.</summary>
        private bool MatchesCurrentButtons(IReadOnlyList<SkillDefinition> skills)
        {
            if (_buttons.Count != skills.Count)
                return false;

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] == null || _buttons[i].Skill != skills[i])
                    return false;
            }

            return true;
        }

        private void ClearButtons()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] != null)
                    Destroy(_buttons[i].gameObject);
            }

            _buttons.Clear();
        }

        /// <summary>메모리에 들어간 경우에만 개수를 깎는다.</summary>
        private void Use(SkillDefinition skill)
        {
            if (memory == null || inventory.GetCount(skill) <= 0)
                return;

            if (!memory.TryPickUp(skill.CreateStack()))
                return;

            inventory.TryUse(skill);
        }
    }
}
