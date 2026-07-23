using System;
using System.Collections.Generic;
using _Work.PAP.Scripts.Agent;
using CoreLib;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class GameManager : MonoSingleTon<GameManager>
    {
        /// <summary>같은 틱 안에서 항상 OnMoveEvent보다 먼저 불린다. 이번 틱에 어느 방향으로 갈지 확정하는 단계(예: 메모리 스캐너).</summary>
        public event Action OnBeforeMoveEvent;

        /// <summary>OnBeforeMoveEvent에서 확정된 방향으로 실제 이동/행동을 실행하는 단계.</summary>
        public event Action OnMoveEvent;

        /// <summary>같은 틱 안에서 항상 OnMoveEvent보다 나중에 불린다. 이동이 끝난 뒤에 판정해야 하는 것(예: 적 공격 텔레그래프/타격).</summary>
        public event Action OnAfterMoveEvent;

        public float GameSpeed = 2f;

        public float targetSpeed;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            targetSpeed += Time.deltaTime * GameSpeed;
            if (targetSpeed > 1f)
            {
                OnBeforeMoveEvent?.Invoke();
                OnMoveEvent?.Invoke();
                OnAfterMoveEvent?.Invoke();
                targetSpeed -= 1f;
            }
        }
    }
}