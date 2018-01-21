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

        public static void TestMouseButton(Func<bool> check, Object sender, Action onClick, params GameObject[] ignore)
        {
            if (check())
            {
                PerformOnValidTargetObject(sender, onClick, ignore);
            }
        }

        public static void PerformOnValidTargetObject([CanBeNull]Object sender, Action onClick, params GameObject[] ignore)
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

        public static bool IsValidTargetClicked(Object toIgnore)
        {
            var mousePos   = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);
            var hit        = Physics2D.Raycast(mousePos2D, Vector2.zero);
            return hit.collider != null && (toIgnore == null || hit.collider.gameObject == toIgnore);
        }

        public static bool IsMouseLeftDown => Input.GetMouseButton(0);

        public const float DEFAULT_CLICK_REPEAT_DURATION_SECONDS = 0.3f;
    }
}