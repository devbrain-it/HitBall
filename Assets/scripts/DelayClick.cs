using System;
using UnityEngine;

namespace Assets.scripts
{
    public class DelayClick
    {
        public const int MOUSE_LEFT_BUTTON = 0;

        private double lastTime;

        public DelayClick()
        {
            DelaySeconds     = 0.09f;
            MouseButtonIndex = MOUSE_LEFT_BUTTON;
        }

        public float DelaySeconds { get; set; }

        public int MouseButtonIndex { get; set; }

        public Action ClickEvent { get; set; }

        public void Check()
        {
            if (Input.GetMouseButton(MOUSE_LEFT_BUTTON) && Time.time - lastTime >= DelaySeconds)
            {
                lastTime   = Time.time;
                var handle = ClickEvent;
                if (handle != null)
                {
                    handle();
                }
            }
        }
    }
}