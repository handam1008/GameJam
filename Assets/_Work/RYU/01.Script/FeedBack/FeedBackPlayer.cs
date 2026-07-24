using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Work.RYU._01.Script.FeedBack
{
    public class FeedBackPlayer : MonoBehaviour
    {
        List<AbstractFeedBack> feedBacks = new List<AbstractFeedBack>();

        private void Awake()
        {
            feedBacks = GetComponents<AbstractFeedBack>().ToList();
        }

        public void PlayAllFeedBack()
        {
            feedBacks.ForEach(x => x.CreateFeedBack());
        }

        public void StopAllFeedBack()
        {
            feedBacks.ForEach(x => x.StopFeedBack());
        }
    }
}