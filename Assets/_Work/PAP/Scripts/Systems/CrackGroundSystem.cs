using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class CrackGroundSystem : MonoBehaviour
    {
        private SpriteRenderer _sr;
        [SerializeField] private List<Sprite> levelSprites;
        [SerializeField] private Transform CameraTrm;
        private CinemachineImpulseSource impulser;
        private int currentIndex = 0;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            impulser = GetComponent<CinemachineImpulseSource>();
        }

        public void Crack()
        {
            currentIndex++;
            if (currentIndex < levelSprites.Count-1)
            {
                _sr.sprite = levelSprites[currentIndex];
                impulser.GenerateImpulseWithForce(0.5f);
            }
            else
            {
                _sr.sprite = levelSprites[currentIndex];
                Time.timeScale = 0;
                CameraTrm.DOMoveY(60f,4f).SetEase(Ease.InCubic).SetUpdate(true);
                CameraTrm.DOShakeRotation(2f,Vector3.one).SetUpdate(true);
            }
        }

        public void Recover()
        {
            currentIndex--;
            if (currentIndex >= 0)
            {
                _sr.sprite = levelSprites[currentIndex];
            }
            else
            {
                currentIndex = 0;
            }
        }
    }
}