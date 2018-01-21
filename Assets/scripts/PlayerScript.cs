using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
    public class PlayerScript : MonoBehaviour
    {
        public const string TAG = "Player";

        [Header("UI")]public     Text   MoneyText;
        public                   Text   LevelText;
        public                   Text   RemainingMoneyText;
        public                   Slider RemainingMoneySlider;
        [Header("Player")]public int    Level = 1;
        public                   double InitMoney;
        [HideInInspector]public  Hit    Money;

        private string               moneyFormat;
        private string               levelFormat;
        private string               remainingMoneyFormat;
        private AttackerButtonScript attackerTouch;

        public static PlayerScript Player
        {
            get
            {
                var playerGo = GameObject.FindGameObjectWithTag(TAG);
                return playerGo == null ? null : playerGo.GetComponent<PlayerScript>();
            }
        }

        public Hit GetClickHitForce()
        {
            return Hit.FromFullLife(attackerTouch.Force);
        }

        void Start()
        {
            Money                = Hit.FromFullLife(InitMoney);
            moneyFormat          = MoneyText.text;
            levelFormat          = LevelText.text;
            remainingMoneyFormat = RemainingMoneyText.text;
            attackerTouch        = GameObject.FindGameObjectsWithTag(AttackerButtonScript.TAG).Select(bo => bo.GetComponent<AttackerButtonScript>()).FirstOrDefault(script => script.Titel.Equals("Touch"));
            UpdateTexts();
        }

        void Update()
        {
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            MoneyText.text             = string.Format(moneyFormat, Money);
            var money                  = GetRemainingMoney();
            RemainingMoneyText.text    = string.Format(remainingMoneyFormat, money);
            RemainingMoneySlider.value = GetRemainingMoneyPercentage(money);
            LevelText.text             = string.Format(levelFormat, Level);
        }

        private static Hit GetRemainingMoney()
        {
            var objects = GameObject.FindGameObjectsWithTag(BarrierScript.TAG);
            var scripts = objects.Select(bo => bo.GetComponent<BarrierScript>().Life);
            return Hit.FromFullLife(scripts.Sum(l => l / 10000d) * 10000);
        }

        private static float GetRemainingMoneyPercentage(Hit money)
        {
            var remaining = money.FullLife;
            var game      = GameScript.Game;
            if (game != null)
            {
                var full = game.MoneyToEarn;
                return 1f - (float) (remaining / full);
            }

            return 0;
        }

        public void AddMoney(double money)
        {
            Money += Hit.FromFullLife(money);
        }
    }
}