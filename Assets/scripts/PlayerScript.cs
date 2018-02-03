using System;
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
        public                   Text   ClickTime;
        public                   Text   RemainingMoneyText;
        public                   Slider RemainingMoneySlider;
        public                   Text   HighscoreText;
        [Header("Player")]public int    Level = 1;
        public                   double InitMoney;
        [HideInInspector]public  Hit    Money;

        private string               moneyFormat;
        private string               levelFormat;
        private string               remainingMoneyFormat;
        private string               clickTimeFormat;
        private AttackerButtonScript attackerTouch;
        private string               highscoreFormat;
        private TimeSpan             highscoreTime;
        private DateTime             levelTime;
        private int                  highscoreLevel;

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
            Money                = Hit.FromFullLife(InitMoney, 4);
            moneyFormat          = MoneyText.text;
            levelFormat          = LevelText.text;
            remainingMoneyFormat = RemainingMoneyText.text;
            clickTimeFormat      = ClickTime.text;
            highscoreFormat      = HighscoreText.text;
            highscoreTime        = TimeSpan.Zero;
            highscoreLevel       = 1;
            levelTime            = DateTime.Now;
            attackerTouch        = GameObject.FindGameObjectsWithTag(AttackerButtonScript.TAG).Select(bo => bo.GetComponent<AttackerButtonScript>()).FirstOrDefault(script => script.Titel.Equals("Touch"));
            UpdateTexts();
        }

        void Update()
        {
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            MoneyText.text             = string.Format(moneyFormat, Money.ToString(4));
            var money                  = GetRemainingMoney();
            RemainingMoneyText.text    = string.Format(remainingMoneyFormat, money.ToString(4));
            RemainingMoneySlider.value = GetRemainingMoneyPercentage(money);
            LevelText.text             = string.Format(levelFormat,      Level);
            ClickTime.text             = string.Format(clickTimeFormat,  TimeSpan.FromSeconds(CalculateClickInterval).ToString(@"ss\.fff"));
            var text                   = string.Format("{0:0.00}s (Level {1})", highscoreTime.TotalSeconds, highscoreLevel);
            HighscoreText.text         = string.Format(highscoreFormat,  text);
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
            Money += Hit.FromFullLife(money, 4);
        }

        public static float CalculateClickInterval => MouseHelper.DEFAULT_CLICK_REPEAT_DURATION_SECONDS / PlayerScript.Player.Level;

        public void LevelUpPlayer()
        {
            var span = DateTime.Now - levelTime;
            if (highscoreTime == TimeSpan.Zero || span <= highscoreTime)
            {
                highscoreTime  = span;
                highscoreLevel = Level;
            }

            Level++;
            levelTime = DateTime.Now;
        }
    }
}