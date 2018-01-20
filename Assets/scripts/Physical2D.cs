using System;
using UnityEngine;

namespace Assets.scripts
{
    public abstract class Physical2D : MonoBehaviour, ILinearBounce
    {
        public event Action<Collision2D> OnCollisionEvent;
        public event Action              OnUpdateEvent;

        public float Speed = 10;
        public float Mass  = 1;

        protected LinearBounce lb { get; private set; }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        void Start()
        {
            lb    =  new LinearBounce(GetComponent<Rigidbody2D>(), this);
            Speed *= 1 / Mass;
            lb.SetSpeed(Speed);
            lb.Force.mass = Mass;
            AfterStart();
        }

        protected abstract void AfterStart();

        protected virtual void BeforeUpdateSpeed()
        {
            lb.Speed = Speed;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            BeforeUpdateSpeed();
            var handle = OnUpdateEvent;
            if (handle != null) handle.Invoke();
            AfterUpdate();
        }

        protected abstract void AfterUpdate();

        void OnCollisionEnter2D(Collision2D coll)
        {
            BeforeCollision(coll);
            var handle = OnCollisionEvent;
            if (handle != null) handle.Invoke(coll);
            AfterCollision(coll);
        }

        protected abstract void BeforeCollision(Collision2D coll);

        protected abstract void AfterCollision(Collision2D coll);
    }
}