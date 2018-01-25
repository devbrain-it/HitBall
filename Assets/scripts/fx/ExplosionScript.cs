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

        [Header("Particle Effect")]public ParticleSystem ExplosionPrefab;
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

        private float            miniumRadius;
        private CircleCollider2D effector;
        private float            time;
        private bool             runExplosion;
        private AnimationGroup   animations;
        private float            theta;
        private float            thetaScale = 1;
        private ParticleSystem   particles;
        private float            progress;
        private double           power;
        private Color            randomColor;

        public float Effect { get; private set; }

        void Start()
        {
            Debug.LogWarning("Explosion Start");
            BurstExplosionRenderer.enabled = false;
            BurstExplosionRenderer.loop    = true;
        }

        private void PrepareColliderAsBurstTrigger()
        {
            effector           = GetComponent<CircleCollider2D>();
            effector.isTrigger = true;
        }

        private void CreateAnimationControl()
        {
            animations             =  new AnimationGroup(this, false);
            animations.TimeUpEvent += onAnimationDone;
            animations.Set(new ParticleAnimationClip(particles));
        }

        private void CreateLineRendererForCircle()
        {
            BurstExplosionRenderer.startWidth = 1;
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
            }
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
                    //demage     = power;
                    transform.lossyScale.Scale((float) demage * Vector3.one * (1 - f));
                    barrierScript.DoDemage(Hit.FromFullLife(demage), false);
                }
            }
        }

        private void PaintCircle()
        {
            var remainingLength = TotalRadius - effector.radius;
            effector.radius     += remainingLength * Time.deltaTime;
            effector.radius     =  Mathf.Clamp(effector.radius, 0, TotalRadius);
            progress            =  Mathf.Clamp01(progress);

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

            const float endWidth              = 0; // 0.013f;
            var         width                 = endWidth + (1 - progress) * (BurstWidthStart - endWidth);
            BurstExplosionRenderer.startWidth = width;
            BurstExplosionRenderer.endWidth   = BurstExplosionRenderer.startWidth;

            var spriteRenderer = GetComponent<SpriteRenderer>();
            var a              = spriteRenderer == null ? AdditiveFallbackColor : spriteRenderer.color;
            var b              = BurstLifeTimeColor.Evaluate(progress);
            switch (BurstColorMixMode)
            {
                case ColorBehavior.MULTIPLY:
                    BurstExplosionRenderer.material.color = a * b;
                    break;
                case ColorBehavior.ADDITIVE:
                    BurstExplosionRenderer.material.color = a + b;
                    break;
                case ColorBehavior.SUBSTRACTED:
                    BurstExplosionRenderer.material.color = a - b;
                    break;
                case ColorBehavior.RANDOM_COLOR:
                    BurstExplosionRenderer.material.color = b * randomColor;
                    break;
                case ColorBehavior.RANDOM_COLOR_ADDITIVE:
                    BurstExplosionRenderer.material.color = b + randomColor;
                    break;
                case ColorBehavior.RANDOM_COLOR_SUBSTRACTED:
                    BurstExplosionRenderer.material.color = randomColor - b;
                    break;
                case ColorBehavior.RANDOM_COLOR_SUBSTRACTED_INV:
                    BurstExplosionRenderer.material.color = b - randomColor;
                    break;
                case ColorBehavior.COLOR_OVERTIME_ONLY:
                    BurstExplosionRenderer.material.color = b;
                    break;
                case ColorBehavior.FALLBACK_ONLY:
                    BurstExplosionRenderer.material.color = a;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!BurstExplosionRenderer.enabled)
            {
                BurstExplosionRenderer.enabled = true;
            }
        }

        private void onAnimationDone()
        {
            if (runExplosion)
            {
                Debug.LogWarning("Explosion End");
                runExplosion = false;

                //Destroy(gameObject.transform.parent);
                Destroy(particles);
            }
        }

        public void ExplodeAsTrigger(float minRadius, double hitForceLife)
        {
            if (power > 0) return;

            power = Math.Max(1, hitForceLife);

            var source = ExplosionPrefab;
            if (source == null)
            {
                Debug.LogWarning("No Explosion Prefab to clone");
                return;
            }

            CreateExplosionFromPrefab(source);

            initialize(minRadius);

            if (RandomParticleColor)
            {
                StartExplosion(randomColor:true);
            }
            else
            {
                StartExplosion(ParticleColor);
            }
        }

        private void CreateExplosionFromPrefab(ParticleSystem source)
        {
            // location and direction
            var position = transform.position;
            var rotation = source.transform.rotation;

            // create
            particles = Instantiate(source, position, rotation);
            SpawnHelper.SetParentInHierarchy(particles.gameObject, gameObject);
        }

        private void initialize(float minRadius)
        {
            PrepareColliderAsBurstTrigger();

            CreateAnimationControl();

            CreateLineRendererForCircle();

            miniumRadius    =  minRadius;
            TotalRadius     += miniumRadius;
            effector.radius =  miniumRadius;
            Effect          =  0f;
            time            =  0;
        }

        private void StartExplosion(Color? tintColorParticles = null, bool randomColor = false)
        {
            runExplosion = true;
            enabled      = true;

            this.randomColor = ColorizeParticles((ParticleAnimationClip) animations[0], tintColorParticles, randomColor);

            animations.Play();
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