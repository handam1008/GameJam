using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Work.PAP.Scripts.Systems
{
    // 엔딩 씬에 붙인다. 잠시 뒤 자동으로 메인 타이틀 씬으로 넘어간다.
    public class EndSceneAutoReturn : MonoBehaviour
    {
        // 넘어갈 타이틀 씬 이름 (Build Settings에 등록돼 있어야 함)
        [SerializeField] private string titleScene = "MainMenu";

        // 몇 초 뒤에 넘어갈지
        [SerializeField] private float delay = 5f;

        // 넣으면 넘어가기 전 이걸로 서서히 어두워진다(페이드). 비우면 바로 넘어간다
        [SerializeField] private CanvasGroup fade;
        [SerializeField] private float fadeTime = 1f;

        private void Start()
        {
            StartCoroutine(ReturnRoutine());
        }

        private IEnumerator ReturnRoutine()
        {
            yield return new WaitForSeconds(delay);

            if (fade != null)
                fade.DOFade(1f, fadeTime).OnComplete(() => SceneManager.LoadScene(titleScene));
            else
                SceneManager.LoadScene(titleScene);
        }
    }
}
