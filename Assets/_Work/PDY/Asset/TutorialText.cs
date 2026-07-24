using System;
using System.Collections;
using _Work.PAP.Scripts;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace _Work.PDY.Asset
{
    public class TutorialText : MonoBehaviour
    {
        public bool canNext = false;
        private int currentText = 0;
        [SerializeField] private TutorialTextSO[] text;
        [SerializeField] TypewriterCore typewriter;
        [SerializeField] private PlayerInputSO playerInput;
        
        [SerializeField] private float height = 0.08f;
        [SerializeField] private float duration = 0.35f;
        

        private Coroutine bounceCoroutine;
        
        public void EndText()
        {
            canNext = true;
        }
        private void Start()
        {
            Bounce();
            typewriter.ShowText(text[0].text);
            canNext = false;
            currentText++;
        }
        private void OnEnable()
        {
            playerInput.OnMouseKeyPressed += Text;
            typewriter.onMessage.AddListener(OnMessage);
        }

        private void Text()
        {
            if (canNext)
            {
                Bounce();
                canNext = false;
                typewriter.ShowText(text[currentText].text);
                currentText++;
            }
        }

        private void OnMessage(EventMarker marker)
        {
            // 플레이에 따라서 진행되게 조건하고 싶으면 여기에
        }

        private void OnDisable()
        {
            typewriter.onMessage.RemoveAllListeners();
        }
        public void Bounce()
        {
            if (bounceCoroutine != null)
                StopCoroutine(bounceCoroutine);

            bounceCoroutine = StartCoroutine(BounceRoutine());
        }

        private IEnumerator BounceRoutine()
        {
            Vector3 startPosition = transform.localPosition;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;

                float progress = time / duration;
                float y = Mathf.Sin(progress * Mathf.PI * 2f) * height;

                transform.localPosition =
                    startPosition + Vector3.up * y;

                yield return null;
            }

            transform.localPosition = startPosition;
            bounceCoroutine = null;
        }
    }
}