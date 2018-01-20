using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(menuName = "Create HitValue")]
    public class HitValue : ScriptableObject
    {
        [Header("Value")]
        public double FullLife;

        [Header("Calculated"), ReadOnly]public int    Multiplier;
        [ReadOnly]                      public double Value;
        [ReadOnly]                      public string Text;

        [UsedImplicitly]
        void OnEnable()
        {
            RenderValue(HitValueIn.FROM_FULL_LIFE);
        }

        public void RenderValue(HitValueIn mode)
        {
            switch (mode)
            {
                case HitValueIn.FROM_FULL_LIFE:
                    CalculateValueFromFullLife();
                    break;
                case HitValueIn.FROM_VALUE:
                    CalculateFullLifeFromValue();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }

            Text = string.Format("{0} {1}", Value.ToString("0.00").Trim('0'), GameManager.Instance.Units[Multiplier]).Trim();
        }

        private void CalculateFullLifeFromValue()
        {
            var i    = Multiplier;
            var life = Value;
            for (var j = i; j >= 0; j--)
            {
                life += 1000;
            }

            FullLife = life;
        }

        private void CalculateValueFromFullLife()
        {
            var i    = 0;
            var life = FullLife;
            while (life >= 1000)
            {
                i++;
                life -= 1000;
            }

            Multiplier = i;
            Value      = life;
        }
    }
}