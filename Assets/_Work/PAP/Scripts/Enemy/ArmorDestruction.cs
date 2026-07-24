using System;
using DG.Tweening;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy
{
    public class ArmorDestruction : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private bool isDestructioned = false;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        public void DestructionArmor()
        {
            if (isDestructioned) return;
            isDestructioned = true;
            _sr.DOFade(0f, 0.5f);
            transform.DOScale(0f, 0.5f);
        }

        private void OnDestroy()
        {
            _sr.DOKill();
            transform.DOKill();
        }
    }
}