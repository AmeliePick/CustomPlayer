using GTA;
using GTA.Native;
using GTAN = GTA.Native.Function;


namespace CustomPlayer
{
    class LoadClass
    {
        Model BaseModel { get; }


        public LoadClass(Model model)
        {
            BaseModel = new Model(model.Hash);
        }


        public bool LoadCharacter(string Name)
        {
            Character character = Character.Load(Name);

            if (character != null)
            {
                // Changing the GTA player
                while (Game.Player.Character.Model.Hash != character.ModelHash)
                {
                    Game.Player.ChangeModel(new Model(character.ModelHash));
                    System.Threading.Thread.Sleep(500);
                }

                Ped PlayerPed = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX);

                foreach (var component in character.PedComponents)
                {
                    GTAN.Call(Hash.SET_PED_COMPONENT_VARIATION, PlayerPed, (int)component.ComponentId, component.DrawableId, component.TextureId, 2);
                }

                if(character.Voice != "")
                    PlayerPed.Voice = character.Voice;

                return true;
            }
            else return false;
        }

        public void LoadDefaultPlayer()
        {
            Game.Player.ChangeModel(this.BaseModel);
        }


    }
}
