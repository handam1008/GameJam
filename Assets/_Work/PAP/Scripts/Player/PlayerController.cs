using System;
using _Work.PAP.Scripts.Agent;
using RYU.Memory;
using UnityEngine;

namespace _Work.PAP.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private MemoryController memory;

        [Header("Move Stack Icons")]
        [Tooltip("메모리 칸에 이 이동이 표시될 때 쓸 아이콘. 방향별로 다르게 지정한다.")]
        [SerializeField] private Sprite upIcon;
        [SerializeField] private Sprite downIcon;
        [SerializeField] private Sprite leftIcon;
        [SerializeField] private Sprite rightIcon;

        public AgentMovement AgentMovement { get; private set; }

        private void Awake()
        {
            AgentMovement = GetComponentInChildren<AgentMovement>();
            if (memory == null) memory = GetComponent<MemoryController>();
        }

        private void HandleKeyPressed()
        {
            AbstractStack item = new AttackStack(1, null, this);
            memory.TryPickUp(item);
        }

        /// <summary>
        /// WASD 입력은 즉시 이동시키지 않고 메모리 스택 맨 아래에 쌓아둔다.
        /// 실제 이동은 스캐너가 이 칸을 실행할 때(MoveStack.Execute) AgentMovement에 방향이 들어가면서 일어난다.
        /// </summary>
        private void Update()
        {
            Vector2 movementInput = playerInput.MovementInput;
            Vector2Int direction = new Vector2Int((int)movementInput.x, (int)movementInput.y);
            if (direction == Vector2Int.zero) return;
            if (direction.x != 0 && direction.y != 0) return; // 대각선은 한 칸 이동으로 처리할 수 없다

            AgentMovement.SetMoveDirection(direction);
        }
    }
}