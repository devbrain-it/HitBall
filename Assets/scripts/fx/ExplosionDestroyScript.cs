using UnityEngine;

namespace Assets.scripts.fx
{
    public class ExplosionDestroyScript : MonoBehaviour
    {
        public ExplosionScript ExplosionPrefab;
        public float           BurstWidth  = 1;
        public float           InitRadius  = 0.62f;
        public float           TotalRadius = 3;

        public void Explode(GameObject parent, ExplosionData data, Vector2? position = null)
        {
            var prefab      = ExplosionPrefab.gameObject;
            var pos         = position ?? transform.position;
            var rot         = Quaternion.Euler(0, 0, 0);
            var newinstance = Instantiate(prefab, pos, rot);

            if (parent != null)
            {
                newinstance.transform.SetParent(parent.transform);
            }

            var script               = newinstance.GetComponent<ExplosionScript>();
            script.BurstMinimumForce = data.Force;
            script.TotalRadius       = TotalRadius;
            script.BurstWidthStart   = BurstWidth;
            script.Radius            = data.InitRadius;
            script.Init();
            script.Play();
        }

        public struct ExplosionData
        {
            public double Force      { get; set; }
            public float  InitRadius { get; set; }
        }
    }
}