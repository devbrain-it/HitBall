using UnityEngine;

namespace Assets.scripts.clip
{
    public class AnimationClip : IPlayableClip
    {
        private readonly Animator animation;
        private          float    playtime;

        public AnimationClip(Animator anim, string animationName)
        {
            animation = anim;
            Name      = animationName;
            Layer     = 0;
        }

        public string Name { get; set; }

        public int Layer { get; set; }

        public bool IsPlaying
        {
            get
            {
                if (!animation.enabled)
                {
                    return false;
                }

                var info = animation.GetCurrentAnimatorStateInfo(Layer);
                return info.IsName(Name) && (info.loop || playtime < info.length);
            }
        }

        public void Play()
        {
            animation.enabled = true;
            playtime          = 0f;
            #if UNITY_EDITOR
            Debug.Log($"AnimationClip.Play() Name: '{Name}' Layer: {Layer}");
            #endif
            animation.Play(Name, Layer);
        }

        public void Stop()
        {
            animation.enabled = false;
            playtime          = 0f;
        }

        public void Update()
        {
            if (animation.enabled)
            {
                playtime += Time.deltaTime;
            }
        }
    }
}