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



    public class CustomPlayer : Script
    {
        GameClass gameClass;



        MenuPool modMenuPool;
        UIMenu mainMenu;

        // Load Menu
        UIMenu UILoadMenu;
        List<dynamic> ListOfNames;
        private int previousCountOfNamesList;
        UIMenuItem UILoadMenuEmptyButton;
        UIMenuListItem UICharactersList;
        UIMenuItem UILoadCharacter;
        UIMenuItem UILoadDefaultPlayer;

        UIMenuItem saveCharacter;
        UIMenu customizeMenu;
        UIMenuItem about;

        enum BodyPart { HEAD = 0, HAIR = 2, FACE = 1, EYES = 7, TORSO = 3, TORSO2 = 11, HANDS = 5, LEGS = 4, FEET = 6 };

        public CustomPlayer()
        {
            // Init mod's classes
            gameClass = new GameClass();

            // Setup the menu and menu's UI
            modMenuPool = new MenuPool();
            mainMenu = new UIMenu("Custom Player", "Select an opinion");
            modMenuPool.Add(mainMenu);



            // Init buttons

            SubMenuCharactersListSetup();

            saveCharacter = new UIMenuItem("Save", "Save your current character in the collection.");
            mainMenu.AddItem(saveCharacter);

            SubMenuCustomizeSetup();

            about = new UIMenuItem("About", "Mod information.");
            mainMenu.AddItem(about);


            // Init events
            mainMenu.OnItemSelect += onMainMenuItemSelect;
            KeyDown += OnKeyDown;
            Tick += OnTick;
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

                if (inputText != "")
                {
                    gameClass.SaveCharacter(inputText);

                    UI.ShowSubtitle("Done!");
                }
                else
                    UI.ShowSubtitle("Name can not be empty!");

            }
            else if (item == about)
            {
                BigMessageThread.MessageInstance.ShowSimpleShard("Custom Player",
                "Developed by AmeliePick — https://ameliepick.ml");
            }
        }


        void LoadMenuInit()
        {
            if (UILoadMenuEmptyButton != null)
            {
                UILoadMenu.Clear();

                UILoadMenuEmptyButton = null;
            }

            UICharactersList = new UIMenuListItem("Select your hero:", ListOfNames, 0, "Load the selected character.");
            UILoadMenu.AddItem(UICharactersList);

            UILoadCharacter = new UIMenuItem("Load the character", "Load your saved character.");
            UILoadMenu.AddItem(UILoadCharacter);

            UILoadDefaultPlayer = new UIMenuItem("Load the default",
                                               "Load the default character, " +
                                               "If you are currently using the added character model, " +
                                               "then use this button to load the character model, " +
                                               "which was before loading the added characters.");
            UILoadMenu.AddItem(UILoadDefaultPlayer);
        }


        void UpdateLoadMenu()
        {
            ListOfNames.Clear();
            Parser.ParseCharactersNames(ListOfNames);

            // Menu rebuilding
            if (ListOfNames.Count > previousCountOfNamesList)
            {
                List<dynamic> MenuItems = new List<dynamic>(UILoadMenu.MenuItems);

                UILoadMenu.Clear();
                if (MenuItems.Count == 1)
                {
                    LoadMenuInit();
                }
                else if (MenuItems.Count > 1)
                {
                    MenuItems.RemoveAt(0);
                    MenuItems.Insert(0, new UIMenuListItem("Select your hero:", ListOfNames, 0, "Load the selected character."));

                    foreach (UIMenuItem item in MenuItems)
                    {
                        UILoadMenu.AddItem(item);
                    }
                }
            }


            previousCountOfNamesList = ListOfNames.Count;
        }


        void SubMenuCharactersListSetup()
        {
            this.UILoadMenu = modMenuPool.AddSubMenu(mainMenu, "Load", "Change your current player character to a saved character.");


            ListOfNames = new List<dynamic>();
            previousCountOfNamesList = 0;

            UpdateLoadMenu();


            if (ListOfNames.Count == 0)
            {

                UILoadMenuEmptyButton = new UIMenuItem("", "You have no saved characters yet.");
                UILoadMenu.AddItem(UILoadMenuEmptyButton);
            }



            UILoadMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == UILoadCharacter)
                {
                    bool check = gameClass.LoadCharacter(ListOfNames[UICharactersList.Index]);

                    if (!check)
                        UI.ShowSubtitle("Character was not loaded");
                    else
                        UI.ShowSubtitle("Character was loaded");
                }
                else if (item == UILoadDefaultPlayer)
                {
                    gameClass.LoadDefaultPlayer();

                    UI.ShowSubtitle("The default player was returned");
                }
            };


        }


        void SubMenuCustomizeSetup()
        {
            customizeMenu = modMenuPool.AddSubMenu(mainMenu, "Customize");


            // VOICE MENU //
            UIMenu UIvoiceMenu = modMenuPool.AddSubMenu(customizeMenu, "Voice", "Change character's voice.");
            UIvoiceMenu.SetMenuWidthOffset(100);



            List<dynamic> voiceList = new List<dynamic>(gameClass.LoadVoiceList());

            UIMenuListItem voiceChangeList = new UIMenuListItem("", voiceList, 0, "Change character's voice.");
            UIvoiceMenu.AddItem(voiceChangeList);


            UIMenuItem voiceTest = new UIMenuItem("Test the voice", "The player will say the phrase in the selected voice.");
            UIvoiceMenu.AddItem(voiceTest);

            UIMenuItem applyNewVoice = new UIMenuItem("Apply", "Set to your character the selected vocie.");
            UIvoiceMenu.AddItem(applyNewVoice);


            UIvoiceMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == voiceTest)
                {
                    gameClass.PlaySpeechByPlayer(voiceList[voiceChangeList.Index]);
                }
                else if (item == applyNewVoice)
                {
                    gameClass.SetNewVoice(voiceList[voiceChangeList.Index]);
                }
            };



            // CLOTHING MENU //
            UIMenu UIclothingMenu = modMenuPool.AddSubMenu(customizeMenu, "Clothing", "Change the clothes of the model, if she has one");

            // Add submenus with elements
            BodyPart bodyPart = BodyPart.HEAD;
            for(int i = 0; i < 1; ++i)
            {
                UIMenuItem _UIMenuItem = new UIMenuItem(bodyPart.ToString());
                UIclothingMenu.AddItem(_UIMenuItem);

                UIMenu submenu = new UIMenu(_UIMenuItem.Text, _UIMenuItem.Text);


                
                List<dynamic> clothingDrawID = new List<dynamic>(gameClass.getNumberOfDrawable((int)bodyPart));
                for (int num = 0; num < clothingDrawID.Capacity; num++)
                {
                    clothingDrawID.Add(num);
                }
                UIMenuListItem drawableID = new UIMenuListItem("Drawable", clothingDrawID, clothingDrawID.IndexOf(gameClass.getCurrentDrawableID((int)bodyPart)));

                if (clothingDrawID.Count < 2) drawableID.Enabled = false;
                else
                {
                    drawableID.OnListChanged += (sender, newIndex) =>
                    {
                        string componentTitleID = "";
                        foreach (var element in UIclothingMenu.MenuItems)
                        {
                            
                        }

                        gameClass.setDrawableID(componentId: (int)Enum.Parse(typeof(BodyPart), componentTitleID),
                                            value: sender.IndexToItem(sender.Index));
                    };
                }

                
                submenu.AddItem(drawableID);



                List<dynamic> clothingTextureID = new List<dynamic>(gameClass.getNumberOfTexture((int)bodyPart));
                for (int num = 0; num < clothingTextureID.Capacity; num++)
                {
                    clothingTextureID.Add(num);
                }
                UIMenuListItem textureID = new UIMenuListItem("Texture", clothingTextureID, clothingTextureID.IndexOf(gameClass.getCurrentTextureID((int)bodyPart)));

                if (clothingTextureID.Count < 2) textureID.Enabled = false;
                else
                {
                    textureID.OnListChanged += (sender, newIndex) =>
                    {
                        gameClass.setTextureID(componentId: (int)Enum.Parse(typeof(BodyPart), sender.IndexToItem(sender.Index)),
                                               value: sender.IndexToItem(sender.Index));
                    };
                }


                submenu.AddItem(textureID);

                modMenuPool.Add(submenu);
                UIclothingMenu.BindMenuToItem(submenu, _UIMenuItem);

                ++bodyPart;
            }

            


            UIclothingMenu.OnItemSelect += (sender, item, index) =>
            {
                
            };
        }
    }
}
