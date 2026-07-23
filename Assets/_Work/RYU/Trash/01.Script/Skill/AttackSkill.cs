using RYU.Memory;
using UnityEngine;

namespace RYU.Skill
{
    /// <summary>바닥에서 주운 칼 같은 공격 스킬.</summary>
    [CreateAssetMenu(menuName = "RYU/Skill/Attack", fileName = "AttackSkill")]
    public class AttackSkill : SkillDefinition
    {
        [SerializeField, Min(0f)] private float damage = 10f;

        public override AbstractStack CreateStack() => new AttackStack(damage, Icon);
    }
}
