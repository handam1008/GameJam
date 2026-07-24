using System;
using _Work.PAP.Scripts.Agent;
using UnityEngine;

namespace _Work.RYU._01.Script.Player
{
    public class SlowMode : TimedMode
    {
        // 느려질 때 총알 속도 배율. 0.3이면 30% 속도
        [SerializeField] private float slowScale = 0.3f;
        private AfterImageEffect afterImageEffect;

        private void Awake()
        {
            afterImageEffect = GetComponentInChildren<AfterImageEffect>();
        }

        protected override void OnEnter()
        {
            BulletTime.Scale = slowScale;
            afterImageEffect.StartTrail(true);
        }

        protected override void OnExit()
        {
            BulletTime.Scale = 1f;
            afterImageEffect.StopTrail();
        }
    }
}
