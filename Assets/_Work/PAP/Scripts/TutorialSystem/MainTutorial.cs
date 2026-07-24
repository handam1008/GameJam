using System;
using System.Collections;
using Febucci.UI;
using UnityEngine;

namespace _Work.PAP.Scripts.TutorialSystem
{
    public class MainTutorial : MonoBehaviour
    {
        [SerializeField] private TypewriterByCharacter writer;
        private IEnumerator _currentTutorial;

        private IEnumerator Tutorial()
        {
            writer.ShowText("안녕 처음보는 인간이네? 째깍 여긴 언더월드라 그래~ 이제부터 조작법을 알려줄게");
            yield return null;
            writer.ShowText("W , A , S , D 로 앞쪽, 왼쪽, 뒷쪽, 오른쪽으로 움직일 수 있어!");
            yield return null;
            writer.ShowText("너가 서있는 바닥이 보이지? 자 이제 마우스를 움직여봐!");
            yield return null;
            writer.ShowText("너의 마우스가 향하는 방향으로 우리의 공간이 회전되게 돼!");
            yield return null;
        }
    }
}