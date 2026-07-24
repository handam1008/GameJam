using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Work.PAP.Scripts.Systems
{
    public class ChangeMap : MonoBehaviour
    {
        private Vector3[] pos =
        {
            new Vector3(2f, 2f, 0),
            new Vector3(2f, -2f, 0),
            new Vector3(-2f, -2f, 0),
            new Vector3(-2f, 2f, -0.5f),
        };
        [SerializeField] private List<GameObject> walls = new List<GameObject>();
        [SerializeField] private float ChangeTiming = 4f;

        private float _currentTime;
        private int index = 0;
        private void Awake()
        {
            _currentTime = Time.time + ChangeTiming;
        }

        private void Update()
        {
            if (Time.time > _currentTime)
            {
                _currentTime = Time.time + ChangeTiming;
                int beforeIndex = index;
                if (index < pos.Length-1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
                walls[beforeIndex].SetActive(true);
                transform.DOLocalMove(pos[index], 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    walls[index].SetActive(false);
                });
            }
        }
    }
}