using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(menuName = "New Barrier-Collection")]
    public class BarrierData : ScriptableObject
    {
        public List<BarrierState> Barriers;
    }
}