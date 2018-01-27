using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts
{
    public class CautionScript : MonoBehaviour
    {
        public Animator Animator;
        public string   TriggerFadeIn;
        public string   TriggerFadeOut;

        private float duration = 1;

        private FadingStory nextTrigger = FadingStory.HIDDEN;
        private float       runtime;

        public bool IsVisible
        {
            get { return nextTrigger == FadingStory.FADE_IN || nextTrigger != FadingStory.VISIBLE; }
        }

        void Start()
        {
            TriggerHide();
        }

        void Update()
        {
            switch (nextTrigger)
            {
                case FadingStory.FADE_IN:
                    TriggerShow();
                    break;
                case FadingStory.VISIBLE:
                    runtime += Time.deltaTime;
                    if (runtime >= duration)
                    {
                        TriggerHide();
                    }

                    break;
                case FadingStory.HIDDEN:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TriggerShow()
        {
            Animator.SetTrigger(TriggerFadeIn);
            Animator.ResetTrigger(TriggerFadeOut);
            nextTrigger = FadingStory.VISIBLE;
            runtime     = 0;
        }

        public void TriggerShow(TimeSpan span)
        {
            if (!IsVisible)
            {
                nextTrigger = FadingStory.FADE_IN;
                duration    = (float) span.TotalSeconds;
            }
        }

        public void TriggerHide()
        {
            if (IsVisible)
            {
                Animator.SetTrigger(TriggerFadeOut);
                Animator.ResetTrigger(TriggerFadeIn);
                nextTrigger = FadingStory.HIDDEN;
            }
        }
    }

    public enum FadingStory
    {
        HIDDEN,
        FADE_IN,
        VISIBLE
    }
}