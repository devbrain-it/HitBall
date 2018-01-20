using UnityEngine;

namespace Assets.scripts
{
    public class AnimationClip : IPlayableClip
    {
        private readonly Animator animation;

        public AnimationClip(Animator anim, string animationName)
        {
            animation = anim;
            Name      = animationName;
            Layer     = 0;
        }

        public string Name  { get; set; }
        public int    Layer { get; set; }

        public void Play()
        {
            animation.enabled = true;
            animation.Play(Name, Layer);
        }

        public void Stop()
        {
            animation.enabled = false;
        }
    }
}