/*
# ======================= #
#   by AmeliePick. 2020   #
#  github.com/AmeliePick  #
# ======================= #
*/






using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace CustomPlayer
{
    public class Player_ : Script
    {
        public Player BasePlayer { private set; get; }
        public Model BaseModel { private set; get; }

        public Player_()
        {
            KeyDown += OnKeyDown;
            BasePlayer = Game.Player;
            BaseModel = BasePlayer.Character.Model;
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.K && Game.Player.Character.Model != "Va")
            {
                Player player = Game.Player;
                Ped playerPed = GTA.Native.Function.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX);
 
                player.ChangeModel(new Model("Va"));
                player.Character.Voice = "execpa_female";



                // Set the standing animation
                playerPed.MovementAnimationSet = "move_f@sexy@a";


                UI.ShowSubtitle("Model changed, all settings was apply");
            }
            else if(e.KeyCode == Keys.K && Game.Player.Character.Model == "Va")
            {
                Game.Player.ChangeModel(BaseModel);
                UI.ShowSubtitle("Initial model was return");
            }
        }
    }
}
