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


using GTAN = GTA.Native.Function;


enum PedVariationData
{
    PED_VARIATION_HEAD = 0,
    PED_VARIATION_FACE = 1,
    PED_VARIATION_HAIR = 2,
    PED_VARIATION_TORSO = 3,
    PED_VARIATION_LEGS = 4,
    PED_VARIATION_HANDS = 5,
    PED_VARIATION_FEET = 6,
    PED_VARIATION_EYES = 7,
    PED_VARIATION_ACCESSORIES = 8,
    PED_VARIATION_TASKS = 9,
    PED_VARIATION_TEXTURES = 10,
    PED_VARIATION_TORSO2 = 11
};


namespace CustomPlayer_Vi
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
                
 
                player.ChangeModel(new Model("Va"));
                player.Character.Voice = "A_F_M_BEACH_01_WHITE_FULL_01";


                Ped playerPed = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX);

                



                // Change walk/standing/movement animations
                GTAN.Call(Hash.HAS_ANIM_SET_LOADED, "move_f@sexy@a");

                if( GTAN.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "move_f@sexy@a") )
                {
                    GTAN.Call(Hash.SET_PED_MOVEMENT_CLIPSET, playerPed, "move_f@sexy@a", 0.1);
                    

                    UI.ShowSubtitle("Anim changed");
                    Wait(1000);
                }

                




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
