using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Work.RYU._01.Script.FeedBack
{
    public class TestFeedBack : MonoBehaviour
    {
        [SerializeField] private FeedBackPlayer _feedBackPlayer;

        private void Update()
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                _feedBackPlayer.PlayAllFeedBack();
            }
        }
    }
}