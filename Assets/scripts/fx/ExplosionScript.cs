using System;
using Assets.scripts.clip;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.fx
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExplosionScript : MonoBehaviour
    {
        public const string TAG = "ExplosionBurst";

        [Header("Particle Effect")]public ParticleSystem Particles;
        public                            bool           RandomParticleColor = true;
        public                            Color          ParticleColor;
        [Header("Effector")]public        AnimationCurve ExplosionRadiusChange;
        public                            float          TotalRadius = 10;
        public                            LineRenderer   BurstExplosionRenderer;
        public                            int            BurstElements = 16;
        public                            Gradient       BurstLifeTimeColor;
        [Range(0.5f, 10f)]public          float          BurstWidthStart       = 0.5f;
        public                            double         BurstMinimumForce     = 1;
        public                            Color          AdditiveFallbackColor = Color.black;
        public                            ColorBehavior  BurstColorMixMode     = ColorBehavior.RANDOM_COLOR;
        public                            GameObject     SpawnParent;

        private float            miniumRadius;
        private CircleCollider2D effector;
        private float            time;
        private bool             runExplosion;
        private AnimationGroup   animations;
        private float            progress;
        private double           power;
        private Color            randomColor;
        public  float            Radius;

        public float Effect { get; private set; }

        private ParticleAnimationClip ParticleClip
        {
            get { return (ParticleAnimationClip) animations[0]; }
        }

        void Start()
        {
            //Debug.LogWarning("Explosion Start");
            BurstExplosionRenderer.enabled = false;
            BurstExplosionRenderer.loop    = true;
        }

        public void Play()
        {
            power = Math.Max(1, BurstMinimumForce);

            var particle = Particles;
            if (particle == null)
            {
                Debug.LogWarning("No Explosion Particles to clone");
                return;
            }

            if (effector == null)
            {
                effector        = GetComponent<CircleCollider2D>();
                effector.radius = Radius;
            }

            var animationGroup = initialize(effector.radius, particle);

            if (RandomParticleColor)
            {
                StartExplosion(animationGroup, rndColor:true);
            }
            else
            {
                StartExplosion(animationGroup, ParticleColor);
            }
        }

        void Update()
        {
            if (runExplosion)
            {
                animations.Update();

                time     += Time.deltaTime;
                Effect   =  ExplosionRadiusChange.Evaluate(Mathf.Clamp01(time));
                progress =  Effect;

                PaintCircle();

                DoExplosionDamage(transform.position, effector.radius);

                var clip = ParticleClip;
                clip.Particles.transform.localScale.Set(1, 1, 1);
                clip.Particles.transform.lossyScale.Set(1, 1, 1);

                gameObject.transform.lossyScale.Set(1, 1, 1);
                gameObject.transform.localScale.Set(1, 1, 1);
            }
        }

        private void PrepareColliderAsBurstTrigger()
        {
            effector           = GetComponent<CircleCollider2D>();
            effector.isTrigger = true;
        }

        private AnimationGroup CreateAnimationControl(ParticleSystem particle)
        {
            animations             =  new AnimationGroup(this, false);
            animations.TimeUpEvent += onAnimationsOnTimeUpEvent;
            animations.Set(new ParticleAnimationClip(particle));
            return animations;
        }

        private void onAnimationsOnTimeUpEvent(AnimationGroup sender)
        {
            if (runExplosion)
            {
                Debug.LogWarning("Explosion End");
                runExplosion = false;

                var p = (ParticleAnimationClip) sender[0];
                if (p != null)
                {
                    Transform pp;
                    if ((pp = p.Particles.transform.parent) != null)
                    {
                        Destroy(pp.gameObject);
                    }
                    else
                    {
                        Destroy(p.Particles.gameObject);
                    }

                    //Destroy(gameObject);
                }
            }
        }

        private void CreateLineRendererForCircle()
        {
            BurstExplosionRenderer.startWidth = 1;
        }

        void DoExplosionDamage(Vector3 center, float radius)
        {
            var hitColliders = Physics2D.OverlapCircleAll(center, radius);
            var thisCollider = GetComponent<Collider2D>();

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == thisCollider) continue;

                var barrierScript = hitCollider.GetComponent<BarrierScript>();
                if (barrierScript != null)
                {
                    var dist = hitCollider.Distance(thisCollider);

                    // je weiter die barriere weg ist, desto weniger schaden nimmt sie
                    var f = 1 - Mathf.Clamp(dist.distance, 0, TotalRadius) / TotalRadius;

                    // je stärker die übertragene kraft ist um so mehr schaden bewirkt
                    // die explosion
                    var demage = Math.Max(f * power, BurstMinimumForce);
                    barrierScript.DoDemage(Hit.FromFullLife(demage), false);
                }
            }
        }

        private void UpdateRadius()
        {
            var remainingLength = TotalRadius - effector.radius;
            effector.radius     += Math.Max(0, remainingLength) * Time.deltaTime;
            progress            =  Mathf.Clamp01(progress);
        }

        private void PaintCircle()
        {
            UpdateRadius();

            DrawCircle();

            UpdateDemageWidth();

            Colorize();

            if (!BurstExplosionRenderer.enabled)
            {
                BurstExplosionRenderer.enabled = true;
            }
        }

        private void UpdateDemageWidth()
        {
            const float endWidth              = 0; // 0.013f;
            var         width                 = endWidth + (1 - progress) * (BurstWidthStart - endWidth);
            BurstExplosionRenderer.startWidth = width;
            BurstExplosionRenderer.endWidth   = BurstExplosionRenderer.startWidth;
        }

        private void DrawCircle()
        {
            const int qualityFactor              = 4;
            BurstExplosionRenderer.positionCount = BurstElements * qualityFactor;
            var offsetAng                        = 360f          / BurstExplosionRenderer.positionCount;
            var ang                              = 0f;
            var radius                           = effector.radius;

            for (var i = 0; i < BurstExplosionRenderer.positionCount; i++)
            {
                // winkel position bestimmen
                ang     += offsetAng;
                var rad = Mathf.PI / 180f * ang;
                var x   = radius   * Mathf.Cos(rad);
                var y   = radius   * Mathf.Sin(rad);

                BurstExplosionRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
        }

        private void Colorize()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var a              = spriteRenderer == null ? AdditiveFallbackColor : spriteRenderer.color;
            var b              = BurstLifeTimeColor.Evaluate(progress);

            var material = BurstExplosionRenderer.material;
            if (material != null)
            {
                switch (BurstColorMixMode)
                {
                    case ColorBehavior.MULTIPLY:
                        material.color = a * b;
                        break;
                    case ColorBehavior.ADDITIVE:
                        material.color = a + b;
                        break;
                    case ColorBehavior.SUBSTRACTED:
                        material.color = a - b;
                        break;
                    case ColorBehavior.RANDOM_COLOR:
                        material.color = b * randomColor;
                        break;
                    case ColorBehavior.RANDOM_COLOR_ADDITIVE:
                        material.color = b + randomColor;
                        break;
                    case ColorBehavior.RANDOM_COLOR_SUBSTRACTED:
                        material.color = randomColor - b;
                        break;
                    case ColorBehavior.RANDOM_COLOR_SUBSTRACTED_INV:
                        material.color = b - randomColor;
                        break;
                    case ColorBehavior.COLOR_OVERTIME_ONLY:
                        material.color = b;
                        break;
                    case ColorBehavior.FALLBACK_ONLY:
                        material.color = a;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private AnimationGroup initialize(float minRadius, ParticleSystem particle)
        {
            PrepareColliderAsBurstTrigger();

            var anim = CreateAnimationControl(particle);

            CreateLineRendererForCircle();

            miniumRadius    = minRadius;
            TotalRadius     = Math.Max(TotalRadius, miniumRadius);
            effector.radius = miniumRadius;
            Effect          = 0f;
            time            = 0;

            return anim;
        }

        private void StartExplosion(IPlayableClip animationGroup, Color? tintColorParticles = null, bool rndColor = false)
        {
            runExplosion = true;
            enabled      = true;

            randomColor = ColorizeParticles(ParticleClip, tintColorParticles, rndColor);

            animationGroup.Play();
        }

        private static Color ColorizeParticles(ParticleAnimationClip clip, Color? tintColorParticles, bool randomColor)
        {
            if (randomColor)
            {
                clip.TintColor = Random.ColorHSV(0, 1, 0.5f, 1, 1, 1, 1, 1);
            }
            else if (tintColorParticles.HasValue)
            {
                clip.TintColor = tintColorParticles.Value;
            }

            return clip.TintColor;
        }

        public void Init()
        {
            enabled = false;
        }
    }
}