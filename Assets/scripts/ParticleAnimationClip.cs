using UnityEngine;

namespace Assets.scripts
{
    public class ParticleAnimationClip : IPlayableClip
    {
        public ParticleAnimationClip(ParticleSystem particles)
        {
            Particles = particles;
        }

        public string Name { get; set; }

        public ParticleSystem Particles { get; }

        public void Play()
        {
            Particles.Restart();
        }

        public void Stop()
        {
            Particles.Stop(true);
        }
    }
}