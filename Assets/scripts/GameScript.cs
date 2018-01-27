using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
    public class GameScript : MonoBehaviour
    {
        [Header("UI")]public                      Collider2D     Spawnarea;
        public                                    GameObject     BarrierPrefab;
        public                                    GameObject     LoadingPanel;
        public                                    LevelUpScript  LevelUpAnimation;
        [Header("Barriers"), Range(1, 100)]public int            BarriersToSpawn = 20;
        [Range(                    1, 100)]public double         BarrierLife     = 7;
        public                                    AnimationCurve BarrierLifeMultiplicator;
        [HideInInspector]public                   bool           CanSpawnBarriers = true;
        public                                    Transform      SpawnParent;
        public                                    GameObject     ExplosionParent;
        [Header("Store Power Actions")]public     Toggle         FullUpgradeToggle;

        public static GameScript Game { get; private set; }

        public bool   InLevelUp   { get; private set; }
        public double MoneyToEarn { get; private set; }

        public bool IsFullUpgradeActive => FullUpgradeToggle.isOn;

        // Use this for initialization
        void Start()
        {
            Game             = this;
            CanSpawnBarriers = FindAllBarriers().Count < BarriersToSpawn;
            Time.timeScale   = 1;
        }

        public static bool HasBarriers => FindAllBarriers().Any();

        private static List<BarrierScript> FindAllBarriers()
        {
            return GameObject.FindGameObjectsWithTag(BarrierScript.TAG)
                             .Select(go => go.GetComponent<BarrierScript>())
                             .Where(go => go.Life > 0)
                             .ToList();
        }

        void Update()
        {
            var barriers = FindAllBarriers();
            if (barriers.Count < BarriersToSpawn && CanSpawnBarriers)
            {
                if (PlayerScript.Player.Level <= 1)
                {
                    SpawnBarrier();
                }
                else if (CanSpawnBarriers)
                {
                    SpawnBarrier();
                }
            }
            else if (barriers.Count == BarriersToSpawn)
            {
                CanSpawnBarriers = false;
                InLevelUp        = false;
            }
        }

        private void SpawnBarrier()
        {
            var spawnArea = Spawnarea;
            if (spawnArea != null)
            {
                var position = SpawnHelper.RandomPositionInArea(spawnArea);
                if (position != null)
                {
                    var b = SpawnHelper.TrySpawn(BarrierPrefab, position.Value, Quaternion.identity);
                    if (b != null)
                    {
                        InitBarrier(b);
                        UpdateLoadingPanel();
                    }
                }
            }
        }

        private void UpdateLoadingPanel()
        {
            if (LoadingPanel != null)
            {
                var slider = LevelSlider;
                if (slider != null)
                {
                    var count    = FindAllBarriers().Count;
                    var progress = count / (float) BarriersToSpawn;
                    LoadingPanel.SetActive(progress < 1f);
                    slider.value = progress;
                    UpdateMoneyToEarn();
                }
            }
        }

        private void UpdateMoneyToEarn()
        {
            var slider = LevelSlider;
            if (slider != null && slider.value >= 1)
            {
                MoneyToEarn = FindAllBarriers().Sum(l => l.FullLife);
            }
            else
            {
                MoneyToEarn = 0;
            }
        }

        private Slider LevelSlider => LoadingPanel.GetComponentInChildren<Slider>();

        private void InitBarrier(GameObject b)
        {
            if (SpawnParent != null)
            {
                SpawnHelper.SetParentInHierarchy(b, SpawnParent.gameObject);
            }

            var barrier             = b.GetComponent<BarrierScript>();
            barrier.ExplosionParent = ExplosionParent;
            barrier.Life            = BarrierLife;
        }

        public void Hit(bool destroyed)
        {
            if (destroyed && !InLevelUp)
            {
                var hasBarriers = HasBarriers;
                //Debug.Log(string.Format("Has barriers: {0}", hasBarriers));
                if (!hasBarriers)
                {
                    LevelUp();
                }
            }
        }

        private void LevelUp()
        {
            InLevelUp  = true;
            var player = PlayerScript.Player;
            player.Level++;
            var f                                        = Mathf.Min(1, player.Level / 100f);
            BarrierLife                                  += BarrierLife              * (1 + BarrierLifeMultiplicator.Evaluate(f));
            LevelUpAnimation.Play(() => CanSpawnBarriers =  true);
        }
    }
}