using System.ComponentModel;
using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(fileName = "ShopItems/ShopItem", menuName = "New Shop-Item")]
    public class ShopItem : ScriptableObject
    {
        public string     Name;
        public Sprite     Image;
        public double     Force = 1;
        public double     Costs = 1;
        public GameObject Prefab;

        [Description("Force upto level 100")]public AnimationCurve ForceUpgrade;
        [Description("Cost upto level 100")] public AnimationCurve CostUpgrade;
    }
}