using System;
using System.Collections.Generic;
using csiimnida.CSILib.SoundManager.RunTime;
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
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject targetObject;
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
                SoundManager.Instance.PlaySound("Crack");
                _sr.sprite = levelSprites[currentIndex];
                impulser.GenerateImpulseWithForce(0.5f);
            }
            else
            {
                SoundManager.Instance.PlaySound("Break");
                _sr.sprite = levelSprites[currentIndex];
                Time.timeScale = 0;
                group.DOFade(0, 2f).SetUpdate(true);
                CameraTrm.DOMoveY(60f,4f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
                {
                    targetObject.SetActive(true);
                });
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