using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Work.PAP.Scripts.Systems
{
    public class Reset : MonoBehaviour
    {
        [SerializeField] private StageBreakingSystem stageBreakingSystem;

        private void Update()
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                stageBreakingSystem.SceneMoveTransition(1);
            }
        }
    }
}