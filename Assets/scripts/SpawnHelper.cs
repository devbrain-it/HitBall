using UnityEngine;

namespace Assets.scripts
{
    public static class SpawnHelper
    {
        public static GameObject SpawnPrefab(GameObject prefab, Vector3 center, Vector3 size, Quaternion rotation, int overlapColliderThreshold = 1)
        {
            var position = RandomPointInBox(center, size - prefab.transform.localScale);
            var b        = Object.Instantiate(prefab, position, rotation);
            var contacts = new Collider2D[15];
            if (b.GetComponent<CircleCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), contacts) > overlapColliderThreshold)
            {
                Object.Destroy(b);
                return null;
            }

            return b;
        }

        public static Vector3 RandomPointInBox(Vector3 center, Vector3 size)
        {
            var x = (Random.value - 0.5f) * size.x;
            var y = (Random.value - 0.5f) * size.y;
            return center         + new Vector3(x, y, 0);
        }

        public static void SetParentInHierarchy(GameObject child, GameObject parent)
        {
            child.transform.SetParent(parent.transform);
        }

        public static void Restart(this ParticleSystem particles)
        {
            particles.Simulate(0, true, true);
            particles.Play();
        }
    }
}