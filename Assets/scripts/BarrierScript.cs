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

        private DelayClick      mouseLeftDelay;
        private ExplosionScript explosion;
        private bool            destroyed;

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
            explosion = GetComponent<ExplosionScript>();
            explosion.Init();
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
            var    b      = GameObject.FindGameObjectsWithTag("Respawn");
            Action attack = () => DoDemage(PlayerScript.Player.GetClickHitForce());
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
                DoDemage(hitComponent.GetHitForce());
            }
            else
            {
                Debug.LogWarning("Barrier hit by explosion!!!!");

                if (coll.gameObject.CompareTag(ExplosionScript.TAG))
                {
                }
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
                destroyed = true;
            }

            PlayerScript.Player.AddMoney(remainingPower);

            NotifyHit(destroy);
        }

        private void Explode(double remainingPower)
        {
            LifepointsText.enabled = false;

            // collider wird zum Trigger beim Aufruf von ExplodeAsTrigger
            var minRadius = GetComponent<CircleCollider2D>().radius;

            // endgültig! -> Dieses gameObject wird endgültig zerstört durch diesen Aufruf
            explosion.ExplodeAsTrigger(minRadius, remainingPower);
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