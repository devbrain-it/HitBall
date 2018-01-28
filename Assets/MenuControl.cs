using System.Linq;
using UnityEngine;

namespace Assets
{
    public class MenuControl : MonoBehaviour
    {
        [Header("Overlay")]public bool         OverlayVisible = true;
        public                    EditCanvas[] Overlay;
        [Header("HUD")]public     bool         HUDVisible = true;
        public                    EditCanvas[] HUD;

        void Start()
        {
            UpdateMenu(false);
        }

        void Update()
        {
            
        }

        public void UpdateMenu(bool visibilty = false)
        {
            var groups = Overlay.GroupBy(o => o.GroupID).ToList();
            var i      = groups.Count - 1;
            foreach (var menuGroup in groups)
            {
                foreach (var menu in menuGroup)
                {
                    menu.Menu.sortingOrder = i;
                    menu.Panel.SetActive(menu.Visible && OverlayVisible);
                }

                i--;
            }

            groups = HUD.GroupBy(o => o.GroupID).ToList();
            i      = groups.Count - 1;
            foreach (var menuGroup in groups)
            {
                foreach (var menu in menuGroup)
                {
                    menu.Menu.sortingOrder = i;
                    menu.Panel.SetActive(menu.Visible && HUDVisible);
                }

                i--;
            }
        }
    }
}