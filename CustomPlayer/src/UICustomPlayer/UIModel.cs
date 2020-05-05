using CustomPlayer;
using System;
using System.Collections.Generic;

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


        public delegate void isPlayerChanged();
        public event isPlayerChanged playerChangedNotify;


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



        public void OnTick(object sender, EventArgs e)
        {
            game.OnTick(sender, e);
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
            return GTA.Game.GetUserInput(GTA.WindowTitle.FMMC_KEY_TIP9N, 60);
        }


        public void saveCharacter(string CharacterName)
        {
            if (CharacterName != "")
            {
                game.SaveCharacter(CharacterName);

                output = "Done!";
            }
            else
                output = "Name can not be empty!";
        }

        public void loadCharacter(string CharacterName)
        {
            bool checkLoad = game.LoadCharacter(CharacterName);

            if (checkLoad)
            {
                playerChangedNotify?.Invoke();
                output = "Character was loaded";
            }         
            else
                output = "Character was not loaded";
        }

        public void loadDefaultCharacter()
        {
            game.LoadDefaultPlayer();
            playerChangedNotify?.Invoke();
        }


        public void setDrawableID(int componentId, int value)
        {
            game.setDrawableID(componentId, value);
        }

        public void setTextureID(int componentId, int value)
        {
            game.setTextureID(componentId, value);
        }



        public void playSpeechByPlayer(string voice)
        {
            game.PlaySpeechByPlayer(voice);
        }

        public void setNewVoice(string voiceName)
        {
            game.SetNewVoice(voiceName);
        }
    }
}
