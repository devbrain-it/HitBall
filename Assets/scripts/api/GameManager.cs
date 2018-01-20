using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(fileName = "GameManager", menuName = "New Game-Manager")]
    public class GameManager : ScriptableObject
    {
        public string            Title;
        public string            Version = "1.0";
        public PlayerData        Player;
        public List<BarrierType> BarrierTypes;
        public Shop              Shop;
        public Gradient          BarrierLifeColor;
        public string            Units = " aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVxXyYzZ";
        public BarrierData       RemainingBarriers;
        public int               BarriersSpawnOnLevelUp = 10;

        [UsedImplicitly]
        void OnEnable()
        {
            Instance = this;
        }

        public static GameManager Instance { get; private set; }
    }
}