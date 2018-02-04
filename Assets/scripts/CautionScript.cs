using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace Assets.scripts
{
    public class CautionScript : MonoBehaviour
    {
        public Animator              Animator;
        public GameObject            Panel;
        public PostProcessingProfile PostProcess;
        public string                ExitParameter    = "exit";
        public bool                  VignetteOnActive = false;

        private bool awaiting;

        void Start()
        {
            MarkHidden();
        }

        private void MarkHidden()
        {
            SetVignet(false);
            Panel.SetActive(false);
            IsVisible = false;
        }

        public bool IsVisible { get; private set; }

        private void SetVignet(bool active)
        {
            PostProcess.vignette.enabled = active;
        }

        public void Show()
        {
            if (ShowingAllowed)
            {
                const string showMethod = nameof(TriggerShow);
                StopCoroutine(showMethod);
                StartCoroutine(showMethod);
            }
            else
            {
                const string waitMethod = nameof(AWaitForShow);
                StopCoroutine(waitMethod);
                StartCoroutine(waitMethod, new Action(Show));
            }
        }

        public bool ShowingAllowed
        {
            get { return !IsVisible && !awaiting; }
        }

        IEnumerator AWaitForShow(object callback)
        {
            yield return new WaitUntil(() => ShowingAllowed);

            var act = callback as Action;
            act?.Invoke();
        }

        IEnumerator TriggerShow()
        {
            awaiting      = true;
            ExitParameter = "exit";
            Panel.SetActive(true);
            PostProcess.vignette.enabled = true;
            SetVignet(VignetteOnActive);

            yield return new WaitForSeconds(0.15f);

            IsVisible = true;
            awaiting  = false;
        }

        private bool inAnimation(int layerIndex)
        {
            return Animator.IsInTransition(layerIndex) || Animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < 1;
        }

        public void Hide()
        {
            if (HideAllowed)
            {
                const string hideMethod = nameof(TriggerHide);
                StopCoroutine(hideMethod);
                StartCoroutine(hideMethod);
            }
            else
            {
                const string waitMethod = nameof(AWaitForHide);
                StopCoroutine(waitMethod);
                StartCoroutine(waitMethod);
            }
        }

        public bool HideAllowed
        {
            get { return IsVisible && !awaiting; }
        }

        IEnumerator AWaitForHide()
        {
            yield return new WaitUntil(() => HideAllowed);
        }

        IEnumerator TriggerHide()
        {
            awaiting = true;
            Animator.SetBool(ExitParameter, true);

            yield return new WaitForSeconds(0.15f);

            while (inAnimation(0))
            {
                yield return null;
            }

            Animator.SetBool(ExitParameter, false);
            yield return new WaitForSeconds(0.15f);

            MarkHidden();

            awaiting = false;
        }
    }
}