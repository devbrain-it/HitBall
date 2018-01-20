using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.api
{
    [CreateAssetMenu(menuName = "New Shop-Data")]
    public class Shop : ScriptableObject
    {
        public float          ClickRateSeconds = 0.2f;
        public List<ShopItem> Buttons          = new List<ShopItem>();
    }
}