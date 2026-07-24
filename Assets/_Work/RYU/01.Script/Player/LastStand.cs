using System;
using _Work.PAP.Scripts.Agent;
using _Work.PAP.Scripts.Player;
using UnityEngine;

namespace _Work.RYU._01.Script.Player
{
    // 패시브. 장착하면 체력을 감시하다가, 체력이 1(마지막)이면 이동속도를 크게 올린다.
    // 체력이 1 초과로 회복되면 속도가 원래대로 돌아온다.
    public class LastStand : MonoBehaviour
    {
        // 발동 기준 체력. 이 값 이하면 속도 증가
        [SerializeField] private int triggerHealth = 1;

        // 발동 시 이동속도 배율 (2 = 2배)
        [SerializeField] private float speedBoost = 2f;

        private PlayerHealth health;
        private AgentMovement movement;

        // 장착됐는지. 아이템 먹기 전엔 작동 안 함
        private bool equipped;
        private bool boosting;

        // 분노 발동 중인지. UI가 이걸 보고 붉게 변한다
        public bool IsRaging => boosting;

        // 발동/해제 순간 발행 (true=발동, false=해제)
        public event Action<bool> OnRageChanged;

        private void Awake()
        {
            health = GetComponent<PlayerHealth>();
            movement = GetComponent<AgentMovement>();
        }

        // 아이템이 부른다. 이때부터 체력 감시 시작
        public void Equip()
        {
            equipped = true;
        }

        private void Update()
        {
            if (!equipped || health == null)
                return;

            // 체력이 발동 기준 이하 && 안 죽었을 때만 속도 증가
            bool shouldBoost = !health.IsDead && health.CurrentHealth <= triggerHealth;

            if (shouldBoost && !boosting)
            {
                boosting = true;
                movement.SetSpeedMultiplier(this, speedBoost);
                OnRageChanged?.Invoke(true);
            }
            else if (!shouldBoost && boosting)
            {
                boosting = false;
                movement.ClearSpeedMultiplier(this);
                OnRageChanged?.Invoke(false);
            }
        }
    }
}
