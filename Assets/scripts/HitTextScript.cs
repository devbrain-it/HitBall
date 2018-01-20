using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
    public class HitTextScript : MonoBehaviour
    {
        [Header("UI")]
        public Text TextElement;

        public void SetHit(double value)
        {
            TextElement.text = scripts.Hit.FromFullLife(value).ToString();
        }
    }
}