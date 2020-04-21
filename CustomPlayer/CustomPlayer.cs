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
        SaveClass saveClass;
        LoadClass loadClass;



        MenuPool modMenuPool;
        UIMenu mainMenu;

        UIMenu LoadCharacterMenu;
        List<dynamic> ListOfNames;
        private int previousCountOfNamesList;
        UIMenuItem EmptyLoadingListButtonInfo;
        UIMenuListItem CharactersList;
        UIMenuItem LoadCharacter;
        UIMenuItem LoadDefaultPlayer;

        UIMenuItem saveCharacter;
        UIMenuItem customize;
        UIMenuItem about;



        public CustomPlayer()
        {
            // Setup the menu and menu's UI
            modMenuPool = new MenuPool();
            mainMenu = new UIMenu("Custom Player", "Select an opinion");
            modMenuPool.Add(mainMenu);



            // Init buttons

            SubMenuCharactersListSetup();

            saveCharacter = new UIMenuItem("Save", "Save your current character in the collection.");
            mainMenu.AddItem(saveCharacter);

            customize = new UIMenuItem("Customize", "Change your character’s clothes, voice.");
            mainMenu.AddItem(customize);

            about = new UIMenuItem("About", "Mod information.");
            mainMenu.AddItem(about);


            // Init events
            mainMenu.OnItemSelect += onMainMenuItemSelect;
            KeyDown += OnKeyDown;
            Tick += OnTick;


            // Init mod's classes
            saveClass = new SaveClass();
            loadClass = new LoadClass(Game.Player.Character.Model);
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
            /* In addition to updating the menu, there may be a "late" initialization of the "Load" menu. 
             * This is due to the fact that the first time you run the script, 
             * there may not be a file with saved characters.
             */
            UpdateLoadMenu();


            if (item == saveCharacter)
            {
                string inputText = GTA.Game.GetUserInput(WindowTitle.FMMC_KEY_TIP9N, 60);

                if(inputText != "")
                {
                    saveClass.SaveCharacter(inputText);

                    UI.ShowSubtitle("Done!");
                }
                else
                    UI.ShowSubtitle("Name can not be empty!");

            }
            else if(item == about)
            {
                BigMessageThread.MessageInstance.ShowSimpleShard("Custom Player",
                "Developed by AmeliePick — https://ameliepick.ml"); 
            } 
        }


        void LoadMenuInit()
        {
            if (EmptyLoadingListButtonInfo != null && LoadCharacterMenu.MenuItems.Count > 0)
            {
                LoadCharacterMenu.Clear();

                EmptyLoadingListButtonInfo = null;
            }

            CharactersList = new UIMenuListItem("Select your hero:", ListOfNames, 0, "Load the selected character.");
            LoadCharacterMenu.AddItem(CharactersList);

            LoadCharacter = new UIMenuItem("Load the character", "Load your saved character.");
            LoadCharacterMenu.AddItem(LoadCharacter);

            LoadDefaultPlayer = new UIMenuItem("Load the default", 
                                                "Load the default character, " +
                                               "If you are currently using the added character model, " +
                                               "then use this button to load the character model, " +
                                               "which was before loading the added characters.");

            LoadCharacterMenu.AddItem(LoadDefaultPlayer);
        }


        void UpdateLoadMenu()
        {
            ListOfNames.Clear();
            Parser.ParseCharactersNames(ListOfNames);

            // Menu rebuilding
            if (ListOfNames.Count > previousCountOfNamesList)
            {
                LoadCharacterMenu.Clear();

                LoadMenuInit();
            }


            previousCountOfNamesList = ListOfNames.Count;


        }


        void SubMenuCharactersListSetup()
        {
            this.LoadCharacterMenu = modMenuPool.AddSubMenu(mainMenu, "Load", "Change your current player character to a saved character.");
            EmptyLoadingListButtonInfo = null;

            ListOfNames = new List<dynamic>();
            previousCountOfNamesList = 0;

            UpdateLoadMenu();


            if (ListOfNames.Count == 0)
            {
                EmptyLoadingListButtonInfo = new UIMenuItem("Nothing to load", "You have no saved characters yet.");
                LoadCharacterMenu.AddItem(EmptyLoadingListButtonInfo);
            }



            LoadCharacterMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == LoadCharacter)
                {
                    bool check = loadClass.LoadCharacter(ListOfNames[CharactersList.Index]);

                    if (!check)
                        UI.ShowSubtitle("Character was not loaded");
                    else
                        UI.ShowSubtitle("Character was loaded");
                }
                else if(item == LoadDefaultPlayer)
                {
                    loadClass.LoadDefaultPlayer();

                    UI.ShowSubtitle("The default player was returned");
                }
            };

            
        }
    }
}
