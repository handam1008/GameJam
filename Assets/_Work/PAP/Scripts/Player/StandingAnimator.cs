using System;
using UnityEngine;

namespace _Work.PAP.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class StandingAnimator : MonoBehaviour
    {
        private Animator animator;
        private string targetAnim = "IsInf";
        private int hash;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            hash = Animator.StringToHash(targetAnim);
        }
        
        public void SetBool(bool value) => animator.SetBool(hash, value);
    }
}