using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Work.PAP.Scripts.Systems
{
    public class AnykeyOpen : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private GameObject targett;
        private void Update()
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                target.SetActive(true);
                targett.SetActive(false);
            }
        }
    }
}