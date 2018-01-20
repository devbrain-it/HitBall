﻿using System;
using UnityEngine;

namespace Assets.scripts
{
    public class LevelUpScript : MonoBehaviour
    {
        [Header("Star")]public    Animator StarAnimation;
        public                    string   StarAnimationName;
        [Header("LevelUp")]public Animator LevelUpTextAnimation;
        public                    string   LevelUpAnimationName;
        [Header("Alpha")]public   Animator AlphaAnimation;
        public                    string   AlphaAnimationName;

        public  ParticleSystem Particles;
        private Action         doneCallback;
        private AnimationGroup animations;

        public GameObject Self => gameObject;

        void Start()
        {
            initOnce();
        }

        private void initOnce()
        {
            if (animations == null)
            {
                animations             =  new AnimationGroup(this, 2f, false);
                animations.TimeUpEvent += onAnimationsEnd;
                animations.Set(new AnimationClip(StarAnimation,        StarAnimationName),
                               new AnimationClip(LevelUpTextAnimation, LevelUpAnimationName),
                               new AnimationClip(AlphaAnimation,       AlphaAnimationName),
                               new ParticleAnimationClip(Particles));
            }
        }

        private void onAnimationsEnd()
        {
            doneCallback?.Invoke();
        }

        void Update()
        {
            animations.Update();
        }

        public void Play(Action callback)
        {
            initOnce();

            doneCallback = callback;
            animations.Play();
        }
    }
}