using System;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    public enum primaryTarget
    {
        owner,
        target,
    }
    [CreateAssetMenu(fileName = "Attack Pattern Data", menuName = "SO/Attack Pattern Data", order = 0)]
    public class AttackPatternSO : ScriptableObject
    {
        public int damage = 1;
        public int range = 3;
        public int patternSize = 3;
        public primaryTarget primary = primaryTarget.owner;
        public string AnimationName;
        public int AnimationHash;
        public float duration;
        public float postDelay;

        [SerializeField] private bool[] _pattern = new bool[9];

        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(AnimationName))
            {
                AnimationHash = Animator.StringToHash(AnimationName);
            }
            else
            {
                AnimationHash = 0;
            }
        }

        /// <summary>(x, y)는 0 ~ patternSize-1 범위. 범위 밖이면 false.</summary>
        public bool GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= patternSize || y >= patternSize) return false;
            int index = y * patternSize + x;
            if (_pattern == null || index >= _pattern.Length) return false;
            return _pattern[index];
        }

        /// <summary>
        /// 패턴은 위쪽(0,1)을 바라본다고 가정하고 그려진다.
        /// 실제 바라보는 방향(facing)에 맞춰 회전시킨, 시전자 기준 상대 셀 오프셋을 돌려준다.
        /// </summary>
        public Vector2Int GetRotatedOffset(int x, int y, Vector2Int facing)
        {
            int center = patternSize / 2;
            int lx = x - center;
            int ly = y - center;

            if (facing == Vector2Int.right) return new Vector2Int(ly, -lx);
            if (facing == Vector2Int.down) return new Vector2Int(-lx, -ly);
            if (facing == Vector2Int.left) return new Vector2Int(-ly, lx);
            return new Vector2Int(lx, ly); // up (기본 방향)
        }
    }
}
