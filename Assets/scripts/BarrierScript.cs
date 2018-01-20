using System;
using Assets.scripts.fx;
using TMPro;
using UnityEngine;

namespace Assets.scripts
{
    [RequireComponent(typeof(ExplosionScript))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BarrierScript : MonoBehaviour
    {
        public const string TAG = "Barrier";

        public double Life
        {
            get { return life; }
            set
            {
                life     = value;
                FullLife = value;
            }
        }

        public TextMeshPro    LifepointsText;
        public PlayAnimation  PlayAnim;
        public ColorizeScript ColorizerLife;
        public double         life;
        public double         FullLife;
        public float          ClickRepeatDurationSeconds = MouseHelper.DEFAULT_CLICK_REPEAT_DURATION_SECONDS;

        private DelayClick  mouseLeftDelay;

        void Start()
        {
            mouseLeftDelay = new DelayClick
                             {
                                 DelaySeconds     = ClickRepeatDurationSeconds,
                                 MouseButtonIndex = DelayClick.MOUSE_LEFT_BUTTON,
                                 ClickEvent       = OnMouseLeftDown
                             };
            UpdateText();
            UpdateLifeColorProgress();
        }

        void Update()
        {
            mouseLeftDelay.Check();

            UpdateText();
        }

        protected virtual void OnMouseLeftDown()
        {
            var    b      = GameObject.FindGameObjectsWithTag("Respawn");
            Action attack = () => hit(PlayerScript.Player.GetClickHitForce());
            if (b != null)
            {
                MouseHelper.PerformOnValidTargetObject(gameObject, attack, b);
            }
            else
            {
                MouseHelper.PerformOnValidTargetObject(gameObject, attack);
            }
        }

        void OnCollisionEnter2D(Collision2D coll)
        {
            var hitComponent = coll.gameObject.GetComponent<IHit>();
            if (hitComponent != null)
            {
                hit(hitComponent.GetHitForce());
            }
            else
            {
                Debug.Log("Explosion!!!!");

                if (coll.gameObject.CompareTag(ExplosionScript.TAG))
                {
                }
            }
        }

        public void UpdateText()
        {
            LifepointsText.text = Hit.FromFullLife(Life).ToString();
        }

        private void hit(Hit hit)
        {
            if (GameScript.Game.LoadingPanel.gameObject.activeSelf)
            {
                return;
            }

            //Debug.Log(string.Format("Hit BarrierScript: {0}", hit.FullLife));
            var h = Hit.FromFullLife(Life);
            h     -= hit;
            life  =  h.FullLife;
            UpdateLifeColorProgress();

            var hitAnimation = PlayAnim;
            if (hitAnimation != null)
            {
                hitAnimation.Play();
            }

            var destroy = h.IsZero;
            if (destroy)
            {
                GetComponent<CircleCollider2D>().enabled = false;

                Explode();

                Destroy(gameObject);
            }

            PlayerScript.Player.Money += hit;

            NotifyHit(destroy);
        }

        private void Explode()
        {
            var explosionDie = GetComponent<ExplosionScript>();

            var explosion                = Instantiate(explosionDie.explosionPrefab, transform.position, explosionDie.explosionPrefab.transform.rotation);
            var explosionScript          = explosion.GetComponent<ExplosionScript>();
            var radius                   = GetComponent<CircleCollider2D>().radius;
            explosionScript.MiniumRadius = Math.Max(1, radius);
            explosionScript.TotalRadius  = 5;
            explosionScript.Explode();
        }

        private void UpdateLifeColorProgress()
        {
            if (ColorizerLife != null)
            {
                ColorizerLife.Progress = CalculateLifeProgress();
            }
        }

        private double CalculateLifeProgress()
        {
            try
            {
                return Life / FullLife;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static void NotifyHit(bool destroyed)
        {
            var game = GameScript.Game;
            if (game != null)
            {
                game.Hit(destroyed);
            }
        }
    }
}