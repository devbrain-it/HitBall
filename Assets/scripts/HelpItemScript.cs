using System;
using Assets.scripts.fx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TimeSpan = System.TimeSpan;

namespace Assets.scripts
{
    public class HelpItemScript : MonoBehaviour
    {
        public ExplosionScript ExplosionScript;
        public Text            CountdownText;
        public double          ExplosionForce;
        public float           CountdownSeconds = 5f;

        private float runtime;
        private bool  exploded;

        void Start()
        {
            UpdateCountdownText();
        }

        void Update()
        {
            if (!exploded)
            {

                if (GameScript.Game.InLevelUp)
                {
                    exploded = true;
                    Destroy(gameObject);
                    return;
                }

                runtime += Time.deltaTime;
                UpdateCountdownText();

                if (runtime > CountdownSeconds)
                {
                    exploded = true;
                    var r    = GetComponent<CircleCollider2D>().radius;
                    ExplosionScript.ExplodeAsTrigger(r, ExplosionForce);

                    var rb          = GetRigidbody(gameObject);
                    rb.gravityScale = 0;
                    rb.velocity     = Vector2.zero;
                }
            }
        }

        public void PlaceBomb()
        {
            if (FindObjectsOfType<HelpItemScript>().Length >= 5)
            {
                return;
            }

            Vector2? position;
            do
            {
                position = SpawnHelper.RandomPositionInArea(GameScript.Game.Spawnarea);
            } while (position == null);

            var bomb          = Instantiate(gameObject, position.Value, Quaternion.identity);
            var rotationspeed = Random.Range(0, 30) - Random.Range(0, 30);

            var rb = GetRigidbody(bomb);
            rb.AddTorque(rotationspeed * Time.deltaTime);
            rb.AddForce(Vector2.up     * 10 * Time.deltaTime);
        }

        private static Rigidbody2D GetRigidbody(GameObject bomb)
        {
            return bomb.GetComponent<Rigidbody2D>();
        }

        private void UpdateCountdownText()
        {
            CountdownText.text = TimeSpan.FromSeconds(Math.Max(0, CountdownSeconds - runtime)).ToString(@"m\:ss\.f");
        }
    }
}