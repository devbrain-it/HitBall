using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.scripts
{
    public class AnimationGroup : IPlayableClip, IEnumerable<IPlayableClip>
    {
        public event Action TimeUpEvent;

        private          IPlayableClip[] clips;
        private          float           uptimeSeconds;
        private          float           endTimeSeconds;
        private readonly MonoBehaviour   owner;

        public AnimationGroup(MonoBehaviour script, float endTimeSeconds, bool initActiveScript)
        {
            owner                = script;
            EndTimeSeconds       = endTimeSeconds;
            BindActiveToPlayback = true;

            SetOwnerActive(initActiveScript);
        }

        public bool BindActiveToPlayback { get; set; }

        public void SetOwnerActive(bool active)
        {
            owner.gameObject.SetActive(active);
        }

        public float EndTimeSeconds
        {
            get { return endTimeSeconds; }
            set { endTimeSeconds = Math.Max(0, value); }
        }

        public IList<IPlayableClip> Clips => clips?.ToList() ?? new List<IPlayableClip>();

        public IPlayableClip this[int index] => Clips[index];

        public string Name { get; set; }

        public bool Enabled { get; private set; }

        public void Set(params IPlayableClip[] clipsToSet)
        {
            clips = clipsToSet;
        }

        public void Play()
        {
            if (!Enabled)
            {
                // seconds, then enabled = true
                uptimeSeconds = 0f;
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
                uptimeSeconds += Time.deltaTime;
                if (uptimeSeconds >= EndTimeSeconds)
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

            uptimeSeconds = 0;
            Enabled       = false;

            TimeUpEvent?.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}