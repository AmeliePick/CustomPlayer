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

        delegate int getCurrentID(int componentID);
        delegate void setID(int componentId, int value);
        delegate int getNumMethods(int componentId);

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
        UIMenu UIclothingMenu;

        UIMenuItem about;


        List<UIMenuComponents> ClothingMenuItems;

        enum BodyPart { HEAD = 0, HAIR = 2, FACE = 1, EYES = 7, TORSO = 3, TORSO2 = 11, HANDS = 5, LEGS = 4, FEET = 6 };

        // EVENTS //
        #region EVENTS
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


        void setTexture(UIMenuListItem sender, int newIndex)
        {
            gameClass.setTextureID((int)Enum.Parse(typeof(BodyPart), FindComponentID(sender.Parent)), sender.IndexToItem(newIndex));
        }

        void setDrawable(UIMenuListItem sender, int newIndex)
        {
            gameClass.setDrawableID((int)Enum.Parse(typeof(BodyPart), FindComponentID(sender.Parent)), sender.IndexToItem(newIndex));
        }
        #endregion



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

            ClothingMenuItems = new List<UIMenuComponents>();
            SubMenuCustomizeSetup();

            about = new UIMenuItem("About", "Mod information.");
            mainMenu.AddItem(about);

            modMenuPool.RefreshIndex();

            // Init events
            mainMenu.OnItemSelect += onMainMenuItemSelect;
            KeyDown += OnKeyDown;
            Tick += OnTick;
        }



        // LOAD MENU //
        #region LOAD
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
                if (UILoadMenu.MenuItems.Count == 0 || UILoadMenu.MenuItems.Count == 1)
                {
                    UILoadMenu.Clear();
                    LoadMenuInit();
                }
                else if (UILoadMenu.MenuItems.Count > 1)
                {
                    UILoadMenu.MenuItems.RemoveAt(0);
                    UILoadMenu.MenuItems.Insert(0, new UIMenuListItem("Select your hero:", ListOfNames, 0, "Load the selected character."));

                    foreach (UIMenuItem item in UILoadMenu.MenuItems)
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
                    {
                        UpdateClothingMenu();

                        UI.ShowSubtitle("Character was loaded");
                    }
                }
                else if (item == UILoadDefaultPlayer)
                {
                    gameClass.LoadDefaultPlayer();

                    UI.ShowSubtitle("The default player was returned");
                }
            };


        }
        #endregion



        // CUSTOMIZE MENU //
        #region CUSTOMIZE
        string FindComponentID(UIMenu desired)
        {
            string componentTitleID = "";
            foreach (var element in ClothingMenuItems)
            {
                if (element.instance == desired)
                {
                    componentTitleID = element.MenuTitle;
                }
            }

            return componentTitleID;
        }


        // CLOTHING MENU //
        #region CLOTHING
        void ClothingMenuInit(UIMenu SubmenuAddTo, BodyPart bodyPart)
        {
            // SubMenu creating
            UIMenuItem _UIMenuItem = new UIMenuItem(bodyPart.ToString());
            SubmenuAddTo.AddItem(_UIMenuItem);
            

            UIMenu submenu = new UIMenu(_UIMenuItem.Text, _UIMenuItem.Text);

            ClothingMenuItems.Add(new UIMenuComponents(submenu, bodyPart.ToString()));



            const int NumberOfLists = 2;
            string[] NamesOfLists = new string[NumberOfLists] { "Drawable", "Texture" };

            getCurrentID[] getMethods = new getCurrentID[NumberOfLists] { gameClass.getCurrentDrawableID, gameClass.getCurrentTextureID };
            getNumMethods[] getNums = new getNumMethods[NumberOfLists] { gameClass.getNumberOfDrawable, gameClass.getNumberOfTexture };


            // Get components IDs
            List<dynamic> componentsIDs = new List<dynamic>();

            // Add a list of components IDs to the submenu
            for (int it = 0; it < NumberOfLists; ++it)
            {    
                for (int num = 0; num < getNums[it]((int)bodyPart); num++)
                {
                    componentsIDs.Add(num);
                }

                if (componentsIDs.Count > 0)
                {
                    // Create the UI list of components
                    UIMenuListItem UIListInSubMenu = new UIMenuListItem(NamesOfLists[it], componentsIDs, componentsIDs.IndexOf(getMethods[it]((int)bodyPart)));

                    if (componentsIDs.Count < 2) UIListInSubMenu.Enabled = false;
                    else
                    {
                        if (it == 0)
                            UIListInSubMenu.OnListChanged += setDrawable;
                        else
                            UIListInSubMenu.OnListChanged += setTexture;


                    }

                    submenu.AddItem(UIListInSubMenu);
                }
                else if(submenu.MenuItems.Count == 0)
                    _UIMenuItem.Enabled = false;

                componentsIDs.Clear();
            }
            modMenuPool.Add(submenu);
            SubmenuAddTo.BindMenuToItem(submenu, _UIMenuItem);
        }

        void ClothingMenuSetup()
        {
            // Add submenus with elements
            BodyPart bodyPart = BodyPart.HEAD;
            for (int i = 0; i < 9; ++i)
            {
                ClothingMenuInit(UIclothingMenu, bodyPart);

                ++bodyPart;
            }
        }

        void UpdateClothingMenu()
        {
            const int NumberOfLists = 2;
            string[] NamesOfLists = new string[NumberOfLists] { "Drawable", "Texture" };
            List<UIMenuListItem> listItems = new List<UIMenuListItem>(NumberOfLists);

            getCurrentID[] getMethods = new getCurrentID[NumberOfLists] { gameClass.getCurrentDrawableID, gameClass.getCurrentTextureID };
            getNumMethods[] getNums = new getNumMethods[NumberOfLists] { gameClass.getNumberOfDrawable, gameClass.getNumberOfTexture };


            for (int i = 0; i < ClothingMenuItems.Count; ++i)
            {
                // Working with each sub menu


                // Create the UI list
                for (int it = 0; it < NumberOfLists; ++it)
                {
                    // Add a list of components IDs to the submenu
                    List<dynamic> componentsIDs = new List<dynamic>();

                    int NumberOfDrawable = getNums[it]((int)Enum.Parse(typeof(BodyPart), ClothingMenuItems[i].MenuTitle));
                    for (int num = 0; num < NumberOfDrawable; num++)
                    {
                        componentsIDs.Add(num);
                    }

                    if (componentsIDs.Count > 0)
                    {
                        // Create UI list
                        UIMenuListItem UIList = new UIMenuListItem(NamesOfLists[it], componentsIDs, getMethods[it]((int)Enum.Parse(typeof(BodyPart), ClothingMenuItems[i].MenuTitle)));
                        if (componentsIDs.Count == 1)
                            UIList.Enabled = false;

                        // Set events
                        if (it == 0)
                            UIList.OnListChanged += setDrawable;
                        else
                            UIList.OnListChanged += setTexture;

                        listItems.Add(UIList);
                    }
                }

                // Add UI lists to the sub menu
                for (int j = 0; j < UIclothingMenu.MenuItems.Count; ++j)
                {
                    if (ClothingMenuItems[i].MenuTitle == UIclothingMenu.MenuItems[j].Text)
                    {
                        if (listItems.Count == 2)
                        {
                            ClothingMenuItems[i].instance.MenuItems.Clear();
                            ClothingMenuItems[i].instance.AddItem(listItems[0]);
                            ClothingMenuItems[i].instance.AddItem(listItems[1]);
                        }
                        else
                        {
                            UIclothingMenu.MenuItems[j].Enabled = false;
                        }

                        break;
                    }
                }

                listItems.Clear();
            }
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
            UIclothingMenu = modMenuPool.AddSubMenu(customizeMenu, "Clothing", "Change the clothes of the model, if she has one");
            ClothingMenuSetup();




            UIclothingMenu.OnItemSelect += (sender, item, index) =>
            {
                
            };
        }
        #endregion
        #endregion
    }
}
