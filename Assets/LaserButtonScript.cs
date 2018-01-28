using System;
using Assets.scripts;
using UnityEngine;

namespace Assets
{
    public class LaserButtonScript : MonoBehaviour
    {
        public CutterScript  Laser;
        public CautionScript CautionPanel;
        public float         CautionDurationSec = 5;

        public bool LaserIsActive { get; private set; }

        // Use this for initialization
        void Start()
        {
            Laser.SetStartPosition(Laser.transform.position);
            Laser.StopLaser();
            LaserIsActive = false;
        }

        public void OnClick()
        {
            if (LaserIsActive)
            {
                Laser.StopLaser();
                CautionPanel.Hide();
            }
            else
            {
                CautionPanel.Show();
                Laser.ActivateLaser();
            }

            LaserIsActive = !LaserIsActive;
        }
    }
}