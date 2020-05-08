using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using System;
using System.Collections.Generic;

using GTAN = GTA.Native.Function;

using CustomPlayerGlobal;


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


    
    class GameClass
    {
        private SaveClass saveClass { get; }

        private LoadClass loadClass { get; }

        public Player player;
        public Ped playerPed;
        int modelHash;

        GamePlayer playerToSave;


        //--------------------------------------------------//

        
        
        public GameClass()
        {
            player = new Player(Game.Player.Handle);
            playerPed = new Ped(GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX).Handle);
            modelHash = playerPed.Model.Hash;
            
            playerToSave = new GamePlayer("", "");


            saveClass = new SaveClass();
            loadClass = new LoadClass(player.Character.Model);

            PlayerChangedEvent.AddHandler(updatePlayerAndPed);
        }



        #region EVENTS
        public void OnTick(object sender, EventArgs e)
        {
            if (Game.Player.IsDead && Game.Player.Character.Model.Hash != -1692214353 && Game.Player.Character.Model.Hash != 225514697 && Game.Player.Character.Model.Hash != -1686040670)
                loadDefaultPlayer();

            if (modelHash != GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX).Model.Hash)
            {
                modelHash = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX).Model.Hash;
                PlayerChangedEvent.EventCall();
            }
        }


        void updatePlayerAndPed()
        {
            this.player = Game.Player;
            this.playerPed = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX, player);
            this.modelHash = playerPed.Model.Hash;
        }
        #endregion



        public bool saveCharacter(string CharacterName)
        {
            playerToSave.Name = CharacterName;

            bool isSaved = saveClass.saveCharacter(playerToSave);

            if (isSaved)
            {
                PlayerSavedEvent.EventCall();
                return true;
            }
            else return false;
        }


        public void loadDefaultPlayer()
        {
            loadClass.LoadDefaultPlayer();

            PlayerChangedEvent.EventCall();
        }


        public bool loadCharacter(string CharacterName)
        {
            bool isLoaded = loadClass.LoadCharacter(CharacterName);
            if(isLoaded)
            {
                this.playerPed = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX);
                PlayerChangedEvent.EventCall();
            }

            return isLoaded;
        }


        //--------------------------------------------------//

        
        public void playSpeechByPlayer(string voice)
        {
            if(!GTAN.Call<bool>(Hash.IS_AMBIENT_SPEECH_PLAYING, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX)))
                GTAN.Call(Hash._PLAY_AMBIENT_SPEECH_WITH_VOICE, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX), "FIGHT", voice, "SPEECH_PARAMS_STANDARD", 0);
        }


        public bool isDefaultPlayer()
        {
            if (playerPed.Model.Hash == -1692214353 || playerPed.Model.Hash == 225514697 || playerPed.Model.Hash == -1686040670)
                return true;
            else return false;
        }


        public void setNewVoice(string voiceName)
        {
            this.playerPed.Voice = playerToSave.Voice = voiceName;
        }


        public List<dynamic> LoadVoiceList()
        {
            VoiceList voiceName = VoiceList.A_F_M_BEACH_01_WHITE_FULL_01;
            List<dynamic> voiceList = new List<dynamic>();

            while((int)voiceName < 791)
            {
                voiceList.Add(voiceName.ToString());

                ++voiceName;
            }

            

            return voiceList;
        }


        //--------------------------------------------------//


        public int getCurrentDrawableID(int componentId)
        {
            return GTAN.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION,
                                  GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX),
                                  componentId);
        }


        public int getCurrentTextureID(int componentId)
        {
            return GTAN.Call<int>(Hash.GET_PED_TEXTURE_VARIATION,
                                  GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX),
                                  componentId);
        }


        public int getNumberOfDrawable(int componentId)
        {
            return GTAN.Call<int>(Hash.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS,
                                  GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX),
                                  componentId);
        }


        public int getNumberOfTexture(int componentId)
        {
            return GTAN.Call<int>(Hash.GET_NUMBER_OF_PED_TEXTURE_VARIATIONS,
                                  GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX),
                                  componentId);
        }


        public void setDrawableID(int componentId, int value)
        {
            GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX),
                      componentId, value, getCurrentTextureID(componentId), 2);
        }


        public void setTextureID(int componentId, int value)
        {
            GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX),
                      componentId, getCurrentDrawableID(componentId), value, 2);
        }
    }
}
