using System;
using CSILib.SoundManager.RunTime;
using DG.Tweening;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class StageBreakingSystem : MonoSingleton<StageBreakingSystem>
    {
        [SerializeField] private int reachToClear = 50;
        private int currentReach = 0;
        private SpriteRenderer _sr;
        [SerializeField] private AnimationCurve _curve;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        public void AddReach(int count)
        {
            currentReach += count;
            _sr.DOColor(new Color(0, 0, 0, 1 - _curve.Evaluate((float)currentReach / (float)reachToClear)),1f);
        }
    }
}