using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.scripts.clip
{
    public class ParticleAnimationClip : IPlayableClip
    {
        private bool          started;
        private float         runtime;
        private List<Emitter> emitters;
        private Color         lastTintColor;

        public ParticleAnimationClip(ParticleSystem particles)
        {
            Particles = particles;
        }

        public string Name { get; set; }

        public ParticleSystem Particles { get; }

        public bool IsPlaying => started && runtime < Particles.main.duration;

        public Color TintColor { get; set; }

        public void Play()
        {
            emitters = CollectEmitters(Particles);
            UpdateColors();

            started = true;
            runtime = 0;
            Particles.Restart();
        }

        private static List<Emitter> CollectEmitters(ParticleSystem root)
        {
            var systems = new List<Emitter>
                          {
                              ConvertToEmitter(root)
                          };

            var particles = root.GetComponentsInChildren<ParticleSystem>();
            foreach (var subParticleSys in particles)
            {
                if (subParticleSys == root) continue;
                systems.Add(ConvertToEmitter(subParticleSys));
                systems.AddRange(CollectEmitters(subParticleSys));
            }

            return systems;
        }

        public void Stop()
        {
            started = false;
            runtime = 0;
            Particles.Stop(true);
        }

        public void Update()
        {
            runtime += Time.deltaTime;
        }

        private void UpdateColors()
        {
            if (!Equals(TintColor, lastTintColor))
            {
                lastTintColor = TintColor;
                foreach (var emitter in emitters)
                {
                    var main        = emitter.ParticleSystem.main;
                    main.startColor = ColorizeScript.AdjustHueColor(TintColor, emitter.InitColor);
                }
            }
        }

        private static Emitter ConvertToEmitter(ParticleSystem particleSystem)
        {
            return new Emitter
                   {
                       ParticleSystem = particleSystem,
                       InitColor      = particleSystem.main.startColor.color
                   };
        }

        struct Emitter
        {
            public ParticleSystem ParticleSystem { get; set; }
            public Color          InitColor      { get; set; }
        }
    }
}