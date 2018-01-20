using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(fileName = "Player", menuName = "Create Player-Data")]
    public class PlayerData : ScriptableObject
    {
        public int    Level;
        public double Money;
    }
}