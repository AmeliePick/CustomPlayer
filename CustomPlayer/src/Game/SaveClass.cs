using System.Collections.Generic;
using System.IO;
using GTA;
using GTA.Native;
using GTAN = GTA.Native.Function;



namespace CustomPlayer
{
    class SaveClass
    {
        public SaveClass()
        {
            if (!Directory.Exists("scripts/CustomPlayer"))
                Directory.CreateDirectory("scripts/CustomPlayer");
        }


        public void SaveCharacter(GamePlayer person)
        {
            Ped playerPed = GTAN.Call<Ped>(Hash.GET_PLAYER_PED_SCRIPT_INDEX);

            // Get all ped's components
            List<Component> PedComponentsVariation = new List<Component>();
            for (PedVariationData PedComponentID = 0; (int)PedComponentID < 11; ++PedComponentID)
            {
                Component component = new Component
                (
                    PedComponentID,
                    GTAN.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, playerPed, (int)PedComponentID),
                    GTAN.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, playerPed, (int)PedComponentID)
                );

                PedComponentsVariation.Add(component);
            }

            Character character = new Character(person.Name, playerPed.Model.Hash, person.Voice, PedComponentsVariation);
            character.Save();
        }
    }
}
