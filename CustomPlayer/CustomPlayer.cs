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
    public enum PedVariationData
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


    public class CustomPlayer : Script
    {
        public Player BasePlayer { private set; get; }
        public Model BaseModel { private set; get; }


        MenuPool modMenuPool;
        UIMenu mainMenu;

        UIMenuItem saveCharacter;
        UIMenuItem customize;
        UIMenuItem about;



        public CustomPlayer()
        {
            // Setup the menu and menu's UI
            modMenuPool = new MenuPool();
            mainMenu = new UIMenu("Custom Player", "Select an opinion");
            modMenuPool.Add(mainMenu);

            UIMenu LoadCharacterMenu = modMenuPool.AddSubMenu(mainMenu, "Load", "Change your current player character to a saved character.");

            saveCharacter = new UIMenuItem("Save", "Save your current character in the collection.");
            mainMenu.AddItem(saveCharacter);

            customize = new UIMenuItem("Customize", "Change your character’s clothes, voice.");
            mainMenu.AddItem(customize);

            about = new UIMenuItem("About", "Mod information.");
            mainMenu.AddItem(about);


            mainMenu.OnItemSelect += onMainMenuItemSelect;


            KeyDown += OnKeyDown;
            Tick += OnTick;
            


            BasePlayer = Game.Player;
            BaseModel = BasePlayer.Character.Model;


            
        }

        void OnTick(object sender, EventArgs e)
        {
            if (modMenuPool != null)
                modMenuPool.ProcessMenus();
           
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10 && !modMenuPool.IsAnyMenuOpen())
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }


        void onMainMenuItemSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if(item == saveCharacter)
            {
                string inputText = GTA.Game.GetUserInput(WindowTitle.FMMC_KEY_TIP9N, 60);

                SaveClass.SaveCharacter(inputText);

                UI.ShowSubtitle("Done!");
            }
            else if(item == about)
            {
                BigMessageThread.MessageInstance.ShowSimpleShard("Custom Player",
                "Developed by AmeliePick — https://ameliepick.ml"); 
            }
        }



        void SaveCharacter()
        {
            
        }



    }
}
