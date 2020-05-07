using System.Windows.Forms;
using NativeUI;
using System;
using System.Collections.Generic;
using CustomPlayer_UserInterfaceModel;
using CustomPlayerGlobal;

namespace CustomPlayer_UserInterface
{
    /// <summary>
    /// Storage for UIMenu components.
    /// </summary>
    class UIMenuComponents
    {
        public UIMenu instance { get; }

        public string MenuTitle { get; }

        public int TitleToCharatcerComponentID { get; }


        public UIMenuComponents(UIMenu menu, string menuTitle, int characterComponentId)
        {
            this.instance   = menu;
            this.MenuTitle  = menuTitle;
            this.TitleToCharatcerComponentID = characterComponentId;
        }
    }


    /// <summary>
    /// User Interface
    /// </summary>
    class UICustomPlayer
    {
        UIModel UIModel;

        MenuPool modMenuPool;
        UIMenu mainMenu;

        // Load Menu
        UIMenu UILoadMenu;
        UIMenuItem UILoadMenuEmptyButton;
        UIMenuListItem UICharactersList;
        UIMenuItem UILoadCharacter;
        UIMenuItem UILoadDefaultPlayer;

        UIMenuItem UIsaveCharacter;

        UIMenu UIcustomizeMenu;
        UIMenu UIclothingMenu;

        UIMenuItem about;


        List<UIMenuComponents> ClothingMenuItems;

        
        //---------------------------------------//
        
        
        
        public UICustomPlayer()
        {
            // Init mod's classes
            UIModel = new UIModel();
            PlayerChangedEvent.AddHandler(isPlayerChanged);

            // Setup the menu and menu's UI
            modMenuPool = new MenuPool();
            mainMenu = new UIMenu("Custom Player", "Select an opinion");
            modMenuPool.Add(mainMenu);



            // Init buttons

            SubMenuCharactersListSetup();

            UIsaveCharacter = new UIMenuItem("Save", "Save your current character in the collection.");
            mainMenu.AddItem(UIsaveCharacter);

            ClothingMenuItems = new List<UIMenuComponents>();
            SubMenuCustomizeSetup();

            about = new UIMenuItem("About", "Mod information.");
            mainMenu.AddItem(about);

            modMenuPool.RefreshIndex();

            // Init events
            mainMenu.OnItemSelect += onMainMenuItemSelect;
        }



