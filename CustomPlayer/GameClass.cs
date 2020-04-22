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


        public GameClass()
        {
            player = new Player(Game.Player.Handle);
            playerPed = new Ped(GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX).Handle);

            saveClass = new SaveClass();
            loadClass = new LoadClass(player.Character.Model);
        }


        public void SaveCharacter(string CharacterName)
        {
            saveClass.SaveCharacter(CharacterName);
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
            GTAN.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX));

            GTAN.Call(Hash._PLAY_AMBIENT_SPEECH_WITH_VOICE, GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX), "GENERIC_HI", voice, "SPEECH_PARAMS_STANDARD", 0);
        }


        public List<dynamic> LoadVoiceList()
        {
            List<dynamic> voiceList = new List<dynamic>();
            voiceList.Add("A_F_M_BEACH_01_WHITE_FULL_01");

            return voiceList;
        }
    }
}
