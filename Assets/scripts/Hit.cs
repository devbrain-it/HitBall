using System;

namespace Assets.scripts
{
    public struct Hit
    {
        public const string UNITS = " aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";

        public double Life { get; set; }

        public int Unit { get; set; }

        public bool IsZero => Life <= 0 && Unit == 0;

        public double FullLife => getLongLife(this);

        public bool GreaterThan(Hit b)
        {
            var aLife = (double) getLongLife(this);
            var bLife = (double) getLongLife(b);
            return aLife - bLife > 0;
        }

        public bool GreaterThanOrEqual(Hit b)
        {
            var aLife = (double) getLongLife(this);
            var bLife = (double) getLongLife(b);
            return aLife - bLife >= 0;
        }

        public bool LowerThan(Hit b)
        {
            var aLife = (double) getLongLife(this);
            var bLife = (double) getLongLife(b);
            return aLife - bLife < 0;
        }

        public bool LowerThanOrEqual(Hit b)
        {
            var aLife = (double) getLongLife(this);
            var bLife = (double) getLongLife(b);
            return aLife - bLife <= 0;
        }

        public static Hit operator -(Hit a, Hit b)
        {
            var valA = a.FullLife;
            var valB = b.FullLife;
            if (valA < valB)
            {
                return FromFullLife(0);
            }

            double result = Math.Max(0, valA - valB);
            return FromFullLife(result);
        }

        public static Hit operator +(Hit a, Hit b)
        {
            var valA   = a.FullLife;
            var valB   = b.FullLife;
            var result = Math.Min(double.MaxValue, valA + valB);
            return FromFullLife(result);
        }

        private static ulong getLongLife(Hit a)
        {
            var valDbl = a.Life * Math.Pow(1000, a.Unit);
            return (ulong) valDbl;
        }

        public static Hit FromFullLife(double value)
        {
            var i    = 0;
            while (value >= 1000d)
            {
                value /= 1000d;
                i++;
            }

            //Assert.AreEqual(init, value * Math.Pow(1000, i), 0, "Invalid number range!");
            return new Hit
                   {
                       Life = value,
                       Unit = i
                   };
        }

        public override string ToString()
        {
            return string.Format("{0:0.0} {1}", Life, UNITS[Unit]).Trim().Replace(".0", string.Empty);
        }
    }
}