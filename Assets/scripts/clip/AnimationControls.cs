using System;
using System.Collections;
using UnityEngine;

namespace Assets.scripts.clip
{
    public class AnimationControls : MonoBehaviour
    {
        private readonly Animator anim;


        public AnimationControls(Animator animator)
        {
            this.anim = animator;
        }
        
        protected void SetTrue(string exitParameter, Action callback)
        {
            var info = new RoutineInfo
                       {
                           TrueParameter  = exitParameter,
                           FalseParameter = exitParameter,
                           Callback       = callback
                       };

            const string methodName = nameof(Exit);
            StopCoroutine(methodName);
            StartCoroutine(methodName, info);
        }

        IEnumerator Exit(RoutineInfo info)
        {
            // trigger einen neuen State "andere Animation wird gestartet"
            anim.SetBool(info.TrueParameter, true);

            // warte bis das umgesetzt ist
            yield return new WaitUntil(() => anim.GetBool(info.TrueParameter) && checkAnimationRunning(0));

            // warte bis die Animation vorbei ist
            while (checkAnimationRunning(0))
            {
                yield return null;
            }

            // schalte die Animation wieder ab
            anim.SetBool(info.FalseParameter, false);

            // warte bis das umgesetzt ist
            yield return new WaitUntil(() => !anim.GetBool(info.TrueParameter));

            // was soll nach der Animation passieren
            // -- canvas.setActive(false)
            info.Callback?.Invoke();
        }

        public bool checkAnimationRunning(int layer)
        {
            return anim.GetAnimatorTransitionInfo(layer).normalizedTime < 1 ||
                   anim.IsInTransition(layer);
        }
    }

    internal struct RoutineInfo
    {
        public Action Callback       { get; set; }
        public string TrueParameter  { get; set; }
        public string FalseParameter { get; set; }
    }

    // setup animator
    // put different animations as new clips into animator
    // setup transitions between animations
    // setup paramters (bool) to transit conditional between wait
    // [Entry] /when await active=false -> active=true / ==> FADE IN ANIMATION ===[ parameter 'e.g. exit': bool == true ]====> FADE OUT ANIMATION ==> EMPTY STATE 'WAIT here' ===[ parameter 'e.g. exit': bool == false ] ===> FADE IN ANIMATION

    // 2 clips
    // - fade in (no loop)
    // - fade out (no loop)
    // 1 empty state
    // 1 parameter
    // - bool: exit
    // 4 transitions
    // - entry -> fade in
    // - fade in -> fade out (exit = true, has exit time = false)
    // - fade out -> empty 'wait' (has exit time = true)
    // - empty 'wait' -> fade in (exit = false, has exit time = false)
}