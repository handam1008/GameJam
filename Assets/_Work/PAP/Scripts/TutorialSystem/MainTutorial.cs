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
            writer.ShowText("안녕, 조작법을 알려줄게! 클릭하여 다음으로 넘어갈 수 있어.");
            ClickTutorial();
            yield return null;
            writer.ShowText("W , A , S , D 로 앞쪽, 왼쪽, 뒷쪽, 오른쪽으로 움직일 수 있어!");
            ClickTutorial();
            yield return null;
            writer.ShowText("Shift를 누르면 대시를 사용할 수 있어! 대시로 장애물들을 넘어 다닐 수 있어!");
            ClickTutorial();
            yield return null;
            writer.ShowText("너가 서있는 바닥이 보이지? 자 이제 마우스를 움직여봐!");
            ClickTutorial();
            yield return null;
            writer.ShowText("너의 마우스가 향하는 방향으로 우리의 공간이 회전되게 돼!");
            ClickTutorial();
            yield return null;
            plane.CallPlane();
            writer.ShowText("앗! 위험해 적이 나타났어,\n" +
                            "적이 쏘는 총알 공격은 벽에 닿으면 튕겨 날라와!\n" +
                            "회전되는 공간과 적이 쓰는 총알을 이용해 적을 쓰러뜨려줘!");
            yield return null;
            writer.ShowText("잘했어, 방어장비를 더 단단히한 친구도 있으니 조심하는게 좋을거야.");
            ClickTutorial();
            yield return null;
            writer.ShowText("다음은 아이템이야.");
            ClickTutorial();
            yield return null;
            writer.ShowText("때가 되면, 위에서 보급물자가 떨어질거야! 아이템을 먹어서 q나 e로 사용할 수 있어.");
            itemPlane.CallPlane();
            ClickTutorial();
            yield return null;
            plane.CallPlane();
            writer.ShowText("다시 한번 적을 잡고 넘어가보자!");
        }
    }
}