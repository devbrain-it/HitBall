using System;
using UnityEngine;

namespace Assets.scripts
{
    public class ColorizeScript : MonoBehaviour
    {
        public  Gradient       LifetimeColors = new Gradient();
        public  SpriteRenderer Renderer;
        private double         progress;

        public double Progress
        {
            get { return progress; }
            set
            {
                progress = Math.Min(1, Math.Max(0, value));
                Update();
            }
        }

        public Color Color { get; private set; }

        void Start()
        {
            UpdateColor();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateColor();
        }

        private void UpdateColor()
        {
            Color = LifetimeColors.Evaluate(1 - Single.Parse(String.Format("{0:0.000}", Progress)));
            RenderColor();
        }

        private void RenderColor()
        {
            if (Renderer != null)
            {
                Renderer.color = Color;
            }
        }

        public static Color AdjustHueColor(Color targetColor, Color source, bool holdSaturation = true, bool holdBrightness = true)
        {
            float hue;
            float saturation;
            float brightness;

            Color.RGBToHSV(targetColor, out hue, out saturation, out brightness);
            var toHue    = hue;
            var toSat    = saturation;
            var toBright = brightness;
            Color.RGBToHSV(source, out hue, out saturation, out brightness);

            var newColor = Color.HSVToRGB(toHue, holdSaturation ? saturation : toSat, holdBrightness ? brightness : toBright);
            return newColor;
        }

        public static readonly Color TRANSPARENT = new Color(0, 0, 0, 0);
    }
}