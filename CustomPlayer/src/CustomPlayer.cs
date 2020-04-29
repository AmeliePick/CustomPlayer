/*
# ======================= #
#   by AmeliePick. 2020   #
#  github.com/AmeliePick  #
# ======================= #
*/





using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using System;
using System.Collections.Generic;

using GTAN = GTA.Native.Function;



namespace CustomPlayer
{
    class UIMenuComponents
    {
        // It's needs because we have a fucking stupidest menu in NativeUI.
        public UIMenu instance { get; }
        public string MenuTitle { get; }


        public UIMenuComponents(UIMenu menu, string menuTitle)
        {
            this.instance = menu;
            this.MenuTitle = menuTitle;
        }


    }


    public class CustomPlayer : Script
    {
        GameClass gameClass;

        UICustomPlayer UICustomPlayer;


        // EVENTS //
        #region EVENTS
        void OnTick(object sender, EventArgs e)
        {
            UICustomPlayer.OnTick(sender, e);

        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                UICustomPlayer.OnKeyDown(sender, e);
            }
        }
        #endregion


        public CustomPlayer()
        {
            // Init mod's classes
            gameClass = new GameClass();

            UICustomPlayer = new UICustomPlayer();

            // Init events
            KeyDown += OnKeyDown;
            Tick += OnTick;
        }
    }
}
