using System;
using UnityEngine;

namespace _Work.PAP.Scripts.Agent
{
    public class AgentAnimator : MonoBehaviour
    {
        private Animator animator;
        public event Action OnAnimationEvent;
        public event Action OnAnimationEndEvent;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void AnimationEventTrigger()
        {
            OnAnimationEvent?.Invoke();
        }

        public void OnAnimatorEndEventTrigger()
        {
            OnAnimationEndEvent?.Invoke();
        }

        public void SetBool(int hashId, bool value) => animator.SetBool(hashId, value);
    }
}