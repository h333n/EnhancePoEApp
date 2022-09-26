using ChaosRecipeEnhancer.UI.Properties;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Model
{
    internal class MouseMover
    {
        public static void MoveMouseToTab(StashTab tab)
        {
            try
            {
                Point tabLocation = tab.TabHeader.PointToScreen(new Point(0, 0));
                var tabSize = new Size(tab.TabHeader.ActualWidth, tab.TabHeader.ActualHeight);

                var x = tabLocation.X + tabSize.Width / 2;
                var y = tabLocation.Y + tabSize.Height / 2;


                MouseHook.MoveMouse((int)x, (int)y);
            }
            catch { }
        }

        public static void MoveMouseToItem(Item item)
        {
            var margin = 7; // there seem to be a margin of ~7px on the top, botton and on the sides
            var overlayHeaderH = 50; // height of the header (seems to be fixed ~ 50px)
            var overlayX = Settings.Default.StashTabOverlayLeftPosition + margin;
            var overlayY = Settings.Default.StashTabOverlayTopPosition + margin;
            var overlayW = Settings.Default.StashTabOverlayWidth - margin - margin;
            var overlayH = Settings.Default.StashTabOverlayHeight - margin - margin;
            var stashtab = StashTabList.StashTabs.First(tab => tab.TabIndex == item.StashTabIndex);
            var gridSize = stashtab.Quad ? 24 : 12;

            var x = overlayX + overlayW * ((item.x + (item.w / 2.0)) / gridSize);
            var y = overlayY + overlayHeaderH + (overlayH - overlayHeaderH) * ((item.y + (item.h / 2.0)) / gridSize);

            MouseHook.MoveMouse((int)x, (int)y);
        }
    }
}
