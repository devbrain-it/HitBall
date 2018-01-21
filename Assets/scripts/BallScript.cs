using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts
{
    public class BallScript : Physical2D, IHit
    {
        public const string TAG = "Ball";

        private BouncingFix fix;
        public  double      HitForce = 1;
        public  GameObject  HitPrefab;
        private GameObject  spawnParent;

        protected override void AfterStart()
        {
            fix = new BouncingFix(lb)
                  {
                      Bounces    = 4,
                      Tolerance  = 0.5f,
                      Fix        = RandomFixAngle,
                      StartAngle = RandomFixAngle(0)
                  };
            fix.NotifyCollided();
            spawnParent = GameObject.Find("pointsfx");
        }

        public static float RandomFixAngle(float angle)
        {
            return angle + Random.Range(1, 179);
        }

        protected override void AfterUpdate()
        {
            if (Vector2.Distance(transform.position, Vector2.zero) > 100)
            {
                var position = SpawnHelper.RandomPositionInArea(GameScript.Game.Spawnarea);
                if (position != null)
                {
                    transform.position = position.Value;
                }
                else
                {
                    transform.position = Vector3.zero;
                }
                lb.Force.AddTorque(Random.Range(0, 360));
            }
        }

        protected override void BeforeCollision(Collision2D coll)
        {
            fix.NotifyCollided();
        }

        protected override void AfterCollision(Collision2D coll)
        {
            if (HitPrefab != null && coll.gameObject.tag.Equals(BarrierScript.TAG))
            {
                SpawnHitEffect();
            }
        }

        private void SpawnHitEffect()
        {
            var textEffect = Instantiate(HitPrefab, gameObject.transform.position, Quaternion.identity);
            SpawnHelper.SetParentInHierarchy(textEffect, spawnParent);
            textEffect.transform.position = gameObject.transform.position;
            textEffect.GetComponentInChildren<HitTextScript>().SetHit(HitForce);
            Destroy(textEffect, 0.5f);
        }

        public Hit GetHitForce()
        {
            return Hit.FromFullLife(HitForce);
        }
    }
}