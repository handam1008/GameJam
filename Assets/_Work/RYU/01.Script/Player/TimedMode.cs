using UnityEngine;

namespace _Work.RYU._01.Script.Player
{
    // 일정 시간 켜졌다 꺼지는 버프의 공통 뼈대.
    // 자식은 OnEnter/OnExit/OnTick만 채우면 된다
    public abstract class TimedMode : MonoBehaviour
    {
        public bool IsActive { get; private set; }

        private float timer;

        public void Activate(float duration)
        {
            timer = duration;

            // 이미 켜져 있으면 시간만 연장한다
            if (IsActive)
                return;

            IsActive = true;
            OnEnter();
        }

        private void Update()
        {
            if (!IsActive)
                return;

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                IsActive = false;
                OnExit();
                return;
            }

            OnTick();
        }

        // 켜질 때 한 번
        protected virtual void OnEnter() { }

        // 꺼질 때 한 번
        protected virtual void OnExit() { }

        // 켜져 있는 동안 매 프레임
        protected virtual void OnTick() { }
    }
}