        #region EVENTS
        public void OnTick(object sender, EventArgs e)
        {
            UIModel.OnTick(sender, e);

            if (modMenuPool != null)
                modMenuPool.ProcessMenus();
        }


        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10 && !modMenuPool.IsAnyMenuOpen())
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }


        public void isPlayerChanged()
        {
            UpdateClothingMenu();
        }



        void onMainMenuItemSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == UIsaveCharacter)
            {
                string inputText = UIModel.getUserInput();
                UIModel.saveCharacter(inputText);

                GTA.UI.ShowSubtitle(UIModel.output);
            }
            else if (item == about)
            {
                BigMessageThread.MessageInstance.ShowSimpleShard("Custom Player",
                "Developed by AmeliePick — https://ameliepick.ml");
            }
        }
        #endregion



        #region LOAD
        void LoadMenuInit()
        {
            if (UILoadMenuEmptyButton != null)
            {
                UILoadMenu.Clear();

                UILoadMenuEmptyButton = null;
            }

            UICharactersList = new UIMenuListItem("Select your hero:", UIModel.listOfCharactersNames, 0, "Load the selected character.");
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
            // If true, to do rebuilding of the menu
            if (UIModel.updateCharactersNames())
            {
                if (UILoadMenu.MenuItems.Count >= 0 || UILoadMenu.MenuItems.Count == 1)
                {
                    UILoadMenu.Clear();
                    LoadMenuInit();
                }
            }     
        }


        void SubMenuCharactersListSetup()
        {
            this.UILoadMenu = modMenuPool.AddSubMenu(mainMenu, "Load", "Change your current player character to a saved character.");


            UpdateLoadMenu();


            if (UIModel.listOfCharactersNames.Count == 0)
            {

                UILoadMenuEmptyButton = new UIMenuItem("", "You have no saved characters yet.");
                UILoadMenu.AddItem(UILoadMenuEmptyButton);
            }



            UILoadMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == UILoadCharacter)
                {
                    UIModel.loadCharacter(UIModel.listOfCharactersNames[UICharactersList.Index]);

                    /* In addition to updating the menu, there may be a "late" initialization of the "Load" menu. 
                     * This is due to the fact that the first time you run the script, 
                     * there may not be a file with saved characters.
                     */
                    UpdateLoadMenu();

                    GTA.UI.ShowSubtitle(UIModel.output);
                }
                else if (item == UILoadDefaultPlayer)
                {
                    UIModel.loadDefaultCharacter();

                    GTA.UI.ShowSubtitle("The default player was returned");
                }
            };
        }
        #endregion


        
        #region CUSTOMIZE

        // EVENTS //
        int FindComponentID(UIMenu desired)
        {
            foreach (var element in ClothingMenuItems)
            {
                if (element.instance == desired)
                {
                    return element.TitleToCharatcerComponentID;
                }
            }
            return 0;
        }


        void setTexture(UIMenuListItem sender, int newIndex)
        {
            UIModel.setTextureID(FindComponentID(sender.Parent), sender.IndexToItem(newIndex));
        }


        void setDrawable(UIMenuListItem sender, int newIndex)
        {
            UIModel.setDrawableID(FindComponentID(sender.Parent), sender.IndexToItem(newIndex));
        }
        //--------------------------------------------------------//

        
        
        
        #region CLOTHING
        void ClothingMenuInit(UIMenu SubmenuAddTo, KeyValuePair<string, int> bodyPart)
        {
            // SubMenu creating
            UIMenuItem _UIMenuItem = new UIMenuItem(bodyPart.Key);
            SubmenuAddTo.AddItem(_UIMenuItem);


            UIMenu submenu = new UIMenu(_UIMenuItem.Text, _UIMenuItem.Text);
            ClothingMenuItems.Add(new UIMenuComponents(submenu, bodyPart.Key, bodyPart.Value));



            const int NumberOfLists = 2;
            string[] NamesOfLists = new string[NumberOfLists] { "Drawable", "Texture" };


            List<dynamic> componentsIDs = new List<dynamic>();

            // Add a list of components IDs to the submenu
            for (int it = 0; it < NumberOfLists; ++it)
            {
                for (int num = 0; num < UIModel.getNumberOfComponents[it](bodyPart.Value); num++)
                {
                    componentsIDs.Add(num);
                }

                if (componentsIDs.Count > 1)
                {
                    // Create the UI list of components
                    UIMenuListItem UIListInSubMenu = new UIMenuListItem(NamesOfLists[it], componentsIDs, componentsIDs.IndexOf(UIModel.getCurrentID[it](bodyPart.Value)));

                    if (it == 0)
                        UIListInSubMenu.OnListChanged += setDrawable;
                    else
                        UIListInSubMenu.OnListChanged += setTexture;

                    submenu.AddItem(UIListInSubMenu);
                }
                
                componentsIDs.Clear();
            }

            if (submenu.MenuItems.Count == 0)
                _UIMenuItem.Enabled = false;

            modMenuPool.Add(submenu);
            SubmenuAddTo.BindMenuToItem(submenu, _UIMenuItem);
        }


        void ClothingMenuSetup()
        {
            // Add submenus with elements
            foreach(var pair in UIModel.bodyPart)
            {
                ClothingMenuInit(UIclothingMenu, pair);
            }
        }


        void UpdateClothingMenu()
        {
            const int NumberOfLists = 2;
            string[] NamesOfLists = new string[NumberOfLists] { "Drawable", "Texture" };
            List<UIMenuListItem> listItems = new List<UIMenuListItem>(NumberOfLists);

            

            for (int i = 0; i < ClothingMenuItems.Count; ++i)
            {
                // Working with each sub menu

                List<dynamic> componentsIDs = new List<dynamic>();

                int NumberOfID = 0;

                // Create the UI list
                for (int it = 0; it < NumberOfLists; ++it)
                {
                    // Add a list of components IDs to the submenu
                    NumberOfID = UIModel.getNumberOfComponents[it](ClothingMenuItems[i].TitleToCharatcerComponentID);
                    for (int num = 0; num < NumberOfID; num++)
                    {
                        componentsIDs.Add(num);
                    }

                    if (componentsIDs.Count > 1)
                    {
                        // Create UI list  
                        UIMenuListItem UIList = new UIMenuListItem(NamesOfLists[it], componentsIDs, UIModel.getCurrentID[it](ClothingMenuItems[i].TitleToCharatcerComponentID));

                        // Set events
                        if (it == 0)
                            UIList.OnListChanged += setDrawable;
                        else
                            UIList.OnListChanged += setTexture;

                        listItems.Add(UIList);
                    }

                    componentsIDs.Clear();
                }

                // Add UI lists to the sub menu
                for (int j = 0; j < UIclothingMenu.MenuItems.Count; ++j)
                {
                    if (ClothingMenuItems[i].MenuTitle == UIclothingMenu.MenuItems[j].Text)
                    {
                        if (listItems.Count > 0)
                        {
                            ClothingMenuItems[i].instance.MenuItems.Clear();
                            for(int l = 0; l < listItems.Count; ++l)
                            {
                                ClothingMenuItems[i].instance.AddItem(listItems[l]);
                            }

                            if (UIclothingMenu.MenuItems[j].Enabled == false)
                                UIclothingMenu.MenuItems[j].Enabled = true;
                        }
                        else if(listItems.Count == 0)
                        {
                            // Set menu as non enabled
                            UIclothingMenu.MenuItems[j].Enabled = false;
                        }

                        break;
                    }
                }

                listItems.Clear();
            }
        }
        #endregion



        void SubMenuCustomizeSetup()
        {
            UIcustomizeMenu = modMenuPool.AddSubMenu(mainMenu, "Customize");


            // VOICE MENU //
            UIMenu UIvoiceMenu = modMenuPool.AddSubMenu(UIcustomizeMenu, "Voice", "Change character's voice.");
            UIvoiceMenu.SetMenuWidthOffset(100);


            UIMenuListItem voiceChangeList = new UIMenuListItem("", UIModel.voiceList, 0, "Change character's voice.");
            UIvoiceMenu.AddItem(voiceChangeList);


            UIMenuItem voiceTest = new UIMenuItem("Test the voice", "The player will say the phrase in the selected voice.");
            UIvoiceMenu.AddItem(voiceTest);

            UIMenuItem applyNewVoice = new UIMenuItem("Apply", "Set to your character the selected vocie.");
            UIvoiceMenu.AddItem(applyNewVoice);


            UIvoiceMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == voiceTest)
                {
                    UIModel.playSpeechByPlayer(UIModel.voiceList[voiceChangeList.Index]);
                }
                else if (item == applyNewVoice)
                {
                    UIModel.setNewVoice(UIModel.voiceList[voiceChangeList.Index]);
                }
            };


            // CLOTHING MENU //
            UIclothingMenu = modMenuPool.AddSubMenu(UIcustomizeMenu, "Clothing", "Change the clothes of the model, if she has one");
            ClothingMenuSetup();
        }
        #endregion
    }
}
