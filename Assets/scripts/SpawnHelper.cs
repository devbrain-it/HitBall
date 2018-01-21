using UnityEngine;

namespace Assets.scripts
{
    public static class SpawnHelper
    {
        public static GameObject TrySpawn(GameObject prefab, Vector3 center, Vector3 size, Quaternion rotation, int overlapColliderThreshold = 1)
        {
            var position = RandomPointInBox(center, size - prefab.transform.localScale);
            return TrySpawn(prefab, position, rotation, overlapColliderThreshold);
        }

        public static GameObject TrySpawn(GameObject prefab, Vector3 position, Quaternion rotation, int overlapColliderThreshold = 1)
        {
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

        /// <summary>
        /// Finds a random position inside a collider area
        /// </summary>
        /// <param name="spawnArea">Collider2D e.g. CircleCollider2D, BoxCollider2D, PoygonCollider2D</param>
        /// <param name="maxMismatches">Tries before give up to find</param>
        /// <returns></returns>
        public static Vector2? RandomPositionInArea(Collider2D spawnArea, int maxMismatches = 10)
        {
            var bounds = spawnArea.bounds;
            var center = bounds.center;

            var     mismatchCounter = 0;
            Vector2 result;
            do
            {
                mismatchCounter++;
                if (maxMismatches >= 0 && mismatchCounter > maxMismatches)
                {
                    return null;
                }

                var x  = Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
                var y  = Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.y);
                result = new Vector2(x, y);
            } while (Physics2D.OverlapPoint(result) == null);

            return result;
        }
    }
}