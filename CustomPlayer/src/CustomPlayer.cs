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
using CustomPlayer_UserInterface;

namespace CustomPlayer
{
    public class CustomPlayer : Script
    {
        UICustomPlayer UICustomPlayer;


        //----------------------------------------//


        public CustomPlayer()
        {
            UICustomPlayer = new UICustomPlayer();

            // Init events
            KeyDown += OnKeyDown;
            Tick += OnTick;
        }



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
    }
}
