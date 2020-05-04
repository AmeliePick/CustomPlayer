using CustomPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPlayer_UserInterfaceModel
{
    public enum BodyPart { HEAD = 0, HAIR = 2, FACE = 1, EYES = 7, TORSO = 3, TORSO2 = 11, HANDS = 5, LEGS = 4, FEET = 6 };



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

        public string output { private set; get; }


        bool PlayerHasBeenChanged;
        public bool isPlayerChanged
        {
            private set
            {
                PlayerHasBeenChanged = value;
            }

            get
            {
                bool toReturn = PlayerHasBeenChanged;

                if(PlayerHasBeenChanged) PlayerHasBeenChanged = false;

                return toReturn;
            }
        }


        public UIModel()
        {
            game = new GameClass();

            listOfCharactersNames = new List<dynamic>();
            previousCountOfCharactersNamesList = 0;

            voiceList = new List<dynamic>(game.LoadVoiceList());

            getNumberOfComponents   = new getNumberOfComponentsVariation[2] { game.getNumberOfDrawable, game.getNumberOfTexture };
            getCurrentID            = new getCurrentIDVariation[2] { game.getCurrentDrawableID, game.getCurrentTextureID };

            PlayerHasBeenChanged = false;
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
                isPlayerChanged = true;
                output = "Character was loaded";
            }         
            else
                output = "Character was not loaded";
        }

        public void loadDefaultCharacter()
        {
            game.LoadDefaultPlayer();
            isPlayerChanged = true;
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
