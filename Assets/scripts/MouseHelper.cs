using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Object = UnityEngine.Object;

namespace Assets.scripts
{
    public class MouseHelper
    {
        public static readonly Func<bool> OnMouseLeftDown = () => Input.GetMouseButtonDown(0);

        public static void TestMouseButton(Func<bool> check, GameObject sender, Action onClick, params GameObject[] ignore)
        {
            if (check())
            {
                PerformOnValidTargetObject(sender, onClick, ignore);
            }
        }

        public static void PerformOnValidTargetObject([CanBeNull]GameObject sender, Action onClick, params GameObject[] ignore)
        {
            SetObjectsActive(ignore, false);

            if (IsValidTargetClicked(sender))
            {
                //Debug.Log("Simulated Click");
                onClick();
            }

            SetObjectsActive(ignore, true);
        }

        public static void SetObjectsActive(IEnumerable<GameObject> list, bool active)
        {
            list.ToList().ForEach(b => b.SetActive(active));
        }

        public static bool IsValidTargetClicked([CanBeNull]GameObject shouldBeTarget)
        {
            var        mousePos     = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var        mousePos2D   = new Vector2(mousePos.x, mousePos.y);
            Collider2D collidedWith = null;
            //if (shouldBeTarget != null)
            //{
            //    var filter = new ContactFilter2D();
            //    var hits   = new Collider2D[3];
            //    Physics2D.OverlapCollider(shouldBeTarget.GetComponent<Collider2D>(), filter, hits);
            //    hits = hits.Where(e => e != null).ToArray();
            //    foreach (var collider2D in hits)
            //    {
            //        if (collider2D.gameObject == shouldBeTarget.gameObject)
            //        {
            //            collidedWith = collider2D;
            //            break;
            //        }
            //    }
            //}
            //else
            {
                var hit      = Physics2D.Raycast(mousePos2D, Vector2.zero);
                collidedWith = hit.collider;
            }

            return collidedWith != null && (shouldBeTarget == null || collidedWith.gameObject == shouldBeTarget);
        }

        public static bool IsMouseLeftDown => Input.GetMouseButton(0);

        public const float DEFAULT_CLICK_REPEAT_DURATION_SECONDS = 0.3f;
    }
}