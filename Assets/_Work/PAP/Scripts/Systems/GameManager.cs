using System;
using System.Collections.Generic;
using _Work.PAP.Scripts.Agent;
using CoreLib;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class GameManager : MonoSingleTon<GameManager>
    {
        public event Action OnMoveEvent;
        public float GameSpeed = 1f;

        private float targetSpeed;

        protected override void Awake()
        {
            base.Awake();
            targetSpeed = Time.time + GameSpeed;
        }

        private void Update()
        {
            if (targetSpeed < Time.time)
            {
                OnMoveEvent?.Invoke();
                targetSpeed = Time.time + GameSpeed;
            }
        }
    }
}