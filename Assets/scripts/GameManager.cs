using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts
{
    [CreateAssetMenu(menuName = "Assets\\GameControl")]
    public class GameManager : ScriptableObject
    {
        public GameScript     BarrierConfig;
        public PlayerScript   PlayerConfig;
        public List<Attacker> ShopConfig = new List<Attacker>();
    }

    public class GameLevel : ScriptableObject
    {
        public int           BarrierSpawnCount = 10;
        public List<Barrier> BarrierTypes      = new List<Barrier>();

        public void Spawn()
        {
            var typeIndex = UnityEngine.Random.Range(0, BarrierTypes.Count);
            var b         = BarrierTypes[typeIndex];
            // GameScript.Game.Spawn();
        }
    }

    public class Barrier : ScriptableObject
    {
        public double               Life = 7;
        public GameObject           Prefab;
        public List<BarrierAbility> Abilities = new List<BarrierAbility>();
    }

    public abstract class BarrierAbility : ScriptableObject
    {
        public abstract void Update(BarrierScript obj);
    }

    public class Attacker : ScriptableObject
    {
        public Sprite     Icon;
        public string     Title;
        public double     UnlockCost     = 1;
        public double     UnlockHitForce = 1;
        public VisualMode VisualMode     = VisualMode.PREFAB;

        public GameObject Prefab;
        public float      Mass;
        public float      Speed;
    }

    public enum VisualMode
    {
        PREFAB,
        TOUCH
    }
}