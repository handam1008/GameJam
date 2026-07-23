using _Work.PAP.Scripts.Agent;
using UnityEngine;

namespace RYU.Memory
{
    /// <summary>
    /// 플레이어가 입력한 이동(WASD) 한 번을 담아두는 메모리 칸.
    /// 입력한 즉시 움직이지 않고, 스캐너가 이 칸에 도착해 Execute를 부를 때
    /// 비로소 AgentMovement에 방향을 넣어 실제로 이동시킨다.
    /// </summary>
    public class MoveStack : AbstractStack
    {
        private readonly Vector2Int _direction;
        private readonly AgentMovement _agentMovement;
        private readonly Sprite _icon;

        public MoveStack(Vector2Int direction, AgentMovement agentMovement, Sprite icon = null)
        {
            _direction = direction;
            _agentMovement = agentMovement;
            _icon = icon;
        }

        public override string DisplayName => $"이동 {DirectionLabel(_direction)}";

        public override Sprite Icon => _icon;

        public override void Execute()
        {
            _agentMovement.SetMoveDirection(_direction);
        }

        private static string DirectionLabel(Vector2Int dir)
        {
            if (dir == Vector2Int.up) return "↑";
            if (dir == Vector2Int.down) return "↓";
            if (dir == Vector2Int.left) return "←";
            if (dir == Vector2Int.right) return "→";
            return "•";
        }
    }
}
