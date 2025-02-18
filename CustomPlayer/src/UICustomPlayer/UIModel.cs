﻿using CustomPlayer;
using System;
using System.Collections.Generic;
using CustomPlayerGlobal;

namespace CustomPlayer_UserInterfaceModel
{
    class UIModel
    {
        private GameClass game;

        public delegate int getCurrentIDVariation(int componentID);
        public readonly getCurrentIDVariation[] getCurrentID;

        public delegate int getNumberOfComponentsVariation(int componentId);
        public readonly getNumberOfComponentsVariation[] getNumberOfComponents;


        public List<dynamic> listOfCharactersNames;
        public int previousCountOfCharactersNamesList;

        public List<dynamic> voiceList;

        public Dictionary<string, int> bodyPart;
        
        public string output { private set; get; }


        //------------------------------------------------------------------------------//

        
        
        public UIModel()
        {
            game = new GameClass();

            listOfCharactersNames = new List<dynamic>();
            previousCountOfCharactersNamesList = 0;

            voiceList = new List<dynamic>(game.LoadVoiceList());
            bodyPart = new Dictionary<string, int>
            {
                { "HEAD", 0 },
                { "HAIR", 2 },
                { "EYES", 7 },
                { "FACE", 1 },
                
                { "TORSO",3 },
                { "TORSO2",11 },
                { "HANDS",5 },
                { "LEGS", 4 },
                { "FEET", 6 },
                
                { "ACCESSORIES", 8 },
                { "TASKS", 9 },
                { "TEXTURES", 10 },     
            };

        
            getNumberOfComponents   = new getNumberOfComponentsVariation[2] { game.getNumberOfDrawable, game.getNumberOfTexture };
            getCurrentID            = new getCurrentIDVariation[2] { game.getCurrentDrawableID, game.getCurrentTextureID };
        }



        #region EVENTS
        public void OnTick(object sender, EventArgs e)
        {
            game.OnTick(sender, e);
        }
        #endregion


        public bool isDefaultPlayer()
        {
            return game.isDefaultPlayer();
        }


        /// <summary>
        /// Performs the list update.
        /// </summary>
        /// <returns>Returns true, if the list was changed after the updating.</returns>
        public bool updateCharactersNames()
        {
            bool isChanged = false;

            listOfCharactersNames.Clear();
            Parser.ParseCharactersNames(listOfCharactersNames);

            if (listOfCharactersNames.Count > previousCountOfCharactersNamesList)
                isChanged = true;

            previousCountOfCharactersNamesList = listOfCharactersNames.Count;

            return isChanged;
        }


        /// <summary>
        /// Get user input from the game console.
        /// </summary>
        public string getUserInput()
        {
            return GTA.Game.GetUserInput(GTA.WindowTitle.FMMC_KEY_TIP8, 30);
        }


        //------------------------------------------------------------------------------//


        public void saveCharacter(string CharacterName)
        {
            if (CharacterName != "")
            {
                bool isSaved = game.saveCharacter(CharacterName);

                if (isSaved)
                    output = "Done!";
                else output = "The player with the same name already exists.";
            }
            else
                output = "Name can not be empty!";
        }


        public void loadCharacter(string CharacterName)
        {
            bool checkLoad = game.loadCharacter(CharacterName);

            if (checkLoad) output = "Character was loaded";
            else output = "Character was not loaded";
        }


        public void loadDefaultCharacter()
        {
            game.loadDefaultPlayer();
        }


        //------------------------------------------------------------------------------//


        public void setDrawableID(int componentId, int value)
        {
            game.setDrawableID(componentId, value);
        }


        public void setTextureID(int componentId, int value)
        {
            game.setTextureID(componentId, value);
        }


        //------------------------------------------------------------------------------//


        public void playSpeechByPlayer(string voice)
        {
            game.playSpeechByPlayer(voice);
        }


        public void setNewVoice(string voiceName)
        {
            game.setNewVoice(voiceName);
        }
    }
}
