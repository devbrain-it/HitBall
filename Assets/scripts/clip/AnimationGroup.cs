using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.scripts.clip
{
    public class AnimationGroup : IPlayableClip, IEnumerable<IPlayableClip>
    {
        public event Action TimeUpEvent;

        private          IPlayableClip[] clips;
        private readonly MonoBehaviour   owner;

        public AnimationGroup(MonoBehaviour script, bool initActiveScript)
        {
            owner                = script;
            BindActiveToPlayback = true;

            SetOwnerActive(initActiveScript);
        }

        public bool BindActiveToPlayback { get; set; }

        public void SetOwnerActive(bool active)
        {
            owner.enabled = active;
            owner.gameObject.SetActive(active);
        }
        
        public IList<IPlayableClip> Clips => clips?.ToList() ?? new List<IPlayableClip>();

        public IPlayableClip this[int index] => Clips[index];

        public string Name { get; set; }

        public bool Enabled { get; private set; }

        public bool IsPlaying => Enabled && Clips.Any(c => c.IsPlaying);

        public void Set(params IPlayableClip[] clipsToSet)
        {
            clips = clipsToSet;
        }

        public void Play()
        {
            if (!Enabled)
            {
                Enabled = true;

                if (BindActiveToPlayback)
                {
                    SetOwnerActive(true);
                }

                foreach (var clip in Clips)
                {
                    clip.Play();
                }
            }
        }

        public void Update()
        {
            if (Enabled)
            {
                foreach (var clip in Clips)
                {
                    clip.Update();
                }

                if (!IsPlaying)
                {
                    TriggerEnd();
                }
            }
        }

        public void Stop()
        {
            if (Enabled)
            {
                foreach (var clip in Clips)
                {
                    clip.Stop();
                }
            }

            if (BindActiveToPlayback)
            {
                SetOwnerActive(false);
            }
        }

        public IEnumerator<IPlayableClip> GetEnumerator()
        {
            return Clips.GetEnumerator();
        }

        public void TriggerEnd()
        {
            Stop();

            Enabled       = false;

            TimeUpEvent?.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}