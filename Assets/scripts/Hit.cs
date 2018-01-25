using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.scripts
{
    public struct Hit
    {
        public const  string UNITS = " aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
        private const double BUCKS = 1000d;
        private       string text;
        private       int    floats;
        private       double value;

        public double Life { get; private set; }

        public int Multiplier { get; private set; }

        public int Index { get; private set; }

        public bool IsZero => Life <= 0 && Multiplier == 0 && Index == 0;

        public double FullLife => getLongLife(this);

        public bool GreaterThan(Hit b)
        {
            var aLife = getLongLife(this);
            var bLife = getLongLife(b);
            return aLife - bLife > 0;
        }

        public bool GreaterThanOrEqual(Hit b)
        {
            var aLife = getLongLife(this);
            var bLife = getLongLife(b);
            return aLife - bLife >= 0;
        }

        public bool LowerThan(Hit b)
        {
            var aLife = getLongLife(this);
            var bLife = getLongLife(b);
            return aLife - bLife < 0;
        }

        public bool LowerThanOrEqual(Hit b)
        {
            var aLife = getLongLife(this);
            var bLife = getLongLife(b);
            return aLife - bLife <= 0;
        }

        public static Hit operator -(Hit a, Hit b)
        {
            var valA = a.FullLife;
            var valB = b.FullLife;
            if (valA < valB)
            {
                return FromFullLife(0, Math.Max(a.floats, b.floats));
            }

            var result = Math.Min(Math.Max(0, valA - valB), double.MaxValue);
            return FromFullLife(result);
        }

        public static Hit operator +(Hit a, Hit b)
        {
            var valA   = a.FullLife;
            var valB   = b.FullLife;
            var result = Math.Max(0,                       Math.Min(double.MaxValue, valA + valB));
            return FromFullLife(result, Math.Max(a.floats, b.floats));
        }

        private static double getLongLife(Hit a)
        {
            return a.value;
        }

        public static Hit FromFullLife(double life, int floats = 2)
        {
            var multiplier = 0;
            var index      = 0;
            var fullLife   = life;
            while (life >= BUCKS)
            {
                life /= BUCKS;
                index++;
                if (index > UNITS.Length)
                {
                    index = 0;
                    multiplier++;
                }
            }

            return new Hit
                   {
                       value      = fullLife,
                       Life       = life,
                       Multiplier = multiplier,
                       Index      = index,
                       floats     = floats,
                       text       = makeText(life, multiplier, index, floats)
                   };
        }

        private static string makeText(double visibleMoney, int generations, int unit, int floats)
        {
            var generationUnit = UNITS.Last().ToString();

            var units = generations.For(generationUnit, createEntry, createResultFromEntries);
            var end   = floats.For("0", (i, v) => v, vals => string.Join(string.Empty, vals));

            if (visibleMoney - Math.Floor(visibleMoney) > 0)
            {
                return ($"{visibleMoney:0.00}".Replace("." + end, string.Empty).Replace("," + end, string.Empty) + $" {units}{UNITS[unit]}").Trim();
            }

            return ($"{visibleMoney}".Replace("." + end, string.Empty).Replace("," + end, string.Empty) + $" {units}{UNITS[unit]}").Trim();
        }

        private static string createResultFromEntries(IEnumerable<string> values)
        {
            return string.Join(string.Empty, values);
        }

        private static string createEntry(int i, string v)
        {
            return v;
        }

        public override string ToString()
        {
            return text ?? "???";
        }

        public string ToString(int floatsOverride)
        {
            text = makeText(Life, Multiplier, Index, floatsOverride);
            return text;
        }
    }
}