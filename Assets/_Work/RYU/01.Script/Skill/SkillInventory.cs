using System;
using System.Collections.Generic;
using UnityEngine;

namespace RYU.Skill
{
    /// <summary>
    /// 주운 스킬을 종류별로 몇 개 갖고 있는지 센다.
    /// 같은 스킬을 또 주우면 개수만 늘고, 0이 되면 목록에서 빠진다.
    /// </summary>
    public class SkillInventory : MonoBehaviour
    {
        /// <summary>버튼 순서가 멋대로 바뀌지 않도록 처음 주운 순서를 지킨다.</summary>
        private readonly List<SkillDefinition> _order = new List<SkillDefinition>();

        private readonly Dictionary<SkillDefinition, int> _counts =
            new Dictionary<SkillDefinition, int>();

        /// <summary>가진 스킬이 늘거나 줄 때 발행.</summary>
        public event Action OnChanged;

        public IReadOnlyList<SkillDefinition> Skills => _order;

        public int GetCount(SkillDefinition skill)
        {
            return skill != null && _counts.TryGetValue(skill, out int count) ? count : 0;
        }

        public void Add(SkillDefinition skill, int amount = 1)
        {
            if (skill == null || amount <= 0)
                return;

            if (_counts.ContainsKey(skill))
            {
                _counts[skill] += amount;
            }
            else
            {
                _counts[skill] = amount;
                _order.Add(skill);
            }

            OnChanged?.Invoke();
        }

        /// <summary>하나 꺼내 쓴다. 없으면 false.</summary>
        public bool TryUse(SkillDefinition skill)
        {
            if (GetCount(skill) <= 0)
                return false;

            int left = _counts[skill] - 1;
            if (left > 0)
            {
                _counts[skill] = left;
            }
            else
            {
                _counts.Remove(skill);
                _order.Remove(skill);
            }

            OnChanged?.Invoke();
            return true;
        }
    }
}
