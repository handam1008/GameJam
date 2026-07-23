using RYU.Memory;
using UnityEngine;

namespace RYU.Skill
{
    /// <summary>
    /// 버튼에 뜨는 스킬 한 종류. 에셋으로 만들어 두고 바닥 아이템이 이걸 가리킨다.
    /// 무엇이 스택에 들어가는지는 상속받은 쪽이 정한다.
    /// </summary>
    public abstract class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Attack";
        [SerializeField] private Sprite icon;

        public string DisplayName => displayName;
        public Sprite Icon => icon;

        /// <summary>버튼을 눌렀을 때 스택에 넣을 것.</summary>
        public abstract AbstractStack CreateStack();
    }
}
