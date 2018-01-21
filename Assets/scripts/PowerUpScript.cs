using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
    public class PowerUpScript : MonoBehaviour
    {
        [Header("UI")]public      Text   TitleText;
        public                    Image  IconImage;
        public                    Text   CostText;
        public                    Text   TimeText;
        [Header("PowerUp")]public float  Multiplier      = 2;
        public                    float  DurationSeconds = (float) TimeSpan.FromMinutes(5).TotalSeconds;
        public                    string Title;
        public                    double Costs;
        public                    Sprite Icon;

        private Button button;
        private float  runtime;
        private bool   started;

        public static MultiplierInfo Instance { get; private set; }

        void Start()
        {
            if (Instance == null)
            {
                Instance = new MultiplierInfo();
            }

            TitleText.text            = Title;
            IconImage.sprite          = Icon;
            CostText.text             = Hit.FromFullLife(Costs).ToString();
            button                    = GetComponent<Button>();
            Instance.MultiplierEffect = 1;
        }

        void Update()
        {
            if (!PlayerCanPay())
            {
                button.interactable = false;
            }
            else if (started)
            {
                button.interactable = Multiplier > Instance.MultiplierEffect;
            }
            else if (Instance.MultiplierEffect > 1)
            {
                button.interactable = Multiplier > Instance.MultiplierEffect;
            }
            else
            {
                button.interactable = true;
            }

            if (MouseHelper.IsMouseLeftDown && MouseHelper.IsValidTargetClicked(gameObject))
            {
                if (button.interactable)
                {
                    OnMouseLeftUp();
                }
            }

            if (started)
            {
                TimeText.text =  ToRemainingTime(DurationSeconds - runtime);
                runtime       += Time.deltaTime;
                if (runtime >= DurationSeconds)
                {
                    Instance.MultiplierEffect = 1;
                    runtime                   = 0;
                    started                   = false;
                }
            }
            else
            {
                TimeText.text = ToRemainingTime(DurationSeconds);
            }
        }

        private bool PlayerCanPay()
        {
            return PlayerScript.Player.Money.GreaterThanOrEqual(Hit.FromFullLife(Costs));
        }

        private static string ToRemainingTime(float seconds)
        {
            return TimeSpan.FromSeconds(seconds).ToString("h\\:mm\\:ss");
        }

        void OnMouseLeftUp()
        {
            Debug.Log("PowerUp");

            if (!started)
            {
                PlayerScript.Player.Money -= Hit.FromFullLife(Costs);
                runtime                   =  0;
                Instance.MultiplierEffect =  Multiplier;
                started                   =  true;
            }
        }
    }

    public class MultiplierInfo
    {
        private float multiplierEffect = 1;

        public float MultiplierEffect
        {
            get { return multiplierEffect; }
            internal set { multiplierEffect = Math.Max(1, value); }
        }
    }
}