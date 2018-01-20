using UnityEngine;

namespace Assets.scripts
{
    public class PlayAnimation : MonoBehaviour
    {
        public Animator Animator;
        public string   AnimationName;

        public void Play()
        {
            Animator.Play(AnimationName);
        }
    }
}