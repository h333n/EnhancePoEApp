﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.Model
{
    public static class Coordinates
    {
        private static bool CheckForHit(Point pt, Button btn)
        {
            var clickX = MouseHook.ClickLocationX;
            var clickY = MouseHook.ClickLocationY;

            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            // +1 border thickness
            var btnX = Convert.ToInt32(Math.Ceiling(pt.X + btn.ActualWidth + 1));
            var btnY = Convert.ToInt32(Math.Ceiling(pt.Y + btn.ActualHeight + 1));

            if (clickX > pt.X
                && clickY > pt.Y
                && clickX < btnX
                && clickY < btnY)
                return true;

            return false;
        }

        private static Point GetCoordinates(Button item)
        {
            if (item != null)
            {
                var locationFromScreen = item.PointToScreen(new Point(0, 0));
                return locationFromScreen;
            }

            return new Point(0, 0);
        }

        private static bool CheckForHeaderHit(StashTab s)
        {
            var clickX = MouseHook.ClickLocationX;
            var clickY = MouseHook.ClickLocationY;

            var pt = GetTabHeaderCoordinates(s.TabHeader);

            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            // can be null if user closes overlay while fetching with stash tab overlay open
            if (s.TabHeader != null)
            {
                var tabX = Convert.ToInt32(Math.Floor(pt.X + s.TabHeader.ActualWidth + 1));
                var tabY = Convert.ToInt32(Math.Floor(pt.Y + s.TabHeader.ActualHeight + 1));


                if (clickX > pt.X
                    && clickY > pt.Y
                    && clickX < tabX
                    && clickY < tabY)
                    return true;
            }

            return false;
        }

        private static bool CheckForEditButtonHit(Button btn)
        {
            var clickX = MouseHook.ClickLocationX;
            var clickY = MouseHook.ClickLocationY;

            var pt = GetEditButtonCoordinates(btn);

            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            var btnX = Convert.ToInt32(Math.Floor(pt.X + btn.ActualWidth + 1));
            var btnY = Convert.ToInt32(Math.Floor(pt.Y + btn.ActualHeight + 1));

            if (clickX > pt.X
                && clickY > pt.Y
                && clickX < btnX
                && clickY < btnY)
                return true;

            return false;
        }

        private static Point GetTabHeaderCoordinates(TextBlock item)
        {
            if (item != null)
            {
                var locationFromScreen = item.PointToScreen(new Point(0, 0));
                return locationFromScreen;
            }

            return new Point(0, 0);
        }

        private static Point GetEditButtonCoordinates(Button button)
        {
            if (button != null)
            {
                var locationFromScreen = button.PointToScreen(new Point(0, 0));
                return locationFromScreen;
            }

            return new Point(0, 0);
        }

        private static List<Cell> GetAllActiveCells(int index)
        {
            var activeCells = new List<Cell>();

            foreach (var cell in StashTabList.StashTabs[index].OverlayCellsList)
                if (cell.Active)
                    activeCells.Add(cell);

            return activeCells;
        }

        public static void OverlayClickEvent(StashTabOverlayView stashTabOverlayView)
        {
            if (stashTabOverlayView.IsOpen)
            {
                var selectedIndex = stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex;
                var isHit = false;
                var hitIndex = -1;

                var activeCells = GetAllActiveCells(selectedIndex);

                var buttonList = new List<ButtonAndCell>();

                if (CheckForEditButtonHit(stashTabOverlayView.EditModeButton))
                    stashTabOverlayView.HandleEditButton(stashTabOverlayView);

                if (StashTabList.StashTabs[selectedIndex].Quad)
                {
                    var ctrl = stashTabOverlayView.StashTabOverlayTabControl.SelectedContent as QuadStashGrid;

                    foreach (var cell in activeCells)
                        buttonList.Add(new ButtonAndCell
                        {
                            Button = ctrl.GetButtonFromCell(cell),
                            Cell = cell
                        });

                    for (var b = 0; b < buttonList.Count; b++)
                        if (CheckForHit(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }

                    Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Quad Tab Current Tab Index: {stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex}");

                    if (isHit)
                    {
                        Data.ActivateNextCell(true, buttonList[hitIndex].Cell, stashTabOverlayView.StashTabOverlayTabControl);
                    }

                    for (var stash = 0; stash < StashTabList.StashTabs.Count; stash++)
                    {

                        if (CheckForHeaderHit(StashTabList.StashTabs[stash]))
                        {
                            stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex = stash;
                            // move mouse to item
                            if (Settings.Default.AutoMove)
                            {
                                var item = Data.CurrentlyHighlightedItem;
                                Console.WriteLine($"Move mouse to item {item} after stash tab hit (quad)");
                                MouseMover.MoveMouseToItem(item);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    var ctrl = stashTabOverlayView.StashTabOverlayTabControl.SelectedContent as NormalStashGrid;
                    foreach (var cell in activeCells)
                        buttonList.Add(new ButtonAndCell
                        {
                            Button = ctrl.GetButtonFromCell(cell),
                            Cell = cell
                        });

                    for (var b = 0; b < buttonList.Count; b++)
                        if (CheckForHit(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }

                    Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Normal Tab Current Tab Index: {stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex}");

                    if (isHit) Data.ActivateNextCell(true, buttonList[hitIndex].Cell, stashTabOverlayView.StashTabOverlayTabControl);

                    for (var stash = 0; stash < StashTabList.StashTabs.Count; stash++)
                        if (CheckForHeaderHit(StashTabList.StashTabs[stash]))
                        {
                            stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex = stash;
                            // move mouse to item
                            if (Settings.Default.AutoMove)
                            {
                                var item = Data.CurrentlyHighlightedItem;
                                Console.WriteLine($"Move mouse to item {item} after stash tab hit (non-quad)");
                                MouseMover.MoveMouseToItem(item);
                            }
                        }
                            
                }
            }
        }
    }

    internal class ButtonAndCell
    {
        public Button Button { get; set; }
        public Cell Cell { get; set; }
    }
}