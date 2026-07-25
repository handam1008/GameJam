using System;
using System.Collections;
using CSILib.SoundManager.RunTime;
using Febucci.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Work.PAP.Scripts.TutorialSystem
{
    public class MainTutorial : MonoSingleton<MainTutorial>
    {
        [SerializeField] private TypewriterByCharacter writer;
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private SupplyPlane plane;
        [SerializeField] private SupplyPlane itemPlane;
        private IEnumerator _currentTutorial;

        private void Awake()
        {
            _currentTutorial =  Tutorial();
            _currentTutorial.MoveNext();
            
        }

        private void ClickTutorial()
        {
            playerInput.OnMouseKeyPressed += ClickNextTuto;
        }

        private void ClickNextTuto()
        {
            playerInput.OnMouseKeyPressed -= ClickNextTuto;
            _currentTutorial.MoveNext();
        }

        public void Next()
        {
            _currentTutorial.MoveNext();
        }

        private IEnumerator Tutorial()
        {
            writer.ShowText("안녕! 지금부터 게임 플레이 방법을 알려줄게!\n클릭하면 다음으로 넘어갈 수 있어.");
            ClickTutorial();
            yield return null;

            writer.ShowText("W, A, S, D로 움직이고,\nShift를 눌러 대시할 수 있어!");
            ClickTutorial();
            yield return null;

            writer.ShowText("네가 서 있는 바닥이 보이지?\n마우스를 움직여 봐!");
            ClickTutorial();
            yield return null;

            writer.ShowText("마우스를 움직이면 바닥도 함께 회전해!");
            ClickTutorial();
            yield return null;

            plane.CallPlane();

            writer.ShowText("앗, 위험해! 적이 나타났어!\n적이 발사한 총알은 공간의 끝에 닿으면 튕겨 나와.\n바닥을 회전시켜 총알의 방향을 바꾸고, 그 총알로 적을 처치해 봐!");
            yield return null;

            // 적을 처치할 때까지 대기
            // yield return new WaitUntil(() => plane.IsDead);

            writer.ShowText("잘했어!\n적 중에는 방어구를 착용한 적도 있으니 조심해야 해.");
            ClickTutorial();
            yield return null;

            writer.ShowText("마지막으로 아이템에 대해 알려줄게.");
            ClickTutorial();
            yield return null;

            itemPlane.CallPlane();

            writer.ShowText("가끔 위에서 보급 물자가 떨어질 거야.\n획득한 아이템은 Q 또는 E 키로 사용할 수 있어.");
            ClickTutorial();
            yield return null;

            plane.CallPlane();

            writer.ShowText("이제 다시 한번 적을 처치하고,\n본격적으로 시작해 보자!");
            ClickTutorial();
            
        }
    }
}