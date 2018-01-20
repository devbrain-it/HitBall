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
            Color = LifetimeColors.Evaluate(1 - float.Parse(string.Format("{0:0.000}", Progress)));
            RenderColor();
        }

        private void RenderColor()
        {
            if (Renderer != null)
            {
                Renderer.color = Color;
            }
        }
    }
}