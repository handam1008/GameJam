using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Work.PAP.Scripts.Agent
{
    public class AfterImageEffect : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 0.25f;
        private SpriteRenderer _renderer;
        
        private CancellationTokenSource _tokenSource;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void StartTrail(bool isColored = false)
        {
            _tokenSource = new CancellationTokenSource();
            TrailAsync(isColored).Forget();
        }

        public void StopTrail()
        {
            CancelToken();
        }

        private void OnDestroy()
        {
            CancelToken();
        }

        private void CancelToken()
        {
            if (_tokenSource != null)
            {
                _tokenSource?.Cancel();
                _tokenSource?.Dispose();
                _tokenSource = null;
            }
        }

        private async UniTaskVoid TrailAsync(bool isColored)
        {
            while (true)
            {
                SpawnGhost(isColored);
                await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval),cancellationToken: _tokenSource.Token);
            }
        }

        public void SpawnGhost(bool isColored)
        {
            GameObject go = new GameObject("Trail");
            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _renderer.sprite;
            if (isColored)
            {
                spriteRenderer.color = new Color(0f,1f,0f,0.7f);
                spriteRenderer.DOColor(new Color(1f, 0f, 0f,0.7f), 0.5f).OnComplete(() => Destroy(go));
            }
            else
            {
                spriteRenderer.color = new Color(1,1,1,0.6f);
                spriteRenderer.DOFade(0f, 0.5f).OnComplete(() => Destroy(go));
            }
            spriteRenderer.sortingLayerName = "Trail";
            go.transform.SetPositionAndRotation(transform.position,transform.rotation);
            go.transform.localScale = transform.localScale;
        }
    }
}