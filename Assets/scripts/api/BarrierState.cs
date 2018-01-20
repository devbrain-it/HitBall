using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(menuName = "New Barrier Position")]
    public class BarrierState : ScriptableObject
    {
        public int    BarrierTypeIndex;
        public float  X;
        public float  Y;
        public float  Z;
        public double Life;
        public double LifeTotal;
        public float  Radius;
    }
}