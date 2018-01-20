using UnityEngine;

namespace Assets.scripts.fx
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExplosionScript : MonoBehaviour
    {
        public const string TAG = "ExplosionBurst";

        [Header("Visual Effect")]
        public ParticleSystem explosionPrefab;

        [Header("Effector")]
        public float DurationSeconds = 1;

        public AnimationCurve ExplosionRadiusChange;
        public float          TotalRadius  = 10;
        public float          MiniumRadius = 0;

        private CircleCollider2D effector;
        private float            time;
        private bool             runExplosion;

        public float Effect { get; private set; }

        void Start()
        {
        }

        void Update()
        {
            if (runExplosion)
            {
                time            += Mathf.Lerp(0, DurationSeconds, Time.deltaTime);
                var rFx         = ExplosionRadiusChange.Evaluate(time);
                effector.radius = MiniumRadius + rFx * (TotalRadius - MiniumRadius);
                Effect          = rFx;
                if (time >= DurationSeconds)
                {
                    runExplosion = false;
                    Destroy(gameObject, 1f);
                }
            }
        }

        public void Explode()
        {
            effector        = GetComponent<CircleCollider2D>();
            effector.radius = MiniumRadius;
            Effect          = 0f;
            time            = 0;
            runExplosion    = true;
            explosionPrefab.Simulate(0, true, true);
            explosionPrefab.Play();
        }
    }
}