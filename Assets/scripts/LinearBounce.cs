using System;
using UnityEngine;

namespace Assets.scripts
{
    public class LinearBounce
    {
        private readonly GameObject gameObject;

        public LinearBounce(Rigidbody2D rigidbody, ILinearBounce gameObjectScript)
        {
            gameObjectScript.OnUpdateEvent    -= Update;
            gameObjectScript.OnUpdateEvent    += Update;
            gameObjectScript.OnCollisionEvent -= OnCollisionEnter2D;
            gameObjectScript.OnCollisionEvent += OnCollisionEnter2D;

            Force      = rigidbody;
            gameObject = rigidbody.gameObject;
        }

        public Rigidbody2D Force { get; private set; }

        public float Speed         { get; set; }
        public float AdditionAngle { get; set; }

        public void Update()
        {
            KeepSpeed(Speed);
        }

        private void OnCollisionEnter2D(Collision2D obj)
        {
            FixReflectionAngleCollision();
        }

        public void SetSpeed(float speed)
        {
            Speed = speed;
            StopSpeed();
            AddSpeed(speed);
        }

        public void StopSpeed()
        {
            Force.velocity = Vector2.zero;
        }

        protected void AddSpeed(float speed)
        {
            Force.AddForce(gameObject.transform.TransformDirection(Vector2.right) * speed);
        }

        protected void KeepSpeed(float speed)
        {
            if (Math.Abs(AdditionAngle) > 0)
            {
                Debug.Log("Fix at update ...");
                var angle      = Force.rotation + AdditionAngle;
                Force.velocity = new Vector2(
                                             Mathf.Cos(angle) * speed,
                                             Mathf.Sin(angle) * speed);
                AdditionAngle = 0;
            }
            else
            {
                Force.velocity = Force.velocity.normalized * speed;
            }
        }

        public void FixReflectionAngleCollision()
        {
            Force.MoveRotation(ToBounceAngle(Force.velocity, AdditionAngle));
        }

        public static float ToBounceAngle(Vector2 velocity, float angle = 0f)
        {
            return Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + angle;
        }
    }
}