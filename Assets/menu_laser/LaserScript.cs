using System.Collections.Generic;
using System.Linq;
using Assets.scripts;
using UnityEngine;

namespace Assets.menu_laser
{
    // TODO ANIMATE Random Points in LineRenderer
    // TODO Design Laser Image
    // TODO Add Particles an Laser Emitters (spheres)
    // TODO Show Collisions with Barriers and Balls
    // TODO Apply friction on speed once, when collide with Barriers
    // TODO Hit Barriers with Force to destroy them over time
    // TODO Define when Laser can be used
    // TODO Animate to show laser
    // TODO Animate to hide laser
    // TODO Add Particle Effects flowing aligned with laser beam (and gravity effect)
    // TODO Define different laser colors for different forces
    // TODO Define different lasers with different speed
    // TODO Style laser 3D elements cooler
    // TODO Add dripping effects to laser line on collision spots at barriers (melting effect)
    // TODO Barriers cannot explode with burst, when destroyed by laser (its a cut, no heating)
    // TODO Show Warning before laser activity with alert (dramatic illusion)

    public class LaserScript : MonoBehaviour
    {
        [Header("UI")]public LineRenderer  LaserBody;
        public               TriggerScript Laser;
        public               TriggerScript EndTrigger;

        [Range(0,     5)]    public float LineWidthMin;
        [Range(0,     5)]    public float LineWidthMax;
        [Range(3,     10000)]public int   RandomElements = 8;
        [Range(0.01f, 0.5f)] public float RandomIntervalMin;
        [Range(0.01f, 0.5f)] public float RandomIntervalMax;

        [Header("Effect")]public double              ForcePerSecond = 10;
        public                   Vector2             Speed;
        public                   float               Friction = 0.01f;
        private                  Vector3             startPosition;
        private                  HashSet<BarrierHit> barrierHits;

        public bool Active { get; private set; }

        public void SetStartPosition(Vector3 pos)
        {
            startPosition = pos;
        }

        void Start()
        {
            startPosition             =  transform.localPosition;
            barrierHits               =  new HashSet<BarrierHit>();
            Laser.TriggeredEvent      += OnLaserCollider2DTriggered;
            Laser.TriggeredExitEvent  += OnLaserCollider2DExit;
            Laser.TriggeredStayEvent  += OnLaserCollider2DStay;
            EndTrigger.TriggeredEvent += OnEndCollider2DTriggered;

            // demo
            // ActivateLaser();
        }

        void Update()
        {
            if (Active)
            {
                MoveBySpeed();
            }
            else
            {
                // keep at start
                transform.localPosition = startPosition;
            }
        }

        private void OnLaserCollider2DStay(TriggerScript laser, Collider2D collidedWith)
        {
            BarrierScript b;
            if (Active && (b = collidedWith.GetComponent<BarrierScript>()) != null)
            {
                var hit = barrierHits.FirstOrDefault(h => h.IsBarrier(b));
                if (hit != null)
                {
                    hit.Update();
                    if (hit.Runtime >= Random.Range(RandomIntervalMin, RandomIntervalMax))
                    {
                        b.DoDemage(Hit.FromFullLife(ForcePerSecond));
                        hit.ResetRuntime();
                    }
                }
            }
        }

        private void OnLaserCollider2DExit(TriggerScript laser, Collider2D exitFrom)
        {
            BarrierScript b;
            if (Active && (b = exitFrom.GetComponent<BarrierScript>()) != null)
            {
                barrierHits.RemoveWhere(h => h.IsBarrier(b));
            }
        }

        private void OnLaserCollider2DTriggered(TriggerScript laser, Collider2D collidedWith)
        {
            Debug.Log($"A/{laser.gameObject.name}.OnTriggerEnter2D triggered by: {collidedWith.gameObject.name}");
            BarrierScript b;
            if (Active && (b = collidedWith.GetComponent<BarrierScript>()) != null)
            {
                barrierHits.Add(new BarrierHit(b));
            }
        }

        private void OnEndCollider2DTriggered(TriggerScript end, Collider2D collidedWith)
        {
            if (Active)
            {
                //Debug.Log($"B/{end.gameObject.name}.OnTriggerEnter2D triggered by: {collidedWith.gameObject.name}");
                if (collidedWith.gameObject.tag == Laser.gameObject.tag)
                {
                    Reactivate();
                }
            }
        }

        private void MoveBySpeed()
        {
            transform.Translate(new Vector3(Speed.x, Speed.y, 0) * Time.deltaTime);
        }

        private void Reactivate()
        {
            StopLaser();
            ActivateLaser();
        }

        public void ActivateLaser()
        {
            Active = true;
            UpdateLaserRuntimeState();
        }

        public void StopLaser()
        {
            Active                  = false;
            transform.localPosition = startPosition;
            UpdateLaserRuntimeState();
        }

        private void UpdateLaserRuntimeState()
        {
            LaserBody.gameObject.SetActive(Active);
        }
    }

    public class BarrierHit : IUpdate
    {
        private          float         runtime;
        private readonly BarrierScript barrier;

        public BarrierHit(BarrierScript barrier)
        {
            runtime      = Time.time;
            this.barrier = barrier;
        }

        public float Runtime => runtime;

        public void ResetRuntime()
        {
            runtime = 0;
        }

        public void Update()
        {
            runtime += Time.deltaTime;
        }

        public bool IsBarrier(BarrierScript script)
        {
            return barrier == script;
        }
    }
}