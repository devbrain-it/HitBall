using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public struct EditCanvas
    {
        public Canvas     Menu;
        public GameObject Panel;
        public bool       Visible;
        public int        GroupID;
    }
}