using System.ComponentModel;
using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(fileName = "Barriers/Barrier", menuName = "Create Barrier-Type")]
    public class BarrierType : ScriptableObject
    {
        public GameObject EffectOnDestroy;
        public float      RadiusMin;
        public float      RadiusMax;
        public bool       RandomRadius = false;
        public double     LifeTotal;

        [Description("Upgrade Change upto Level 100")]
        public AnimationCurve LifeUpgrade;
    }
}