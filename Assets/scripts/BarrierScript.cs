using System;
using Assets.scripts.fx;
using TMPro;
using UnityEngine;

namespace Assets.scripts
{
    [RequireComponent(typeof(ExplosionDestroyScript), typeof(Rigidbody2D))]
    public class BarrierScript : MonoBehaviour
    {
        public const string TAG   = "Barrier";
        public const string LAYER = "Entity";

        public TextMeshPro            LifepointsText;
        public PlayAnimation          PlayAnim;
        public ColorizeScript         ColorizerLife;
        public double                 life;
        public double                 FullLife;
        public float                  ClickRepeatDurationSeconds = MouseHelper.DEFAULT_CLICK_REPEAT_DURATION_SECONDS;
        public GameObject             ExplosionParent;
        public ExplosionDestroyScript Explosion;

        private DelayClick mouseLeftDelay;
        private bool       destroyed;

        public double Life
        {
            get { return life; }
            set
            {
                life     = value;
                FullLife = value;
            }
        }

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
            if (destroyed)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                return;
            }

            mouseLeftDelay.Check();

            UpdateText();
        }

        protected virtual void OnMouseLeftDown()
        {
            var b = GameObject.FindGameObjectsWithTag("Respawn");
            if (b != null)
            {
                MouseHelper.PerformOnValidTargetObject(gameObject, attack, b);
            }
            else
            {
                MouseHelper.PerformOnValidTargetObject(gameObject, attack);
            }
        }

        private void attack() => DoDemage(PlayerScript.Player.GetClickHitForce());

        void OnCollisionEnter2D(Collision2D coll)
        {
            var hitComponent = coll.gameObject.GetComponent<IHit>();
            if (hitComponent != null && !destroyed)
            {
                DoDemage(hitComponent.GetHitForce());
            }
        }

        public void UpdateText()
        {
            LifepointsText.text = Hit.FromFullLife(Life).ToString();
        }

        public void DoDemage(Hit hit, bool showHitAnimation = true)
        {
            if (GameScript.Game.LoadingPanel.gameObject.activeSelf)
            {
                return;
            }

            //Debug.Log(string.Format("Hit BarrierScript: {0}", hit.FullLife));
            var remainingPower = Math.Max(1, hit.FullLife - Life);
            var h              = Hit.FromFullLife(Life);
            h                  -= hit;
            life               =  h.FullLife;
            UpdateLifeColorProgress();

            var hitAnimation = PlayAnim;
            if (hitAnimation != null && showHitAnimation)
            {
                hitAnimation.Play();
            }

            var destroy = h.IsZero;
            if (destroy)
            {
                // entfernen!
                //GetComponent<CircleCollider2D>().enabled = false;

                Explode(remainingPower);
            }

            PlayerScript.Player.AddMoney(remainingPower);

            NotifyHit(destroy);
        }

        private void Explode(double remainingPower)
        {
            if (!destroyed)
            {
                destroyed = true;

                LifepointsText.enabled = false;

                // collider wird zum Trigger beim Aufruf von ExplodeAsTrigger
                var minRadius = GetComponent<CircleCollider2D>().radius;

                // endgültig! -> Dieses gameObject wird endgültig zerstört durch diesen Aufruf
                var data = new ExplosionDestroyScript.ExplosionData
                           {
                               Force      = remainingPower,
                               InitRadius = minRadius
                           };
                Explosion.Explode(ExplosionParent, data, transform.position);

                Destroy(this.gameObject);
            }
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