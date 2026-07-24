using System;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.PAP.Scripts.FeedbackSystem
{
    public class ScreenShakeEffect : AbstractFeedBack
    {
        CinemachineImpulseSource impulseSource;
        [SerializeField] private float power = 0.5f;

        private void Awake()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public override void CreateFeedBack()
        {
            impulseSource.GenerateImpulseWithForce(power);
        }

        public override void StopFeedBack()
        {
        }
    }
}