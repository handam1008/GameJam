using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Work.PAP.Scripts.Systems
{
    public class MoveScene : MonoBehaviour
    {
        [SerializeField] private CanvasGroup whiteboard;

        public void MoveNow(int scene)
        {
            whiteboard.DOFade(1f, 1f).OnComplete(() => SceneManager.LoadScene(scene));
        }
    }
}