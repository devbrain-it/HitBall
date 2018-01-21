using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
    public partial class AttackerButtonScript : MonoBehaviour
    {
        public const string TAG = "Attacker";

        public GameObject BallPrefab;
        public Text       TitleTextObject;
        public Text       ForceTextObject;
        public Text       CostTextObject;
        public Image      IconImage;

        public Sprite         Icon;
        public string         Titel;
        public double         Costs;
        public double         Force;
        public float          Speed = 15;
        public float          Mass  = 1;
        public AnimationCurve CostCurveLevel100;
        public AnimationCurve ForceCurveLevel100;

        private BallScript       ballScript;
        private bool             spawnBall;
        private double           spawnBallForce;
        private double           baseCosts;
        private double           baseForce;
        private double           lastTime;
        private HashSet<PowerUp> powerUpEffect;
        private GameObject       ballsSpawnParent;
        private Button           button;
        private bool active;

        void Start()
        {
            baseCosts        = Costs;
            baseForce        = Force;
            ballsSpawnParent = GameObject.Find("balls");
            button           = GetComponent<Button>();
            UpdateText();
        }

        void Update()
        {
            button.interactable = PlayerScript.Player.Money.GreaterThan(Hit.FromFullLife(Costs));

            if (spawnBall && ballScript == null && BallPrefab != null)
            {
                var spawnArea = GameScript.Game.Spawnarea;
                var bo        = SpawnHelper.TrySpawn(BallPrefab, spawnArea.transform.position, spawnArea.bounds.size, Quaternion.identity);
                if (bo != null)
                {
                    SpawnHelper.SetParentInHierarchy(bo, ballsSpawnParent);

                    //Debug.Log("Spawn Ball");
                    ballScript          = bo.GetComponent<BallScript>();
                    ballScript.HitForce = spawnBallForce;
                    ballScript.Speed    = Speed;
                    ballScript.Mass     = Mass;

                    spawnBall = false;
                }
            }

            UpdateFullUpgrade();

            if (IsPowerUpMaximum && IsTimeDelayed)
            {
                lastTime = Time.time;
                OnClick();
            }
        }

        private void UpdateFullUpgrade()
        {
            // wenn geklickt wird und es aktiv ist, wird es hinzugefügt
            // wenn das Geld alle ist, wird es entfernt, siehe "Upgrade()"

            // klick aktiviert es, wenn es eingeschaltet ist, solange Geld da ist
            if (MouseHelper.IsMouseLeftDown && GameScript.Game.IsFullUpgradeActive && active)
            {
                PowerUps.Add(PowerUp.MAXIMUM);
            }
        }

        private bool IsTimeDelayed => Time.time - lastTime >= MouseHelper.DEFAULT_CLICK_REPEAT_DURATION_SECONDS;

        private HashSet<PowerUp> PowerUps
        {
            get
            {
                var upEffect = powerUpEffect;
                if (upEffect == null)
                {
                    upEffect      = new HashSet<PowerUp>();
                    powerUpEffect = upEffect;
                }

                return upEffect;
            }
        }

        private bool IsPowerUpMaximum => PowerUps.Contains(PowerUp.MAXIMUM);

        public void OnClick()
        {
            var upgrade = Upgrade();
            if (upgrade)
            {
                if (!spawnBall)
                {
                    UpdateText();
                }
            }
            else
            {
                PowerUps.Remove(PowerUp.MAXIMUM);
            }
        }

        private bool Upgrade()
        {
            var costs    = Hit.FromFullLife(Costs);
            var player   = PlayerScript.Player;
            var hasMoney = player.Money.GreaterThanOrEqual(costs);
            if (hasMoney)
            {
                player.Money -= costs;

                if (!IsTouchAttacker && IsSpawnRequired)
                {
                    // spawn in next update
                    spawnBall      = true;
                    spawnBallForce = Force;
                }

                UpgradeForce();

                UpgradeCosts();
            }

            return hasMoney;
        }

        private bool IsSpawnRequired => ballScript == null;

        public bool IsTouchAttacker => BallPrefab == null;

        private void UpgradeCosts()
        {
            var level       = PlayerScript.Player.Level;
            var playerLevel = Math.Min(100, level);

            // f zwischen 0 und 1+
            var f           = CostCurveLevel100.Evaluate(playerLevel);
            var costFactor  = f;
            var forceFactor = ForceCurveLevel100.Evaluate(playerLevel);
            //Debug.Log($"Cost xf: {costFactor:0.000}, Force xf: {forceFactor:0.000}");

            Costs += baseCosts * costFactor  * level;
            Force += baseForce * forceFactor * level;

            UpdateText();
        }

        void OnMouseEnter()
        {
            active = true;
        }
        
        void OnMouseExit()
        {
            active = false;
        }

        private void UpgradeForce()
        {
            if (ballScript != null)
            {
                ballScript.HitForce = Force;
            }
        }

        private void UpdateText()
        {
            TitleTextObject.text = Titel;
            ForceTextObject.text = Hit.FromFullLife(Force).ToString();
            CostTextObject.text  = Hit.FromFullLife(Costs).ToString();
            IconImage.sprite     = Icon;
        }

        public void UpgradeToMaximum()
        {
            powerUpEffect.Add(PowerUp.MAXIMUM);
        }
    }
}