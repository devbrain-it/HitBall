using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts
{
    public class BouncingFix
    {
        private readonly LinearBounce lb;
        private          int          collisions;

        public BouncingFix(LinearBounce linearBounce)
        {
            lb = linearBounce;
        }

        /// <summary>
        /// Anzahl der Kollisionen, ab denen der FIX greifen soll
        /// </summary>
        public int Bounces { get; set; }

        /// <summary>
        /// Toleranz Winkel, bis zu der Winkel zu 45° als FIX Notwendigkeit gezählt wird.
        /// <example>(positiv) 0 bis n</example>
        /// </summary>
        public float Tolerance { get; set; }

        /// <summary>
        /// Korrektur des Winkels, sobald Anzahl von Bounces erreicht wurde
        /// </summary>
        public AngelFix Fix { get; set; }

        /// <summary>
        /// Winkel zu Beginn
        /// </summary>
        public float StartAngle { get; set; }

        public void NotifyCollided()
        {
            var angle         = LinearBounce.ToBounceAngle(lb.Force.velocity);
            var absAngle      = Math.Abs(angle);
            var startAngleFix = Math.Abs(StartAngle)    > 0;
            var needFix       = Math.Abs(absAngle % 45) < Tolerance || startAngleFix;
            if (needFix)
            {
                Debug.Log(string.Format("Collision angle: {0}", absAngle));

                if (startAngleFix)
                {
                    lb.AdditionAngle = StartAngle;
                    lb.FixReflectionAngleCollision();
                    lb.Update();
                    StartAngle = 0;
                }
                else
                {
                    collisions++;

                    if (collisions >= Bounces)
                    {
                        Debug.Log(string.Format("Collision {0}", Bounces));
                        angle            = Fix(angle);
                        lb.AdditionAngle = angle;
                        collisions       = 0;
                    }
                }
            }
            else
            {
                collisions = 0;
            }
        }
    }

    public delegate float AngelFix(float angle);
}