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


    


    class GameClass
    {
        private SaveClass saveClass { get; }
        private LoadClass loadClass { get; }

        public Player player;
        public Ped playerPed;

        GamePlayer PlayerToSave;


        public GameClass()
        {
            player = new Player(Game.Player.Handle);
            playerPed = new Ped(GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX).Handle);

            PlayerToSave = new GamePlayer("", "");

            saveClass = new SaveClass();
            loadClass = new LoadClass(player.Character.Model);
        }



        public void OnTick(object sender, EventArgs e)
        {
            if (Game.Player.IsDead && Game.Player.Character.Model.Hash != -1692214353 && Game.Player.Character.Model.Hash != 225514697 && Game.Player.Character.Model.Hash != -1686040670)
            {
                LoadDefaultPlayer();
            }
        }



        public void SaveCharacter(string CharacterName)
        {
            PlayerToSave.Name = CharacterName;
            saveClass.SaveCharacter(PlayerToSave);
        }


        public void LoadDefaultPlayer()
        {
            loadClass.LoadDefaultPlayer();
        }


        public bool LoadCharacter(string CharacterName)
        {
            bool isLoaded = loadClass.LoadCharacter(CharacterName);
            if(isLoaded)
            {
                this.playerPed = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX);
            }

            return isLoaded;
        }


        public void PlaySpeechByPlayer(string voice)
        {
            if(!GTAN.Call<bool>(Hash.IS_AMBIENT_SPEECH_PLAYING, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX)))
                GTAN.Call(Hash._PLAY_AMBIENT_SPEECH_WITH_VOICE, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX), "FIGHT", voice, "SPEECH_PARAMS_STANDARD", 0);
        }


        public void SetNewVoice(string voiceName)
        {
            this.playerPed.Voice = PlayerToSave.Voice = voiceName;
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
            GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX), componentId, value, getCurrentTextureID(componentId), 2);
        }


        public void setTextureID(int componentId, int value)
        {
            GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX), componentId, getCurrentDrawableID(componentId), value, 2);
        }
    }
}
