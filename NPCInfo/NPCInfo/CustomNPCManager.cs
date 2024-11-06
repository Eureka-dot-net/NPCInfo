using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCInfo.NPCInfo
{
    public class CustomNPCManager
    {
        private static CustomNPCManager _instance;
        private static readonly object _lock = new object();
        private Dictionary<string, CustomNPC> npcDictionary = new Dictionary<string, CustomNPC>();

        public delegate void GiftsTodayIncreasedEventHandler(CustomNPC npc);
        public event GiftsTodayIncreasedEventHandler GiftsTodayIncreased;
        private CustomNPCManager() { }

        public static CustomNPCManager Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new CustomNPCManager();
                }
            }
        }

        public CustomNPC GetOrCreateCustomNPC(NPC character)
        {
            if (character == null || !character.CanSocialize)
                return null;

            string npcName = character.Name;

            if (!npcDictionary.TryGetValue(npcName, out var customNPC))
            {
                customNPC = new CustomNPC(character);
                npcDictionary[npcName] = customNPC; // Store the created NPC
            }

            return customNPC; // Return the existing or new NPC
        }

        public void CheckGiftsTodayChanged()
        {
            foreach (var npc in Game1.currentLocation.characters.Where(x => x.CanSocialize))
            {
                var customNpc = GetOrCreateCustomNPC(npc);
                if (customNpc == null) continue;
                if (customNpc.CheckGiftsTodayChanged())
                {
                    GiftsTodayIncreased?.Invoke(customNpc);
                }
            }
        }
    }

}
