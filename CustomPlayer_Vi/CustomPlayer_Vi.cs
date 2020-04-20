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
using System.Windows.Forms;
using GTAN = GTA.Native.Function;





namespace CustomPlayer_Vi
{
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
                Game.Player.ChangeModel(new Model("Va"));

                Ped playerPed = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX);

                // Change walk/standing/movement animations
                GTAN.Call(Hash.REQUEST_ANIM_SET, "move_f@multiplayer");
                Wait(1500);
                GTAN.Call(Hash.SET_PED_MOVEMENT_CLIPSET, playerPed, "move_f@multiplayer", 0.1);


                // Change the model's skin
                GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, playerPed, (int)PedVariationData.PED_VARIATION_HAIR, 0, 2, 2);
                GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, playerPed, (int)PedVariationData.PED_VARIATION_HEAD, 0, 1, 2);
                GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, playerPed, (int)PedVariationData.PED_VARIATION_TORSO, 1, 0, 2);
                GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, playerPed, (int)PedVariationData.PED_VARIATION_ACCESSORIES, 1, 0, 2);

                // For V model the parameter PED_VARIATION_LEGS changes the lower clothes components
                GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, playerPed, (int)PedVariationData.PED_VARIATION_LEGS, 2, 0, 2);


                playerPed.Voice = "S_F_Y_HOOKER_01_WHITE_FULL_01";


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
