using _Work.PAP.Scripts.Agent;
using UnityEngine;

namespace _Work.RYU._01.Script.Player
{
    // 신발. 일정 시간 이동속도가 빨라진다. TimedMode라 남은 시간을 읽을 수 있다
    public class SpeedMode : TimedMode
    {
        // 빨라지는 배율 (1.5 = 1.5배)
        [SerializeField] private float speedBoost = 1.5f;

        private AgentMovement movement;

        private void Awake()
        {
            movement = GetComponent<AgentMovement>();
        }

        protected override void OnEnter()
        {
            movement.SetSpeedMultiplier(this, speedBoost);
        }

        protected override void OnExit()
        {
            movement.ClearSpeedMultiplier(this);
        }
    }
}
